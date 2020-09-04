using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.DTO
{
    public class AllowanceBalanceDTO
    {
        // Data Transfer Object used to calculate employee allowance balance
        // Taking data from allowance data sets
        public decimal StandardAllowance { get; set; }

        public double Pending { get; set; }

        public double Taken { get; set; }

        public double Approved { get; set; }

        public decimal Remaining { get { return StandardAllowance - (decimal)(Pending + Taken + Approved); } }
    }
}
