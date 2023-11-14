 namespace CinemaSeatBooking.Models
{
    public class Seat
    {
        public int SeatNumber { get; set; }
        public bool IsBooked { get; set; }
        public bool IsSold { get; set; }
        public decimal Price { get; set; }
        public bool IsHall { get; set; }
        public required string id { get; set; }
    }
}
