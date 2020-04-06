using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Employee;
using HolidayTracker.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HolidayTracker.Controllers
{
    //[Authorize(Roles = "Approver")]
    //[Authorize(Roles = "Manager")]
    public class ApproverController : Controller
    {

        private readonly ILogger<ApproverController> _logger;
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        public ApproverController(ILogger<ApproverController> logger, HolidayTracker.Data.ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            int currentUserId = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).Select(x => x.Id).FirstOrDefault();

            ApprovalsViewModel viewModel = new ApprovalsViewModel();
            //viewModel.Requests = _context.Requests.Include(r => r.RequestType).Where(x => x.RequestCreatedByEmployeeId == currentUserId).ToList();

            // get all Employees for the logged in approver
            List<int> myEmployeesId = _context.Employees.Where(x => x.ApproverId == currentUserId).Select(x => x.Id).ToList();

            // get all pending requests for the above Employees
            viewModel.Requests = _context.Requests.Include(r => r.RequestType).Where(x => myEmployeesId.Contains(x.EmployeeId) && x.Status == 0).ToList();

            return View(viewModel);
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
       
    }

    public class ApprovalsViewModel
    {
        // list of requests
        public List<Request> Requests { get; set; }

        public List<Employee> Employees { get; set; }

    }
}