using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.Request
{
    public class Request
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int RequestTypeId { get; set; }
        public int EmployeeId { get; set; }
        public int RequestCreatedByEmployeeId { get; set; }
        public string From { get; set; } //change to DateTime
        public string To { get; set; } //change to DateTime
        public string Status { get; set; }
        public double RequestAmount { get; set; }

        //add description
    }

    public class CreateRequestDTO
    {
        public int RequestTypeId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Description { get; set; }

        public DateTime RealFrom
        {
            get
            {
                return DateTime.ParseExact(From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public DateTime RealTo
        {
            get
            {
                return DateTime.ParseExact(To, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }
    }

    //public class MyRequestsViewModel
    //{
    //    public List<Request> AllRequests { get; set; }
    //}
}
