using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HolidayTracker.Data;
using HolidayTracker.Models.RequestType;

namespace HolidayTracker.Views.RequestTypes
{
    public class DetailsModel : PageModel
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        public DetailsModel(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public RequestType RequestType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RequestType = await _context.RequestTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (RequestType == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
