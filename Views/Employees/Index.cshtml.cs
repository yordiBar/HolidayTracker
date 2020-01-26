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

        public string IdSort { get; set; }
        public string LocationIdSort { get; set; }
        public string DepartmentIdSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string StartDateSort { get; set; }
        public string LeavingDateSort { get; set; }
        public string EmployeeTypeIdSort { get; set; }
        public string EmployeeNumberSort { get; set; }
        public string JobTitleSort { get; set; }
        public string GenderIdSort { get; set; }
        public string CompanyIdSort { get; set; }

        public IList<Employee> Employees { get;set; }

        public async Task OnGetAsync()
        {
            Employees = await _context.Employees.ToListAsync();
        }
    }
}
