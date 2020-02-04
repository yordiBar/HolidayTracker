using HolidayTracker.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace HolidayTracker.Extensions
{
    ///
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.1
    /// 
    public class ApplicationUser : IdentityUser
    {
        public string ContactName { get; set; }
        public int CompanyId { get; set; }

        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    ApplicationDbContext db = new ApplicationDbContext();
        //    var managerx = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        //    var currentUser = managerx.FindById(User.Indentity.GetUserId());
        //    var newProperty = currentUser.CompanyId;

        //}
    }

    public static class IdentityExtensions
    {
        public static int GetCompanyId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("CompanyId");
            return (claim != null) ? Int32.Parse(claim.Value) : -1; // inline if condition to check if the claim has a value
        }
    }

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //        : base(options)
    //    {
    //    }

    //    protected override void OnModelCreating(ModelBuilder builder)
    //    {
    //        base.OnModelCreating(builder);
    //    }
    //}
}
