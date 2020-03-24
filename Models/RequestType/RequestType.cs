using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HolidayTracker.Models.RequestType
{
    public class RequestType
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string RequestTypeCode { get; set; }
        public string RequestTypeName { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool TakesFromAllowance { get; set; }

        public virtual List<HolidayTracker.Models.Request.Request> Requests { get; set; }
    }
}
