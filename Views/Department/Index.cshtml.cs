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

        //public async Task OnGetAsync()
        //{
        //    Department = await _context.Departments.ToListAsync();
        //}
    }
}
