using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HolidayTracker.Models.Request
{
    public class Request
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }
        
        public int RequestTypeId { get; set; }

        public virtual HolidayTracker.Models.RequestType.RequestType RequestType { get; set; }

        public int EmployeeId { get; set; }

        public int RequestCreatedByEmployeeId { get; set; }

        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime From { get; set; }

        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime To { get; set; }

        public int Status { get; set; }

        public string DisplayStatus
        {
            get
            {
                RequestStatus status = (RequestStatus)this.Status;
                return status.ToDescriptionString();
            }
        }

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

    public static class EnumExtensions
    {

        public static IEnumerable<T> GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
        public static string ToDescriptionString<T>(this T val)
        {
            string res = "";
            try
            {
                if (val == null)
                {
                    throw new ArgumentNullException("val");
                }

                var eType = val.GetType();

                DescriptionAttribute[] attributes = (DescriptionAttribute[])eType.GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
                res = attributes.Length > 0 ? attributes[0].Description : string.Empty;
                
            }
            catch
            {
                res = "";
            }
            return res;
        }
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