using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HolidayTracker.Data;
using HolidayTracker.Models.Department;

namespace HolidayTracker.Views.Departments
{
    public class IndexModel : PageModel
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        public IndexModel(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string CodeSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Department> Department { get;set; }

        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CodeSort = sortOrder == "Code" ? "code_desc" : "Code";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Department> employeeIQ = from s in _context.Departments
                                                select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                employeeIQ = employeeIQ.Where(s => s.DepartmentName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    employeeIQ = employeeIQ.OrderByDescending(s => s.DepartmentName);
                    break;
                case "Date":
                    employeeIQ = employeeIQ.OrderBy(s => s.DepartmentCode);
                    break;
                case "date_desc":
                    employeeIQ = employeeIQ.OrderByDescending(s => s.DepartmentCode);
                    break;
                default:
                    employeeIQ = employeeIQ.OrderBy(s => s.DepartmentName);
                    break;
            }

            int pageSize = 3;
            Department = await PaginatedList<Department>.CreateAsync(
                employeeIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }        
}
