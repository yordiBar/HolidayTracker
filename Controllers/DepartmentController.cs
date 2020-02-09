using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Models.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Controllers
{
    //[Authorize(Roles = "SystemAdmin,Manager")]
    public class DepartmentController : Controller
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public DepartmentController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.Departments.IndexModel pageData = new Views.Departments.IndexModel(_context);
            //var user = new ApplicationUser { CompanyId = model.CompanyId };
            int currentUsersCompanyId = 1;
            pageData.CurrentSort = sortOrder;
            pageData.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            pageData.CodeSort = sortOrder == "Code" ? "code_desc" : "Code";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            pageData.CurrentFilter = searchString;

            IQueryable<Department> dbdata = _context.Departments.Where(x => x.CompanyId == currentUsersCompanyId);
            if (!String.IsNullOrEmpty(searchString))
            {
                dbdata = dbdata.Where(s => s.DepartmentName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    dbdata = dbdata.OrderByDescending(s => s.DepartmentName);
                    break;
                case "Code":
                    dbdata = dbdata.OrderBy(s => s.DepartmentCode);
                    break;
                case "code_desc":
                    dbdata = dbdata.OrderByDescending(s => s.DepartmentCode);
                    break;
                default:
                    dbdata = dbdata.OrderBy(s => s.DepartmentName);
                    break;
            }

            int pageSize = 3;
            pageData.Department = await PaginatedList<Department>.CreateAsync(
                dbdata.AsNoTracking(), pageIndex ?? 1, pageSize);

            return View(pageData);
        }

        [HttpPost]
        public IActionResult Create()
        {
            return RedirectToAction("Create");
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
    }
}
