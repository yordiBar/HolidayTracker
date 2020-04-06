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
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Routing;

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
            services.AddDbContext<ApplicationDbContext>((Action<DbContextOptionsBuilder>)(options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"),
                (Action<SqlServerDbContextOptionsBuilder>)null)), ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            services.AddDefaultIdentity<ApplicationUser>((Action<IdentityOptions>)(options => options.SignIn.RequireConfirmedAccount = false))
                .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, MyUserClaimsPrincipalFactory>();
            MvcServiceCollectionExtensions.AddControllersWithViews(services);
            MvcServiceCollectionExtensions.AddMvc(services);
            services.Configure<IdentityOptions>((Action<IdentityOptions>)(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5.0);
                options.Lockout.AllowedForNewUsers = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            }));
            IdentityServiceCollectionExtensions.ConfigureApplicationCookie(services, (Action<CookieAuthenticationOptions>)(options =>
           {
               options.get_Cookie().set_HttpOnly(true);
               options.set_ExpireTimeSpan(TimeSpan.FromMinutes(5.0));
               options.set_LoginPath((PathString)"/Identity/Account/Login");
               options.set_AccessDeniedPath((PathString)"/Identity/Account/AccessDenied");
               options.set_SlidingExpiration(true);
           }));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddClaimsPrincipalFactory<MysUserClaimsPrincipalFactory>();
            //    //.AddDefaultTokenProviders();


            //services.AddIdentity<ApplicationUser, IdentityRole>(options => 
            //{
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireDigit = true;
            //})
            //    .AddRoleManager<RoleManager<IdentityRole>>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultUI()
            //    .AddClaimsPrincipalFactory<MyUserClaimsPrincipalFactory>()
            //    .AddDefaultTokenProviders();

            //services.AddControllersWithViews();
            //services.AddRazorPages();
            //services.AddMvc(options => options.EnableEndpointRouting = false);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (HostEnvironmentEnvExtensions.IsDevelopment((IHostEnvironment)env))
            {
                DeveloperExceptionPageExtensions.UseDeveloperExceptionPage(app);
                app.UseDatabaseErrorPage();
            }
            else
            {
                ExceptionHandlerExtensions.UseExceptionHandler(app, "/Home/Error");
                HstsBuilderExtensions.UseHsts(app);
            }
            HttpsPolicyBuilderExtensions.UseHttpsRedirection(app);
            StaticFileExtensions.UseStaticFiles(app);
            EndpointRoutingApplicationBuilderExtensions.UseRouting(app);
            AuthAppBuilderExtensions.UseAuthentication(app);
            AuthorizationAppBuilderExtensions.UseAuthorization(app);
            EndpointRoutingApplicationBuilderExtensions.UseEndpoints(app, (Action<IEndpointRouteBuilder>)(endpoints =>
        {
            ControllerEndpointRouteBuilderExtensions.MapControllerRoute(endpoints, "default", "{controller=Home}/{action=Index}/{id?}", (object)null, (object)null, (object)null);
            RazorPagesEndpointRouteBuilderExtensions.MapRazorPages(endpoints);
        }));
            this.CreateRoles(services).Wait();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //        app.UseDatabaseErrorPage();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Home/Error");
        //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //        app.UseHsts();
        //    }
        //    app.UseHttpsRedirection();
        //    app.UseStaticFiles();

        //    app.UseRouting();

        //    app.UseAuthentication();
        //    app.UseAuthorization();
        //    //app.UseMvc(routes =>
        //    //{
        //    //    routes.MapRoute(
        //    //        name: "default",
        //    //        template: "{controller=Home}/{action=Index}/{id?}");
        //    //});

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllerRoute(
        //            name: "default",
        //            pattern: "{controller=Home}/{action=Index}/{id?}");
        //        endpoints.MapRazorPages();
        //    });
        //    try
        //    {
        //        CreateRoles(services).Wait();
        //    }
        //    catch(Exception ae)
        //    {
        //        throw ae;
        //    }

        //}

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> RoleManager = ServiceProviderServiceExtensions.GetRequiredService<RoleManager<IdentityRole>>(serviceProvider);
            UserManager<ApplicationUser> UserManager = ServiceProviderServiceExtensions.GetRequiredService<UserManager<ApplicationUser>>(serviceProvider);
            bool roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck)
            {
                IdentityResult roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            ApplicationUser user = await UserManager.FindByEmailAsync("bart.rybak@hotmail.com");
            bool flag = await UserManager.IsInRoleAsync(user, "Admin");
            if (flag)
                return;
            IdentityResult roleAsync = await UserManager.AddToRoleAsync(user, "Admin");
        }

        //    private async Task CreateRoles(IServiceProvider serviceProvider)
        //    {
        //        var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //        var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        //        IdentityResult roleResult;
        //        //here in this line we are adding Admin Role

        //        var systemAdmin = await RoleManager.RoleExistsAsync("SystemAdmin");
        //        if (!systemAdmin)
        //        {
        //            //here in this line we are creating SystemAdmin role and seed it to the database
        //            roleResult = await RoleManager.CreateAsync(new IdentityRole("SystemAdmin"));
        //        }

        //        var adminRole = await RoleManager.RoleExistsAsync("Admin");
        //        if (!adminRole)
        //        {
        //            //here in this line we are creating admin role and seed it to the database
        //            roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
        //        }

        //        //here we are assigning the Admin role to the User that we have registered above 
        //        //Now, we are assigning admin role to this user("admin@admin.com"). When will we run this project then it will
        //        //be assigned to that user.
        //        ApplicationUser user = await UserManager.FindByEmailAsync("admin@admin.com");
        //        if(user == null)
        //        {
        //            var newuser = new ApplicationUser { UserName = "admin@admin.com", Email = "admin@admin.com" };
        //            var result = await UserManager.CreateAsync(newuser, "Admin0!");
        //            user = await UserManager.FindByEmailAsync("admin@admin.com");
        //        }

        //        await UserManager.AddToRoleAsync(user, "SystemAdmin");

        //        var managerRole = await RoleManager.RoleExistsAsync("Manager");
        //        if (!managerRole)
        //        {
        //            //here in this line we are creating Manager role and seed it to the database
        //            roleResult = await RoleManager.CreateAsync(new IdentityRole("Manager"));
        //        }

        //        var approverRole = await RoleManager.RoleExistsAsync("Approver");
        //        if (!approverRole)
        //        {
        //            //here in this line we are creating Approver role and seed it to the database
        //            roleResult = await RoleManager.CreateAsync(new IdentityRole("Approver"));
        //        }

        //        var employeeRole = await RoleManager.RoleExistsAsync("Employee");
        //        if (!employeeRole)
        //        {
        //            //here in this line we are creating Employee role and seed it to the database
        //            roleResult = await RoleManager.CreateAsync(new IdentityRole("Employee"));
        //        }
        //    }


        //}

        public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
        {
            public MyUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
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

            //protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
            //{
            //    var identity = await base.GenerateClaimsAsync(user);
            //    identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));
            //    return identity;
            //}

        }
    }
}