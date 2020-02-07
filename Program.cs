using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HolidayTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                using (var services = host.Services.CreateScope())
                {
                    var dbContext = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var userMgr = services.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                    var roleMgr = services.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    dbContext.Database.Migrate();

                    var adminRole = new IdentityRole("Admin");

                    if (!dbContext.Roles.Any())
                    {
                        roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();
                    }

                    if (!dbContext.Users.Any(u => u.UserName == "admin"))
                    {
                        var adminUser = new IdentityUser
                        {
                            UserName = "admin@admin.com",
                            Email = "admin@admin.com"
                        };
                        var result = userMgr.CreateAsync(adminUser, "password").GetAwaiter().GetResult();
                        userMgr.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
                    }
                }
                host.Run();
            }
            catch(AggregateException ae)
            {
                throw ae;
            }
            
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
