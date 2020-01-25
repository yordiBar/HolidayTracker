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
        public string DepartmentId { get; set; }
        public string StartDate { get; set; }
        public string LeavingDate { get; set; }
        public string EmployeeTypeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string JobTitle { get; set; }
        public string GenderId { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
        public string CompanyId { get; set; }
    }
}
