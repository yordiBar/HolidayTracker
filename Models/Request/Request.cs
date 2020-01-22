using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.Request
{
    public class Request
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string RequestTypeId { get; set; }
        public string EmployeeId { get; set; }
        public string RequestCreatedByEmployeeId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Status { get; set; }
        public string RequestAmount { get; set; }
    }
}
