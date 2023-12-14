namespace CinemaSeatBooking.Models
{
    public class ProductsViewModel
    {
        public List<Product>? Products { get; set; }
        public string? MovieScheduleCode { get; set; }
        public string? CompanyTinNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? SpaceType { get; set; }
        public string? MovieName { get; set; }
        public string? ScheduleDate { get; set; }
        public string? ScheduleTime { get; set; }
        public string? HallName { get; set; }
        public string? Dimension { get; set; }
        public decimal Price { get; set; }
    }

}
