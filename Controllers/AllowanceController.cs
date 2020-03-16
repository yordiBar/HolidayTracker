using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
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
            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();
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

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId(); //User.Identity.GetCompanyId();

            Allowance allowance = await _context.Allowances.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (allowance == null)
            {
                return NotFound();
            }

            return View(allowance);
        }

        [HttpPost]
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

        private bool AllowanceExists(int id)
        {
            return _context.Allowances.Any(d => d.Id == id);
        }

        //[HttpGet]
        //public async Task<IActionResult> Create()
        //{
        //    return View(new Allowance());
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(Allowance all)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(all);
        //    }

        //    int currentUsersCompanyId = 1;

        //    all.CompanyId = currentUsersCompanyId;

        //    _context.Allowances.Add(all);

        //    try
        //    {
        //        await _context.SaveChangesAsync();

        //        var allowance = new Allowance { From = all.From, To = all.To, Amount = all.Amount, CarryOver = all.CarryOver };

        //        var result = await _context.AddAsync(allowance);

                
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!AllowanceExists(all.EmployeeId))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }                
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}
