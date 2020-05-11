using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Department Model
namespace HolidayTracker.Models.Department
{
    public class Department
    {
        // Properties
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public int CompanyId { get; set; }
        public string DepartmentCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
