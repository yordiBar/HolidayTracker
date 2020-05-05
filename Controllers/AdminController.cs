﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Admin Controller

namespace HolidayTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        
        // Action method to display Admin View
        public IActionResult Index()
        {
            return View();
        }

        // Unused method
        public IActionResult Edit()
        {
            return View();
        }

        // Unused method
        public IActionResult Create()
        {
            return View();
        }

        // Unused method
        public IActionResult Details()
        {
            return View();
        }

        // Unused method
        public IActionResult Delete()
        {
            return View();
        }
    }
}