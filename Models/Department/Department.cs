﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTracker.Models.Department
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyId { get; set; }
        public string DepartmentCode { get; set; }
    }
}
