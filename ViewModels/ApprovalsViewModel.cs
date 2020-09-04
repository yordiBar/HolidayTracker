using HolidayTracker.Models.Employee;
using HolidayTracker.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.ViewModels
{
    public class ApprovalsViewModel
    {
        // list of requests
        public List<Request> Requests { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
