using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HolidayTracker.Data;
using HolidayTracker.Models.RequestType;

namespace HolidayTracker.Views.RequestTypes
{
    public class EditModel : PageModel
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        public EditModel(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(RequestType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestTypeExists(RequestType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RequestTypeExists(int id)
        {
            return _context.RequestTypes.Any(e => e.Id == id);
        }
    }
}
