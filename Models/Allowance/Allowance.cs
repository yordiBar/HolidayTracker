using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.Allowance
{
    public class Allowance
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Amount { get; set; }
        public int CarryOver { get; set; }
        public int EmployeeId { get; set; }
        public string CompanyId { get; set; }
    }
}
