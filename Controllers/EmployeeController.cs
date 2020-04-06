using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HolidayTracker.Areas.Identity.Data;
using HolidayTracker.Areas.Identity.Extensions;
using HolidayTracker.Models.Allowance;
using HolidayTracker.Models.Department;
using HolidayTracker.Models.Employee;
using HolidayTracker.Models.Gender;
using HolidayTracker.Models.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Controllers
{
    //[Authorize(Roles = "Admin")]
    //[Authorize(Roles = "Manager")]
    //[Authorize(Roles = "SystemAdmin")]
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
            int currentUsersCompanyId = User.Identity.GetCompanyId();
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

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

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

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            emp.CompanyId = currentUsersCompanyId;
            
            _context.Attach(emp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                ApplicationUser user = await _userManager.FindByEmailAsync(emp.Email);

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
                //}

                // check if there is an allowance for the current year for the employee if not create
                await CreateAllowanceIfRequired(emp, currentUsersCompanyId);
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

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

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
            // check if validation attibutes are met if not return error to screen
            if (!ModelState.IsValid)
            {
                return View(emp);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            emp.CompanyId = currentUsersCompanyId;
            
            _context.Employees.Add(emp);

            try
            {
                await _context.SaveChangesAsync();

                var user = new ApplicationUser { UserName = emp.Email, Email = emp.Email, CompanyId = emp.CompanyId };

                string password = emp.Password;

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


                //check if there is an allowance for the current year for the employee if not create

                await CreateAllowanceIfRequired(emp, currentUsersCompanyId);

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

            int currentUsersCompanyId = User.Identity.GetCompanyId();//User.Identity.GetCompanyId();

            Employee employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        
        //check if there is an allowance for the current year for the employee if not create
        private async Task CreateAllowanceIfRequired(Employee emp, int currentUsersCompanyId)
        {
            if (_context.Allowances.Where(x => x.EmployeeId == emp.Id && x.CompanyId == currentUsersCompanyId).Count() == 0)
            {
                _context.Allowances.Add(new Allowance
                {
                    From = new DateTime(2020, 1, 1),
                    To = new DateTime(2020, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                _context.Allowances.Add(new Allowance
                {
                    From = new DateTime(2021, 1, 1),
                    To = new DateTime(2021, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                _context.Allowances.Add(new Allowance
                {
                    From = new DateTime(2022, 1, 1),
                    To = new DateTime(2022, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                await _context.SaveChangesAsync();
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentName(string name)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            List<Department> departmentNameList = _context.Departments.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false).ToList();
            List<Department> departmentNameResults = new List<Department>();
            foreach (var deptName in departmentNameList)
            {
                if (String.IsNullOrEmpty(name) || deptName.DepartmentName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    departmentNameResults.Add(deptName);
                }
            }

            departmentNameResults.Sort(delegate (Department d1, Department d2) { return d1.DepartmentName.CompareTo(d2.DepartmentName); });

            var serialisedJson = from result in departmentNameResults
                                 select new
                                 {
                                     text = result.DepartmentName,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationName(string name)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            List<Location> locationNameList = _context.Locations.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false).ToList();
            List<Location> locationNameResults = new List<Location>();
            foreach (var locName in locationNameList)
            {
                if (String.IsNullOrEmpty(name) || locName.LocationName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    locationNameResults.Add(locName);
                }
            }

            locationNameResults.Sort(delegate (Location l1, Location l2) { return l1.LocationName.CompareTo(l2.LocationName); });

            var serialisedJson = from result in locationNameResults
                                 select new
                                 {
                                     text = result.LocationName,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }

        [HttpGet]
        public async Task<IActionResult> GetGenderName(string name)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            List<Gender> genderNameList = _context.Genders.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false).ToList();
            List<Gender> genderNameResults = new List<Gender>();
            foreach (var genName in genderNameList)
            {
                if (String.IsNullOrEmpty(name) || genName.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    genderNameResults.Add(genName);
                }
            }

            genderNameResults.Sort(delegate (Gender g1, Gender g2) { return g1.Name.CompareTo(g2.Name); });

            var serialisedJson = from result in genderNameResults
                                 select new
                                 {
                                     text = result.Name,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }

        [HttpGet]
        public async Task<IActionResult> GetApproverName(string name)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            List<Employee> approverNameList = _context.Employees.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false).ToList();
            List<Employee> approverNameResults = new List<Employee>();
            foreach (var appName in approverNameList)
            {
                if (String.IsNullOrEmpty(name) || appName.DisplayName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    approverNameResults.Add(appName);
                }
            }

            approverNameResults.Sort(delegate (Employee a1, Employee a2) { return a1.DisplayName.CompareTo(a2.DisplayName); });

            var serialisedJson = from result in approverNameResults
                                 select new
                                 {
                                     text = result.DisplayName,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentById(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Department department = _context.Departments.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false && x.Id == Id).FirstOrDefault();
            
            if (department != null)
            {
                var serialisedJson = new
                                     {
                                         text = department.DepartmentName,
                                         id = department.Id
                                     };
                return Json(serialisedJson);
            }
            else
            {
                var serialisedJson = new
                {
                    text = "",
                    id = 0
                };
                return Json(serialisedJson);
            }
            
            
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationById(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Location location = _context.Locations.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false && x.Id == Id).FirstOrDefault();

            if (location != null)
            {
                var serialisedJson = new
                {
                    text = location.LocationName,
                    id = location.Id
                };
                return Json(serialisedJson);
            }
            else
            {
                var serialisedJson = new
                {
                    text = "",
                    id = 0
                };
                return Json(serialisedJson);
            }


        }

        [HttpGet]
        public async Task<IActionResult> GetGenderById(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Gender gender = _context.Genders.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false && x.Id == Id).FirstOrDefault();

            if (gender != null)
            {
                var serialisedJson = new
                {
                    text = gender.Name,
                    id = gender.Id
                };
                return Json(serialisedJson);
            }
            else
            {
                var serialisedJson = new
                {
                    text = "",
                    id = 0
                };
                return Json(serialisedJson);
            }


        }

        [HttpGet]
        public async Task<IActionResult> GetApproverById(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Employee approver = _context.Employees.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false && x.Id == Id).FirstOrDefault();

            if (approver != null)
            {
                var serialisedJson = new
                {
                    text = approver.DisplayName,
                    id = approver.Id
                };
                return Json(serialisedJson);
            }
            else
            {
                var serialisedJson = new
                {
                    text = "",
                    id = 0
                };
                return Json(serialisedJson);
            }


        }
    }
}