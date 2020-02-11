using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Models.RequestType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HolidayTracker.Controllers
{
    //[Authorize(Roles = "SystemAdmin,Manager")]
    public class RequestTypeController : Controller
    {
        // GET: /<controller>/
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public RequestTypeController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.RequestTypes.IndexModel pageData = new Views.RequestTypes.IndexModel(_context);
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

            IQueryable<RequestType> dbdata = _context.RequestTypes.Where(x => x.CompanyId == currentUsersCompanyId);
            if (!String.IsNullOrEmpty(searchString))
            {
                dbdata = dbdata.Where(s => s.RequestTypeName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    dbdata = dbdata.OrderByDescending(s => s.RequestTypeName);
                    break;
                case "Code":
                    dbdata = dbdata.OrderBy(s => s.RequestTypeCode);
                    break;
                case "code_desc":
                    dbdata = dbdata.OrderByDescending(s => s.RequestTypeCode);
                    break;
                default:
                    dbdata = dbdata.OrderBy(s => s.RequestTypeName);
                    break;
            }

            int pageSize = 10;
            pageData.RequestType = await PaginatedList<RequestType>.CreateAsync(
                dbdata.AsNoTracking(), pageIndex ?? 1, pageSize);

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
