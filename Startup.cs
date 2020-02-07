using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using HolidayTracker.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HolidayTracker.Extensions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddClaimsPrincipalFactory<MysUserClaimsPrincipalFactory>();
            //    //.AddDefaultTokenProviders();


            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddClaimsPrincipalFactory<MysUserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddMvc(options => options.EnableEndpointRouting = false);
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
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //    endpoints.MapRazorPages();
            //});
            try
            {
                CreateRoles(services).Wait();
            }
            catch(Exception ae)
            {
                throw ae;
            }
            
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult roleResult;
            //here in this line we are adding Admin Role

            var systemAdmin = await RoleManager.RoleExistsAsync("SystemAdmin");
            if (!systemAdmin)
            {
                //here in this line we are creating SystemAdmin role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("SystemAdmin"));
            }

            var adminRole = await RoleManager.RoleExistsAsync("Admin");
            if (!adminRole)
            {
                //here in this line we are creating admin role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }

            //here we are assigning the Admin role to the User that we have registered above 
            //Now, we are assigning admin role to this user("admin@admin.com"). When will we run this project then it will
            //be assigned to that user.
            ApplicationUser user = await UserManager.FindByEmailAsync("admin@admin.com");
            if(user == null)
            {
                var newuser = new ApplicationUser { UserName = "admin@admin.com", Email = "admin@admin.com" };
                var result = await UserManager.CreateAsync(newuser, "Admin0!");
                user = await UserManager.FindByEmailAsync("admin@admin.com");
            }
            
            await UserManager.AddToRoleAsync(user, "SystemAdmin");

            var managerRole = await RoleManager.RoleExistsAsync("Manager");
            if (!managerRole)
            {
                //here in this line we are creating Manager role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Manager"));
            }

            var approverRole = await RoleManager.RoleExistsAsync("Approver");
            if (!approverRole)
            {
                //here in this line we are creating Approver role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Approver"));
            }

            var employeeRole = await RoleManager.RoleExistsAsync("Employee");
            if (!employeeRole)
            {
                //here in this line we are creating Employee role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Employee"));
            }
        }


    }

    public class MysUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public MysUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, 
            IOptions<IdentityOptions> optionsAssessor)
            : base(userManager, optionsAssessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));
            return identity;
        }
        
    }
}