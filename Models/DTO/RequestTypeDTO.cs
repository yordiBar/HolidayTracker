namespace HolidayTracker.Models.DTO
{
    public class RequestTypeDTO
    {
        public int Id { get; set; }
        public string RequestTypeName { get; set; }
        public string RequestTypeCode { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
    }

}
