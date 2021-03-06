﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Location Model
namespace HolidayTracker.Models.Location
{
    public class Location
    {
        // Properties
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
