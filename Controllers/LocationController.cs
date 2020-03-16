using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Location;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HolidayTracker.Controllers
{
    public class LocationController : Controller
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public LocationController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.Locations.IndexModel pageData = new Views.Locations.IndexModel(_context);
            //var user = new ApplicationUser { CompanyId = model.CompanyId };
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

            IQueryable<Location> dbdata = _context.Locations.Where(x => x.CompanyId == currentUsersCompanyId);
            if (!String.IsNullOrEmpty(searchString))
            {
                dbdata = dbdata.Where(s => s.LocationName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    dbdata = dbdata.OrderByDescending(s => s.LocationName);
                    break;
                case "Code":
                    dbdata = dbdata.OrderBy(s => s.LocationCode);
                    break;
                case "code_desc":
                    dbdata = dbdata.OrderByDescending(s => s.LocationCode);
                    break;
                default:
                    dbdata = dbdata.OrderBy(s => s.LocationName);
                    break;
            }

            int pageSize = 10;
            pageData.Location = await PaginatedList<Location>.CreateAsync(
                dbdata.AsNoTracking(), pageIndex ?? 1, pageSize);

            return View(pageData);
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

            Location location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Location loc)
        {
            if (!ModelState.IsValid)
            {
                return View(loc);
            }

            _context.Attach(loc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(loc.Id))
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

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(l => l.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

            Location location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Location loc)
        {
            if (!ModelState.IsValid)
            {
                return View(loc);
            }

            loc.IsDeleted = true;

            _context.Attach(loc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(loc.Id))
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

        //HttpGet and HTTPPost methods to create a new Location
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Location());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Location loc)
        {
            if (!ModelState.IsValid)
            {
                return View(loc);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            loc.CompanyId = currentUsersCompanyId;

            _context.Locations.Add(loc);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(loc.Id))
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

            Location location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }
    }
}
