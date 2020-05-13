using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HolidayTracker.Models;
using HolidayTracker.Models.RequestType;
using Microsoft.EntityFrameworkCore;
using HolidayTracker.Models.Request;
using System.Net;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Employee;
using HolidayTracker.Models.Allowance;
using Microsoft.AspNetCore.Authorization;

// Home Controller

namespace HolidayTracker.Controllers
{
    
    public class HomeController : Controller
    {
        // Store the connection in the variable _context
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        // Creating a connection to the database
        public HomeController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Action method to display Employee Dashboard page
        // Authorisation is set to allow all Employees to see their Dashboard containing holiday requests functionality

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public IActionResult Dashboard()
        {

            int currentUsersCompanyId = User.Identity.GetCompanyId();
            int currentUserId = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).Select(x => x.Id).FirstOrDefault();

            HomeViewModel viewModel = new HomeViewModel();
            viewModel.Requests = _context.Requests.Include(r => r.RequestType).Where(x => x.EmployeeId == currentUserId).ToList();

            // get first day and last day of the current year
            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            DateTime firstDayNextYear = lastDay.AddDays(1); // get first day of next year

            // Allowance balance to be displayed in a table on employee dashboard page
            // Using AllowanceBalance Data Transfer Object
            Allowance allowance = _context.Allowances.Where(x => x.EmployeeId == currentUserId && x.From == firstDay).FirstOrDefault();

            AllowanceBalance allowanceBalance = new AllowanceBalance();
                        
            allowanceBalance.StandardAllowance = allowance.Amount;

            allowanceBalance.Pending = viewModel.Requests.Where(x => x.Status == 0 && x.RequestType.TakesFromAllowance == true && x.From >= firstDay && x.To < firstDayNextYear).Sum(x => x.RequestAmount);

            allowanceBalance.Taken = viewModel.Requests.Where(x => x.Status == 1 && x.RequestType.TakesFromAllowance == true && x.From >= firstDay && x.To < firstDayNextYear && x.From < DateTime.Now).Sum(x => x.RequestAmount);

            allowanceBalance.Approved = viewModel.Requests.Where(x => x.Status == 1 && x.RequestType.TakesFromAllowance == true && x.From >= firstDay && x.To < firstDayNextYear && x.From > DateTime.Now).Sum(x => x.RequestAmount);

            viewModel.Balance = allowanceBalance;


            return View(viewModel);
        }

        // Action method to display Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Acion method to display Contact page
        public IActionResult Contact()
        {
            return View();
        }

        // Action method to display Home
        public IActionResult Index()
        {
            return View();
        }

        // Action method to display Landing Page
        public IActionResult Landing()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // HttpGet method to retrieve Request Types created for the current company
        [HttpGet]
        public async Task<IActionResult> GetRequestType(string query)
        {

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            List<RequestType> requestList = _context.RequestTypes.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false).ToList();
            
            List<RequestType> requestResults = new List<RequestType>();
            foreach (var request in requestList)
            {
                if (String.IsNullOrEmpty(query) || request.RequestTypeName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    requestResults.Add(request);
                }
            }

            requestResults.Sort(delegate (RequestType r1, RequestType r2) { return r1.RequestTypeName.CompareTo(r2.RequestTypeName); });

            var serialisedJson = from result in requestResults
                                 select new
                                 {
                                     text = result.RequestTypeName,
                                     id = result.Id
                                 };

            return Json(serialisedJson);
        }

        // HttpPost method to create a new request for the logged in user
        [HttpPost]        
        public async Task<IActionResult> CreateRequest(CreateRequestDTO data)
        {
            try
            {
                //save data to database
                //bool saveSuccess = true;

                // take data from CreateRequestDTO
                // insert it into new request
                // save request to db
                int currentUsersCompanyId = User.Identity.GetCompanyId();

                // select Employee from database
                Employee employee = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).FirstOrDefault();

                // Create a new request and add request data
                Request request = new Request();

                request.CompanyId = currentUsersCompanyId;
                request.RequestTypeId = data.RequestTypeId;
                request.EmployeeId = employee.Id;
                request.RequestCreatedByEmployeeId = employee.Id;
                request.From = data.RealFrom;
                request.To = data.RealTo;
                request.Status = (int)RequestStatus.Pending;
                request.Description = data.Description;
                request.RequestAmount = GetDaysTaken(request, employee);
                _context.Requests.Add(request);


                await _context.SaveChangesAsync();

                if (request.Id <= 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Failed");
                }
                else
                {
                    //  When I want to return success:
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json("Saved!");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed"); ;
            }
        }

        // GetGaysTaken method to calculate days requested by an employee from new request
        private double GetDaysTaken(Request data, Employee employee)
        {
            double calcDays = 0;

            // check if a request exists
            if (data != null)
            {
                // check days the employee is working, if true add to requested days
                for (DateTime date = data.From; date.Date <= data.To; date = date.AddDays(1))
                {

                    if (date.DayOfWeek == DayOfWeek.Monday && employee.Mon == true)
                    {
                        calcDays++;
                    }

                    else if (date.DayOfWeek == DayOfWeek.Tuesday && employee.Tue == true)
                    {
                        calcDays++;
                    }

                    else if (date.DayOfWeek == DayOfWeek.Wednesday && employee.Wed == true)
                    {
                        calcDays++;
                    }

                    else if (date.DayOfWeek == DayOfWeek.Thursday && employee.Thu == true)
                    {
                        calcDays++;
                    }

                    else if (date.DayOfWeek == DayOfWeek.Friday && employee.Fri == true)
                    {
                        calcDays++;
                    }

                    else if (date.DayOfWeek == DayOfWeek.Saturday && employee.Sat == true)
                    {
                        calcDays++;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Sunday && employee.Sun == true)
                    {
                        calcDays++;
                    }
                }
            }
            return calcDays;
        }

        // HttpPost method to cancel a request submitted by an employee
        [HttpPost]
        public async Task<IActionResult> CancelRequest(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            int currentUserId = _context.Employees.Where(x => x.Email.ToLower() == HttpContext.User.Identity.Name.ToLower()).Select(x => x.Id).FirstOrDefault();


            Request requestPending = _context.Requests.Where(r => r.Id == Id && r.EmployeeId == currentUserId).FirstOrDefault();

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

    // Data Transfer Object to retrieve data from Request data set 
    // and Allowance data set
    public class HomeViewModel
    {
        // list of requests
        public List<Request> Requests { get; set; }

        
        public AllowanceBalance Balance { get; set; }
    }

    // Data Transfer Object used to calculate employee allowance balance
    // Taking data from allowance data sets
    public class AllowanceBalance
    {
        public decimal StandardAllowance { get; set; }

        public double Pending { get; set; }

        public double Taken { get; set; }

        public double Approved { get; set; }

        public decimal Remaining { get { return StandardAllowance - (decimal)(Pending + Taken + Approved); } }
    }

}