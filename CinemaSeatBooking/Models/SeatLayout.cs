namespace CinemaSeatBooking.Models
{
    public class SeatLayout
    {
        public string SpaceCode { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public List<SeatInfo> Seats { get; set; }
        public string? MaxSeats {  get; set; }  
    }

    public class SeatInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; }
    }


}
