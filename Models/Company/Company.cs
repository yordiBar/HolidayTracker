﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Company Model
namespace HolidayTracker.Models.Company
{    
    public class Company
    {
        // Properties
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public byte[] CompanyLogo { get; set; }

    }
}
