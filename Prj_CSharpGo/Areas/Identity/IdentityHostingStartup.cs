using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prj_CSharpGo.Areas.Identity.Data;

[assembly: HostingStartup(typeof(Prj_CSharpGo.Areas.Identity.IdentityHostingStartup))]
namespace Prj_CSharpGo.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityForContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("IdentityForContextConnection")));

                services.AddDefaultIdentity<identityForUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                })
                    .AddEntityFrameworkStores<IdentityForContext>();
            });
        }
    }
}