using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HolidayTracker.Controllers
{
    [Authorize(Roles = "Manager")]
    public class DepartmentController : Controller
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public DepartmentController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
