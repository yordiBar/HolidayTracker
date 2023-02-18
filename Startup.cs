using HolidayTracker.Areas.Identity.Data;
using HolidayTracker.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HolidayTracker
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
            // Use ApplicationDBContext to store user permissions
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, MyUserClaimsPrincipalFactory>();
            services.AddControllersWithViews();
            services.AddMvc();
            //added
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20.0);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
            //added
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20.0);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Seed roles before defining endpoints
            SeedRoles(services).Wait();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Landing}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        // A method to store CompanyId for logged in users
        public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
        {
            public MyUserClaimsPrincipalFactory(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
                Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager,
                IOptions<IdentityOptions> optionsAssessor)
                : base(userManager, roleManager, optionsAssessor)
            {
            }

            public override async Task<ClaimsPrincipal> CreateAsync(
                ApplicationUser user)
            {
                ClaimsPrincipal principal = await base.CreateAsync(user);
                ((ClaimsIdentity)principal.Identity).AddClaims((IEnumerable<Claim>)new Claim[1]
            {
                new Claim("CompanyId", user.CompanyId.ToString())
            });
                return principal;
            }
        }

        private async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();

            Microsoft.AspNetCore.Identity.IdentityResult roleResult;

            // Create roles if they don't exist
            var systemAdminRole = await roleManager.RoleExistsAsync("SystemAdmin");
            if (!systemAdminRole)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole("SystemAdmin"));
            }

            var adminRole = await roleManager.RoleExistsAsync("Admin");
            if (!adminRole)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var managerRole = await roleManager.RoleExistsAsync("Manager");
            if (!managerRole)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole("Manager"));
            }

            var approverRole = await roleManager.RoleExistsAsync("Approver");
            if (!approverRole)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole("Approver"));
            }

            var employeeRole = await roleManager.RoleExistsAsync("Employee");
            if (!employeeRole)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole("Employee"));
            }

            // Create admin user if they don't exist
            var adminUser = await userManager.FindByEmailAsync("admin@admin.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@admin.com"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin0!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
                }
            }
        }


        private async Task CreateRole(Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Error creating role '{roleName}': {roleResult.Errors}");
                }
            }
        }
    }
}