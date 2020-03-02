using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public DateTime From { get; set; } //change to DateTime
        public DateTime To { get; set; } //change to DateTime
        public int Status { get; set; } //change to int
        public double RequestAmount { get; set; }
        public string Description { get; set; }
    }

    public enum RequestStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Approved")]
        Approved = 1,
        [Description("Taken")]
        Taken = 2,
        [Description("Cancelled")]
        Cancelled = 3,
        [Description("Rejected")]
        Rejected = 4
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
}
