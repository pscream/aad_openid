using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using WebApi.Models.Responses;
using WebApi.Security.Contexts;
using WebApi.Security.Entities;
using WebApi.Security.Services;

using TokenValidatedContext = Microsoft.AspNetCore.Authentication.OpenIdConnect.TokenValidatedContext;

[assembly: HostingStartup(typeof(WebApi.Areas.Identity.IdentityHostingStartup))]
namespace WebApi.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {

                var jwtKey = Encoding.UTF8.GetBytes(context.Configuration["ApplicationSettings:JwtKey"]);
                var authority = context.Configuration["ApplicationSettings:AAD:Authority"];
                var tenantId = context.Configuration["ApplicationSettings:AAD:TenantId"];
                var clientId = context.Configuration["ApplicationSettings:AAD:ClientId"];
                var clientSecret = context.Configuration["ApplicationSettings:AAD:ClientSecret"];

                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("IdentityDbContextConnection")));

                services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddDefaultUI()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<IdentityDbContext>();

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {

                    options.SignInScheme = IdentityConstants.ApplicationScheme;

                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                    options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
                    options.CallbackPath = new PathString("/signin-oidc");
                    // Here we add all possible scopes to get as much data as possible
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name", // Specify the key name in 'id_token' content where to get a value for 'User.Identity.Name'
                        ValidateIssuer = true
                    };

                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = AfterAuthentication,

                    };
                }).AddCookie();
            });
        }

        private async Task AfterAuthentication(TokenValidatedContext context)
        {
            var username = context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimConstants.PreferredUserName)?.Value;
            var userId = context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ObjectId)?.Value;

            var _securityManager = context.HttpContext.RequestServices.GetRequiredService<ISecurityManager>();
            var token = await _securityManager.CreateExternalUserToken(username);
            if (token == null)
            {
                await _securityManager.CreateOrUpdateExternalUser(username, new Guid(userId));
                token = await _securityManager.CreateExternalUserToken(username);
            }

            if (token != null)
            {
                // The rest of the code breaks the redirection chain in case we want to generate a Jwt and redirect explicitly at the Frontend
                context.Response.ContentType = "application/json; charset=utf-8";
                await JsonSerializer.SerializeAsync(context.Response.Body, 
                    new JwtToken(token, context.SecurityToken.RawData, context.SecurityToken.EncodedHeader, context.SecurityToken.EncodedPayload), typeof(JwtToken));
                context.HandleResponse();
            }
        }

    }
}