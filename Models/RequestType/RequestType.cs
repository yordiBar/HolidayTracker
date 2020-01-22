using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.RequestType
{
    public class RequestType
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string RequestTypeCode { get; set; }
        public string RequestTypeName { get; set; }
        public string Description { get; set; }
    }
}
