using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HolidayTracker.Extensions;
using HolidayTracker.Models.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Controllers
{
    //[Authorize(Roles = "SystemAdmin,Admin,Employee,Manager,Approver")]
    public class EmployeeController : Controller
    {
        private readonly HolidayTracker.Data.ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public EmployeeController(HolidayTracker.Data.ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            
            
            
            ApplicationUser newuser = await _userManager.FindByEmailAsync(employee.Email);

            employee.IsAdmin = await _userManager.IsInRoleAsync(newuser, "Admin");

            employee.IsApprover = await _userManager.IsInRoleAsync(newuser, "Approver");

            employee.IsManager = await _userManager.IsInRoleAsync(newuser, "Manager");
                       
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee emp)
        {
            if (!ModelState.IsValid)
            {
                return View(emp);
            }

            int currentUserCompanyId = 1;
            
            emp.CompanyId = currentUserCompanyId;
            
            _context.Attach(emp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                var user = new ApplicationUser { UserName = emp.Email, Email = emp.Email, CompanyId = emp.CompanyId };
                
                string password = Guid.NewGuid().ToString();

                var result = await _userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    if (emp.IsAdmin)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }

                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Admin");
                        }
                    }

                    if (emp.IsApprover)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Approver"))
                        {
                            await _userManager.AddToRoleAsync(user, "Approver");
                        }

                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Approver"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Approver");
                        }
                    }

                    if (emp.IsManager)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            await _userManager.AddToRoleAsync(user, "Manager");
                        }

                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Manager");
                        }
                    }

                    if (!await _userManager.IsInRoleAsync(user, "Employee"))
                    {
                        await _userManager.AddToRoleAsync(user, "Employee");
                    }
                }

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
            return  View(new Employee()); 
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee emp)
        {
            if (!ModelState.IsValid)
            {
                return View(emp);
            }

            int currentUsersCompanyId = 1;
            
            emp.CompanyId = currentUsersCompanyId;
            
            _context.Employees.Add(emp);

            try
            {
                await _context.SaveChangesAsync();

                var user = new ApplicationUser { UserName = emp.Email, Email = emp.Email, CompanyId = emp.CompanyId };
                
                string password = Guid.NewGuid().ToString();

                var result = await _userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    if (emp.IsAdmin)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }

                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Admin");
                        }
                    }
                
                    if (emp.IsApprover)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Approver"))
                        {
                            await _userManager.AddToRoleAsync(user, "Approver");
                        }

                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Approver"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Approver");
                        }
                    }

                    if (emp.IsManager)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            await _userManager.AddToRoleAsync(user, "Manager");
                        }

                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Manager");
                        }
                    }

                    if (!await _userManager.IsInRoleAsync(user, "Employee"))
                    {
                        await _userManager.AddToRoleAsync(user, "Employee");
                    }                                                                                

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(emp.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                }

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