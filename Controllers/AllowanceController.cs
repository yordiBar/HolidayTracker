using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Models.Allowance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Controllers
{
    public class AllowanceController : Controller
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public AllowanceController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.Allowances.IndexModel pageData = new Views.Allowances.IndexModel(_context);
            //var user = new ApplicationUser { CompanyId = model.CompanyId };
            int currentUsersCompanyId = 1;
            pageData.CurrentSort = sortOrder;
            pageData.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            pageData.DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            pageData.CurrentFilter = searchString;

            IQueryable<Allowance> employeeIQ = _context.Allowances.Where(x => x.CompanyId == currentUsersCompanyId);
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    employeeIQ = employeeIQ.Where(s => s.DisplayName.Contains(searchString));
            //}
            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        employeeIQ = employeeIQ.OrderByDescending(s => s.DisplayName);
            //        break;
            //    case "Date":
            //        employeeIQ = employeeIQ.OrderBy(s => s.StartDate);
            //        break;
            //    case "date_desc":
            //        employeeIQ = employeeIQ.OrderByDescending(s => s.StartDate);
            //        break;
            //    default:
            //        employeeIQ = employeeIQ.OrderBy(s => s.DisplayName);
            //        break;
            //}

            int pageSize = 10;
            pageData.Allowance = await PaginatedList<Allowance>.CreateAsync(
                employeeIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            return View(pageData);
        }
    }
}
