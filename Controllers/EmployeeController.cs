using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Models.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Controllers
{
    [Authorize(Roles = "Admin,Employee,Manager,Approver")]
    public class EmployeeController : Controller
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public EmployeeController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.Employees.IndexModel pageData = new Views.Employees.IndexModel(_context);
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

            IQueryable<Employee> employeeIQ = _context.Employees.Where(x => x.CompanyId == currentUsersCompanyId);
            if (!String.IsNullOrEmpty(searchString))
            {
                employeeIQ = employeeIQ.Where(s => s.DisplayName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    employeeIQ = employeeIQ.OrderByDescending(s => s.DisplayName);
                    break;
                case "Date":
                    employeeIQ = employeeIQ.OrderBy(s => s.StartDate);
                    break;
                case "date_desc":
                    employeeIQ = employeeIQ.OrderByDescending(s => s.StartDate);
                    break;
                default:
                    employeeIQ = employeeIQ.OrderBy(s => s.DisplayName);
                    break;
            }

            int pageSize = 3;
            pageData.Employee = await PaginatedList<Employee>.CreateAsync(
                employeeIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            return View(pageData);
        }

        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
    }
}