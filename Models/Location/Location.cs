﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.Location
{
    public class Location
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
    }
}