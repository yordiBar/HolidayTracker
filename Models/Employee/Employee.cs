using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.Employee
{
    public class Employee
    {
        public int Id { get; set; }
        public string LocationId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime StartDate { get; set; }
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
        public int ApproverId { get; set; }
    }


    public class EmployeeView : Employee
    {
        public bool IsApprover { get; set; }
        public bool IsManager { get; set; }

        public bool IsAdmin { get; set; }
    }
}
