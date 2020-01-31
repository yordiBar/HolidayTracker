using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HolidayTracker.Data;
using HolidayTracker.Models.Employee;

namespace HolidayTracker.Views.Employees
{
    public class IndexModel : PageModel
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        public IndexModel(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Employee> Employee { get;set; }

        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Employee> employeeIQ = from s in _context.Employees
                                             select s;
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
            Employee = await PaginatedList<Employee>.CreateAsync(
                employeeIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        //public async Task OnGetAsync()
        //{
        //    Employee = await _context.Employees.ToListAsync();
        //}
    }
}
