using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Allowance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Allowance Controller

namespace HolidayTracker.Controllers
{
    // Access control using Role-based Authorisation
    [Authorize(Roles = "Admin, Manager, SystemAdmin")]
    public class AllowanceController : Controller
    {
        // Store the connection in the variable _context
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        // Creating a connection to the database
        public AllowanceController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Action method to display Allowance view
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.Allowances.IndexModel pageData = new Views.Allowances.IndexModel(_context);
            
            int currentUsersCompanyId = User.Identity.GetCompanyId();
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

            IQueryable<Allowance> employeeIQ = _context.Allowances.Include(x => x.Employee).Where(x => x.CompanyId == currentUsersCompanyId);

            int pageSize = 10;
            pageData.Allowance = await PaginatedList<Allowance>.CreateAsync(
                employeeIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            return View(pageData);
        }

        // Method to display Allowances for employees for the currently logged in user in Edit view
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Allowance allowance = await _context.Allowances.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (allowance == null)
            {
                return NotFound();
            }

            return View(allowance);
        }

        // Method to Edit annual allowances for employees of the currently logged in user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Allowance all)
        {
            if (!ModelState.IsValid)
            {
                return View(all);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            all.CompanyId = currentUsersCompanyId;

            _context.Attach(all).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllowanceExists(all.Id))
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

        // Boolean method to check if allowances exist
        private bool AllowanceExists(int id)
        {
            return _context.Allowances.Any(d => d.Id == id);
        }
    }
}
