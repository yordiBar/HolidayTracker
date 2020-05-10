using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace HolidayTracker.Areas.Identity.Extensions
{
    public static class IdentityExtensions
    {
        // A method to get the CompanyId from the Claims
        public static int GetCompanyId(this IIdentity identity)
        {
            Claim first = ((ClaimsIdentity)identity).FindFirst("CompanyId");
            return first != null ? Int32.Parse(first.Value) : 0;
        }
    }
}
