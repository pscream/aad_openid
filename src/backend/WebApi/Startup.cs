using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WebApi.Security.Services;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISecurityManager, SecurityManager>();
            services.AddControllers();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/debug/routes", PrintEndpoints);

                //endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }

        private async Task PrintEndpoints(HttpContext context)
        {
            var service = (IEnumerable<EndpointDataSource>)context.RequestServices.GetService(typeof(IEnumerable<EndpointDataSource>));
            var endpoints = service.SelectMany(es => es.Endpoints);
            var sb = new StringBuilder();
            foreach (var endpoint in endpoints)
            {
                if (sb.Length == 0)
                    sb.Append(endpoint.DisplayName);
                else
                {
                    sb.AppendLine();
                    sb.Append(endpoint.DisplayName);
                }
                var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
                if (httpMethodsMetadata?.HttpMethods != null)
                {
                    if (httpMethodsMetadata.HttpMethods.Count > 0)
                    {
                        sb.Append($": {string.Join(", ", httpMethodsMetadata.HttpMethods)}");
                    }
                }

                var controllerActionDescriptorMetadata = endpoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
                if (controllerActionDescriptorMetadata?.RouteValues != null)
                {
                    if (controllerActionDescriptorMetadata.RouteValues.Count > 0)
                    {
                        sb.Append($": {string.Join(", ", controllerActionDescriptorMetadata.RouteValues.Select(routeValue => $"{routeValue.Key}='{routeValue.Value}'"))}");
                    }
                }

                var routeEndpoint = endpoint as RouteEndpoint;
                if (routeEndpoint != null)
                {
                    sb.Append($": {routeEndpoint.RoutePattern.RawText}");
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
