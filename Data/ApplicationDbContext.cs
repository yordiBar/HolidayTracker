using System;
using System.Collections.Generic;
using System.Text;
using HolidayTracker.Extensions;
using HolidayTracker.Models.Allowance;
using HolidayTracker.Models.Company;
using HolidayTracker.Models.Department;
using HolidayTracker.Models.Employee;
using HolidayTracker.Models.Gender;
using HolidayTracker.Models.Location;
using HolidayTracker.Models.Request;
using HolidayTracker.Models.RequestType;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HolidayTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }        
        public DbSet<Location> Locations { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<Allowance> Allowances { get; set; }
        public IEnumerable<object> Department { get; internal set; }
    }
}
