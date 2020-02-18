﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTracker.Extensions;
using HolidayTracker.Models.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Controllers
{
    //[Authorize(Roles = "SystemAdmin,Admin,Employee,Manager,Approver")]
    public class EmployeeController : Controller
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        public EmployeeController(HolidayTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            HolidayTracker.Views.Employees.IndexModel pageData = new Views.Employees.IndexModel(_context);
            //var user = new ApplicationUser { CompanyId = model.CompanyId };
            int currentUsersCompanyId = 1;
            pageData.CurrentSort = sortOrder;
            pageData.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            pageData.DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            pageData.CurrentFilter = searchString;

            IQueryable<Employee> employeeIQ = _context.Employees.Where(x => x.CompanyId == currentUsersCompanyId);
            if (!String.IsNullOrEmpty(searchString))
            {
                employeeIQ = employeeIQ.Where(s => s.DisplayName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    employeeIQ = employeeIQ.OrderByDescending(s => s.DisplayName);
                    break;
                case "Date":
                    employeeIQ = employeeIQ.OrderBy(s => s.StartDate);
                    break;
                case "date_desc":
                    employeeIQ = employeeIQ.OrderByDescending(s => s.StartDate);
                    break;
                default:
                    employeeIQ = employeeIQ.OrderBy(s => s.DisplayName);
                    break;
            }

            int pageSize = 10;
            pageData.Employee = await PaginatedList<Employee>.CreateAsync(
                employeeIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            return View(pageData);
        }

        [HttpGet]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = 1;//User.Identity.GetCompanyId();
            Employee employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false); //FirstOrDefaultAsync(m => m.Id == id );
            EmployeeView returndata = (EmployeeView)employee;

            //set roles in returndata with result from actual user role
            //returndata.IsApprover = User.IsInRole("Approver")

            if (employee == null)
            {
                return NotFound();
            }
            return View(returndata);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeView emp)
        {
            if (!ModelState.IsValid)
            {
                return View(emp);
            }

            _context.Attach(emp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();


                // set the roles the emplyoee has
                //like what is done when you create a company and set the roles for the user that created company
                //everyone should have an employee
                //check for each role 
                //if is role checked add approval role
                //else check if user has role remove role from user

                //also check if there is an allowance for the current year for the employee if not create
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(emp.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = 1;//User.Identity.GetCompanyId();
            Employee employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId); //FirstOrDefaultAsync(m => m.Id == id );

            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Employee emp)
        {
            if (!ModelState.IsValid)
            {
                return View(emp);
            }

            emp.IsDeleted = true;
            _context.Attach(emp).State = EntityState.Modified;

            //_context.Employees.Remove(emp);// kee

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(emp.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return  View(new EmployeeView()); 
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeView emp)
        {
            if (!ModelState.IsValid)
            {
                return View(emp);
            }

            _context.Employees.Add(emp);

            try
            {
                await _context.SaveChangesAsync();


                // set the roles the emplyoee has
                //like what is done when you create a company and set the roles for the user that created company
                //everyone should have an employee
                //check for each role 
                //if is role checked add approval role
                //else check if user has role remove role from user

                //also check if there is an allowance for the current year for the employee if not create

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(emp.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }
                                        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = 1;//User.Identity.GetCompanyId();

            Employee employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
    }
}