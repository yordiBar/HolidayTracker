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

namespace HolidayTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HolidayTracker.Data.ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, HolidayTracker.Data.ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        //public ActionResult MyRequestViewModel()
        //{
        //    Data _requestData = new Data();
        //    MyRequestsViewModel viewModel = new MyRequestsViewModel();
        //    viewModel.AllRequests = _requestData.GetAllRequest();
        //    return View(viewModel);
        //}
                

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestType(string query)
        {

            int currentUsersCompanyId = 0;

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

        [HttpPost]
        public IActionResult CreateRequest(CreateRequestDTO data)
        {

            //save data to database
            bool saveSuccess = true;

            // take data from CreateRequestDTO
            // insert it into new request
            // save request to db


            //public class Request
            //{

            //    public int CompanyId { get; set; } value in currentUsersCompanyId
            //    public int RequestTypeId { get; set; } value from CreateRequestDTO data
            //    public int EmployeeId { get; set; } value from currentUserId
            //    public int RequestCreatedByEmployeeId { get; set; } value from currentUserId
            //    public string From { get; set; }  value from CreateRequestDTO data RealFrom
            //    public string To { get; set; } value from CreateRequestDTO data RealTo
            //    public string Status { get; set; } value from static list default to pending when creating
            //    public double RequestAmount { get; set; } calculate for the employee using there working days and the from - to date

            //    //add description
            

            if (!saveSuccess)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed");
            }
            else
            {
                //  When I want to return sucess:
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json("Saved!");
            }
        }

        //Global Errors ASP.net MVC --global.cs file method Error
    }
}
