using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HolidayTracker.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public ManagerController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
            {
                this.roleManager = roleManager;
                this.userManager = userManager;
            }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}