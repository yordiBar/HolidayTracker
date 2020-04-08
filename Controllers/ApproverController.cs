using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            //viewModel.Requests = _context.Requests.Include(r => r.RequestType).Where(x => x.EmployeeId == currentUserId).ToList();

            List<int> myEmployeesId = _context.Employees.Where(x => x.ApproverId == currentUserId).Select(x => x.Id).ToList();

            viewModel.Requests = _context.Requests.Include(r => r.RequestType).Include(e => e.Employee).Where(x => myEmployeesId.Contains(x.EmployeeId) && x.Status == 0).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult DisplayAllRequests()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            int currentUserId = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).Select(x => x.Id).FirstOrDefault();

            ApprovalsViewModel viewModel = new ApprovalsViewModel();

            List<int> myEmployeesId = _context.Employees.Where(x => x.ApproverId == currentUserId).Select(x => x.Id).ToList();

            viewModel.Requests = _context.Requests.Include(r => r.RequestType).Include(e => e.Employee).Where(x => myEmployeesId.Contains(x.EmployeeId) && x.Status > 0).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            int currentUserId = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).Select(x => x.Id).FirstOrDefault();

            List<int> myEmployeesId = _context.Employees.Where(x => x.ApproverId == currentUserId).Select(x => x.Id).ToList();

            Request requestPending = _context.Requests.Where(r => r.Id == Id && myEmployeesId.Contains(r.EmployeeId)).FirstOrDefault();

            if (requestPending != null)
            {
                requestPending.Status = 1;

                _context.Attach(requestPending).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();

                    //  When I want to return success:
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json("Saved!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Failed"); ;
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            int currentUserId = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).Select(x => x.Id).FirstOrDefault();

            List<int> myEmployeesId = _context.Employees.Where(x => x.ApproverId == currentUserId).Select(x => x.Id).ToList();

            Request requestPending = _context.Requests.Where(r => r.Id == Id && myEmployeesId.Contains(r.EmployeeId)).FirstOrDefault();

            if (requestPending != null)
            {
                requestPending.Status = 4;

                _context.Attach(requestPending).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();

                    //  When I want to return success:
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json("Saved!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Failed"); ;
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelRequest(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            int currentUserId = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).Select(x => x.Id).FirstOrDefault();

            List<int> myEmployeesId = _context.Employees.Where(x => x.ApproverId == currentUserId).Select(x => x.Id).ToList();

            Request requestPending = _context.Requests.Where(r => r.Id == Id && myEmployeesId.Contains(r.EmployeeId)).FirstOrDefault();

            if (requestPending != null)
            {
                requestPending.Status = 3;

                _context.Attach(requestPending).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();

                    //  When I want to return success:
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json("Saved!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Failed"); ;
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed");
            }
        }
    }

    public class ApprovalsViewModel
    {
        // list of requests
        public List<Request> Requests { get; set; }

        public List<Employee> Employees { get; set; }

    }

    
}