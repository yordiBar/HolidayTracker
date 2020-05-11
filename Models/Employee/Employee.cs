using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

// Employee Model
namespace HolidayTracker.Models.Employee
{
    public class Employee
    {
        // Properties
        public int Id { get; set; }
        public string LocationId { get; set; }
        public int DepartmentId { get; set; }

        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime LeavingDate { get; set; }
        public int EmployeeTypeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string JobTitle { get; set; }
        public int GenderId { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        // Password property to store Employee password, not migrated to the database
        [NotMapped]
        public string Password { get; set; }
        public int ApproverId { get; set; }
        public bool IsApprover { get; set; }
        public bool IsManager { get; set; }
        public bool IsAdmin { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        
        // Properties for diplay purposes, not migrated to the database
        [NotMapped]
        public string DisplayStartDate { get { return this.StartDate.ToString("dd/MM/yyyy"); } }
        
        [NotMapped]
        public string DisplayLeavingDate { get { return this.LeavingDate.ToString("dd/MM/yyyy"); } }

    }



}