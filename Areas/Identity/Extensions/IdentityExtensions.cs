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
        public static string GetOrganizationId(this IIdentity identity)
        {
            Claim first = ((ClaimsIdentity)identity).FindFirst("CompanyId");
            return first != null ? first.Value : string.Empty;
        }
    }
}
