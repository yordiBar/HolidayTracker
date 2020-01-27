using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HolidayTracker.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}