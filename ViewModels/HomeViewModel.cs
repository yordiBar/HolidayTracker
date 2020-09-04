using HolidayTracker.Models.DTO;
using HolidayTracker.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.ViewModels
{
    // Data Transfer Object to retrieve data from Request data set 
    // and Allowance data set
    public class HomeViewModel
    {
        // list of requests
        public List<Request> Requests { get; set; }


        public AllowanceBalanceDTO Balance { get; set; }
    }
}
