using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Employee;
using HolidayTracker.Models.Request;
using HolidayTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// Approver Controller

namespace HolidayTracker.Controllers
{
    // Access control using Role-based Authorisation
    [Authorize(Roles = "Approver, Manager")]
    public class ApproverController : Controller
    {

        
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        public ApproverController( HolidayTracker.Data.ApplicationDbContext context)
        {
            
            _context = context;
        }

        // Method to display all pending request of Employees that the logged in user is an Approver for
        [HttpGet]
        public IActionResult Index()
        {

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            int currentUserId = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).Select(x => x.Id).FirstOrDefault();


            ApprovalsViewModel viewModel = new ApprovalsViewModel();
            
            List<int> myEmployeesId = _context.Employees.Where(x => x.ApproverId == currentUserId).Select(x => x.Id).ToList();

            viewModel.Requests = _context.Requests.Include(r => r.RequestType).Include(e => e.Employee).Where(x => myEmployeesId.Contains(x.EmployeeId) && x.Status == 0).ToList();

            return View(viewModel);
        }

        // A HttpGet method display the request history for the logged in user
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

        // HttpPost method to approve a request
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

        //HttpPOst method to reject a request
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

        // HttpPost method to cancel a request
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
}