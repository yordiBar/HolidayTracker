using HolidayTracker.Models.Allowance;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Views.Allowances
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

        public PaginatedList<Allowance> Allowance { get; set; }
    }
}
