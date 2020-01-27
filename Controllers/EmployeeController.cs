using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Models.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HolidayTracker.Controllers
{
    [Authorize(Roles = "Admin,Employee,Manager,Approver")]
    public class EmployeeController : Controller
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public EmployeeController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Employee> data = new List<Employee>();
            //logged in users company id
            string userCompanyId = "1";
            data = _context.Employees.Where(row => row.CompanyId == userCompanyId).ToList();

            return View(data);
        }
    }
}