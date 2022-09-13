using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WebApi.Security.Contexts;
using WebApi.Security.Entities;

[assembly: HostingStartup(typeof(WebApi.Areas.Identity.IdentityHostingStartup))]
namespace WebApi.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("IdentityDbContextConnection")));

                //services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<IdentityDbContext>();
                services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddDefaultUI()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<IdentityDbContext>();
            });
        }
    }
}