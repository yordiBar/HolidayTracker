using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.RequestType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// RequestType Controller

namespace HolidayTracker.Controllers
{
    // Access control using Role-based Authorisation
    [Authorize(Roles = "Admin, Manager")]
    public class RequestTypeController : Controller
    {
        
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public RequestTypeController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Action method to display Request Type view
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.RequestTypes.IndexModel pageData = new Views.RequestTypes.IndexModel(_context);
            
            int currentUsersCompanyId = User.Identity.GetCompanyId();
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

        // HttpGet method to display Request Type Edit view
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            RequestType requestType = await _context.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (requestType == null)
            {
                return NotFound();
            }

            return View(requestType);
        }

        // HttpPost method to edit Request Types
        [HttpPost]
        public async Task<IActionResult> Edit(RequestType reqType)
        {
            if (!ModelState.IsValid)
            {
                return View(reqType);
            }

            _context.Attach(reqType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestTypeExists(reqType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }                
            }

            return RedirectToAction("Index");
        }

        // A boolean method to check if any Reqyest Types exist
        private bool RequestTypeExists(int id)
        {
            return _context.RequestTypes.Any(d => d.Id == id);
        }

        // HttpGet method to display Request Type Delete view
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            RequestType requestType = await _context.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (requestType == null)
            {
                return NotFound();
            }
            return View(requestType);
        }

        // HttpPost method to delete Request Types
        [HttpPost]
        public async Task<IActionResult> Delete(RequestType reqType)
        {
            if (!ModelState.IsValid)
            {
                return View(reqType);
            }

            reqType.IsDeleted = true;

            _context.Attach(reqType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestTypeExists(reqType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        // HttpGet method to display Request Type Create view
        [HttpGet]
        public IActionResult Create()
        {
            return View(new RequestType());
        }

        // HttpPost method to create Request Types
        [HttpPost]
        public async Task<IActionResult> Create(RequestType reqType)
        {
            if (!ModelState.IsValid)
            {
                return View(reqType);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            reqType.CompanyId = currentUsersCompanyId;

            _context.RequestTypes.Add(reqType);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestTypeExists(reqType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        // Method to return Request Types created for the company of the currently logged in user
        // It is displayed in the Details view
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            RequestType requestType = await _context.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (requestType == null)
            {
                return NotFound();
            }
            return View(requestType);
        }
    }
}
