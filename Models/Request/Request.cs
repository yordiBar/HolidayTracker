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
        public int EmployeeId { get; set; }
        public int RequestCreatedByEmployeeId { get; set; }

        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime From { get; set; }

        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime To { get; set; }
        public int Status { get; set; }
        public double RequestAmount { get; set; }
        [Display(Name ="Comments")]
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

    //public static class Extensions
    //{
    //    public static string GetDescription(this Enum e)
    //    {
    //        var attribute =
    //            e.GetType()
    //                .GetTypeInfo()
    //                .GetMember(e.ToString())
    //                .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
    //                .GetCustomAttributes(typeof(DescriptionAttribute), false)
    //                .SingleOrDefault()
    //                as DescriptionAttribute;

    //        return attribute?.Description ?? e.ToString();
    //    }
    //}

    //    public static string GetDescription<T>(T e) where T : IConvertible
    //    {
    //        if (e is Enum)
    //        {
    //            Type type = e.GetType();
    //            Array values = System.Enum.GetValues(type);

    //            foreach (int val in values)
    //            {
    //                if (val == e.ToInt32(CultureInfo.InvariantCulture))
    //                {
    //                    var memInfo = type.GetMember(type.GetEnumName(val));
    //                    var descriptionAttribute = memInfo[0]
    //                        .GetCustomAttributes(typeof(DescriptionAttribute), false)
    //                        .FirstOrDefault() as DescriptionAttribute;

    //                    if (descriptionAttribute != null)
    //                    {
    //                        return descriptionAttribute.Description;
    //                    }
    //                }
    //            }
    //        }

    //        return null; // could also return string.Empty
    //    }
    //    return description;
    //}

    //public static string GetEnumDescription(Enum value)
    //{
    //    FieldInfo fi = value.GetType().GetField(value.ToString());

    //    DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

    //    if (attributes != null && attributes.Any())
    //    {
    //        return attributes.First().Description;
    //    }

    //    return value.ToString();
    //}


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
