using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Manager")]
    [Authorize(Roles = "SystemAdmin")]
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

            int pageSize = 10;
            pageData.Department = await PaginatedList<Department>.CreateAsync(
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

            Department department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Department dept)
        {
            if (!ModelState.IsValid)
            {
                return View(dept);
            }

            _context.Attach(dept).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(dept.Id))
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

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(d => d.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

            Department department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Department dept)
        {
            if (!ModelState.IsValid)
            {
                return View(dept);
            }

            dept.IsDeleted = true;

            _context.Attach(dept).State = EntityState.Modified;                        

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(dept.Id))
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

        //HttpGet and HTTPPost methods to create a new Department
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Department());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department dept)
        {
            if (!ModelState.IsValid)
            {
                return View(dept);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            dept.CompanyId = currentUsersCompanyId;

            _context.Departments.Add(dept);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(dept.Id))
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

            Department department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }
    }
}
