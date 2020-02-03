using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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

        [HttpPost]
        public IActionResult Create()
        {
            return RedirectToAction("Create");
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
    }
}
