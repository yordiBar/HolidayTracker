﻿//**************************
// UNUSED CONTROLLER
//**************************
using HolidayTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HolidayTracker.Controllers
{
    //[Authorize(Roles = "SystemAdmin,Manager")]
    public class ManagerController : Controller
    {

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public ManagerController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
            {
                this.roleManager = roleManager;
                this.userManager = userManager;
            }
        public IActionResult Index()
        {
            return View();
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
}