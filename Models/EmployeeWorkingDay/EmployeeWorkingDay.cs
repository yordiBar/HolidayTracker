using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.EmployeeWorkingDay
{
    public class EmployeeWorkingDay
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string CompanyId { get; set; }
        
        // should be bool?
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }

    }
}
