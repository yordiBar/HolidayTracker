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
        private readonly HolidayTracker.Data.ApplicationDbContext _context;//private readonly HomeViewModel _viewModel;

        public HomeController(ILogger<HomeController> logger, HolidayTracker.Data.ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            //_viewModel = viewModel;
        }

        //public async Task<IActionResult> Index(string sortOrder,
        //    string currentFilter, string searchString, int? pageIndex)
        //{
        //    HolidayTracker.Controllers.HomeViewModel pageData = new Controllers.HomeViewModel(_context);
        //    //var user = new ApplicationUser { CompanyId = model.CompanyId };
        //    int currentUsersCompanyId = 1;
        //    pageData.CurrentSort = sortOrder;
        //    pageData.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        //    pageData.CodeSort = sortOrder == "Code" ? "code_desc" : "Code";
        //    if (searchString != null)
        //    {
        //        pageIndex = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }

        //    pageData.CurrentFilter = searchString;

        //    IQueryable<RequestType> dbdata = _context.RequestTypes.Where(x => x.CompanyId == currentUsersCompanyId);
        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        dbdata = dbdata.Where(s => s.RequestTypeName.Contains(searchString));
        //    }
        //    switch (sortOrder)
        //    {
        //        case "name_desc":
        //            dbdata = dbdata.OrderByDescending(s => s.RequestTypeName);
        //            break;
        //        case "Code":
        //            dbdata = dbdata.OrderBy(s => s.RequestTypeCode);
        //            break;
        //        case "code_desc":
        //            dbdata = dbdata.OrderByDescending(s => s.RequestTypeCode);
        //            break;
        //        default:
        //            dbdata = dbdata.OrderBy(s => s.RequestTypeName);
        //            break;
        //    }

        //    int pageSize = 10;
        //    pageData.RequestType = await PaginatedList<RequestType>.CreateAsync(
        //        dbdata.AsNoTracking(), pageIndex ?? 1, pageSize);

        //    return View(pageData);
        //}

        //public IActionResult Dashboard(HomeViewModel data)
        //{
        //    int currentUsersCompanyId = 0;
        //    //int currentUserId = 1;

        //    var viewModel = new List<Request>();

        //    var request = new HomeViewModel();
        //    //request.CompanyId = currentUsersCompanyId;
        //    request.RequestTypeId = data.RequestTypeId;
        //    request.From = data.From;
        //    request.To = data.To;
        //    request.Status = data.Status;
        //    request.Description = data.Description;

        //    return View(viewModel);
        //}
        [HttpGet]
        public IActionResult Dashboard()
        {
            int currentUsersCompanyId = 0;
            int currentUserId = 1;


            HomeViewModel viewModel = new HomeViewModel();

            //viewModel.CompanyId = currentUsersCompanyId;
            //viewModel.RequestTypeId = viewModel.RequestTypeId;
            //viewModel.EmployeeId = currentUserId;
            //viewModel.RequestCreatedByEmployeeId = currentUserId;
            //viewModel.From = viewModel.From;
            //viewModel.To = viewModel.To;
            //viewModel.Status = (int)RequestStatus.Pending;
            //viewModel.Description = viewModel.Description;

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

            //    public int CompanyId { get; set; } value in currentUsersCompanyId
            //    public int RequestTypeId { get; set; } value from CreateRequestDTO data
            //    public int EmployeeId { get; set; } value from currentUserId
            //    public int RequestCreatedByEmployeeId { get; set; } value from currentUserId
            //    public string From { get; set; }  value from CreateRequestDTO data RealFrom
            //    public string To { get; set; } value from CreateRequestDTO data RealTo
            //    public string Status { get; set; } value from static list default to pending when creating
            //    public double RequestAmount { get; set; } calculate for the employee using there working days and the from - to date

            //    //add description
            

            
        }

        

        //Global Errors ASP.net MVC --global.cs file method Error
    }

    public class HomeViewModel
    {
        // list of requests
        public List<Request> Requests { get; set; }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int RequestTypeId { get; set; }
        public int EmployeeId { get; set; }
        public int RequestCreatedByEmployeeId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Status { get; set; }
        public double RequestAmount { get; set; }
        public string Description { get; set; }
    }
}
