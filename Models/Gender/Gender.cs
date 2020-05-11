using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Gender Model
namespace HolidayTracker.Models.Gender
{
    public class Gender
    {
        // Properties
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
