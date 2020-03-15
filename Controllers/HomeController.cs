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

        [HttpGet]
        public IActionResult Dashboard()
        {
            //int currentUsersCompanyId = 1;
            //int currentUserId = 1;

            
            HomeViewModel viewModel = new HomeViewModel();
            viewModel.Requests = _context.Requests.Where(x => x.EmployeeId==1).ToList();

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }                

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestType(string query)
        {

            int currentUsersCompanyId = 1;

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
        public async Task<IActionResult> CreateRequest(CreateRequestDTO data)
        {

            //save data to database
            //bool saveSuccess = true;

            // take data from CreateRequestDTO
            // insert it into new request
            // save request to db
            int currentUsersCompanyId = 1;
            int currentUserId = 1;

            Request request = new Request();

            request.CompanyId = currentUsersCompanyId;
            request.RequestTypeId = data.RequestTypeId;
            request.EmployeeId = currentUserId;
            request.RequestCreatedByEmployeeId = currentUserId;
            request.From = data.RealFrom;
            request.To = data.RealTo;
            request.Status = (int)RequestStatus.Pending;
            request.Description = data.Description;

            _context.Requests.Add(request);

            try
            {
                await _context.SaveChangesAsync();

                if (request.Id <= 0)
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
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed"); ;
            }   
        }
        
        //Global Errors ASP.net MVC --global.cs file method Error
    }

    public class HomeViewModel
    {
        // list of requests
        public List<Request> Requests { get; set; }
    }
}
