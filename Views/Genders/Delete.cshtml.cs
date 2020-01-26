using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HolidayTracker.Data;
using HolidayTracker.Models.Gender;

namespace HolidayTracker.Views.Genders
{
    public class DeleteModel : PageModel
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        public DeleteModel(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Gender Gender { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Gender = await _context.Genders.FirstOrDefaultAsync(m => m.Id == id);

            if (Gender == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Gender = await _context.Genders.FindAsync(id);

            if (Gender != null)
            {
                _context.Genders.Remove(Gender);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
