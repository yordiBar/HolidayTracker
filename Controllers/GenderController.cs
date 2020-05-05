using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Gender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Gender Controller

namespace HolidayTracker.Controllers
{
    // Access control using Role-based Authorisation
    [Authorize(Roles = "Admin, Manager")]
    public class GenderController : Controller
    {
        
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public GenderController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Action method to display Gender view
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.Genders.IndexModel pageData = new Views.Genders.IndexModel(_context);
            
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

            IQueryable<Gender> dbdata = _context.Genders.Where(x => x.CompanyId == currentUsersCompanyId);
            if (!String.IsNullOrEmpty(searchString))
            {
                dbdata = dbdata.Where(s => s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    dbdata = dbdata.OrderByDescending(s => s.Name);
                    break;
                case "Code":
                    dbdata = dbdata.OrderBy(s => s.Id);
                    break;
                case "code_desc":
                    dbdata = dbdata.OrderByDescending(s => s.Id);
                    break;
                default:
                    dbdata = dbdata.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            pageData.Gender = await PaginatedList<Gender>.CreateAsync(
                dbdata.AsNoTracking(), pageIndex ?? 1, pageSize);

            return View(pageData);
        }

        // HttpGet method to display Gender Edit view
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Gender gender = await _context.Genders.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (gender == null)
            {
                return NotFound();
            }
            return View(gender);
        }

        // HttpPost method to edit genders
        [HttpPost]
        public async Task<IActionResult> Edit(Gender gen)
        {
            if (!ModelState.IsValid)
            {
                return View(gen);
            }

            _context.Attach(gen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenderExists(gen.Id))
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

        // A boolean method to check if any genders exist
        private bool GenderExists(int id)
        {
            return _context.Genders.Any(g => g.Id == id);
        }

        // HttpGet method to display create Genders view
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Gender());
        }

        // HttpPost method to create genders
        [HttpPost]
        public async Task<IActionResult> Create(Gender gen)
        {
            if (!ModelState.IsValid)
            {
                return View(gen);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            gen.CompanyId = currentUsersCompanyId;

            _context.Genders.Add(gen);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenderExists(gen.Id))
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

        // Action method to return genders created for the company of the currently logged in user
        // It is displayed in the Details view
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Gender gender = await _context.Genders.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (gender == null)
            {
                return NotFound();
            }
            return View(gender);
        }

        // HttpGet method to display Delete Gender view
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Gender gender = await _context.Genders.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (gender == null)
            {
                return NotFound();
            }
            return View(gender);
        }

        // HttpPost method to delete genders
        [HttpPost]
        public async Task<IActionResult> Delete(Gender gen)
        {
            if (!ModelState.IsValid)
            {
                return View(gen);
            }

            gen.IsDeleted = true;

            _context.Attach(gen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenderExists(gen.Id))
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
    }
}
