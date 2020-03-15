using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HolidayTracker.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync( UserManager<ApplicationUser> manager)
        {
            ClaimsIdentity userIdentity = new ClaimsIdentity("Cookies");
            userIdentity.AddClaim(new Claim("CompanyId", this.CompanyId.ToString()));
            return userIdentity;
        }

        //public string DisplayName { get; set; }

        public string ContactName { get; set; }

        public int CompanyId { get; set; }
    }
}
