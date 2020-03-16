using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Gender;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Controllers
{
    public class GenderController : Controller
    {
        // GET: /<controller>/
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public GenderController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.Genders.IndexModel pageData = new Views.Genders.IndexModel(_context);
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

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

            Gender gender = await _context.Genders.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (gender == null)
            {
                return NotFound();
            }
            return View(gender);
        }

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

        private bool GenderExists(int id)
        {
            return _context.Genders.Any(g => g.Id == id);
        }

        //HttpGet and HTTPPost methods to create a new Gender
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Gender());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Gender gen)
        {
            if (!ModelState.IsValid)
            {
                return View(gen);
            }

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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

            Gender gender = await _context.Genders.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (gender == null)
            {
                return NotFound();
            }
            return View(gender);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

            Gender gender = await _context.Genders.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (gender == null)
            {
                return NotFound();
            }
            return View(gender);
        }

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
