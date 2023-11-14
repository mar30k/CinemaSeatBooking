using CinemaSeatBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CinemaSeatBookingSystem.Controllers
{
    public class SeatController : Controller
    {
        private  List<Seat> seats = new()
        {
            new Seat {id = "seat 1-1", SeatNumber = 1, IsBooked = false, IsSold = false, Price = 10.00M, IsHall=true,},
            new Seat {id = "seat 1-2", SeatNumber = 2, IsBooked = false, IsSold = false, Price = 12.50M, IsHall=true, },
            new Seat {id = "seat 1-3", SeatNumber = 3, IsBooked = false, IsSold = false, Price = 8.00M, IsHall=true, },
            new Seat {id = "seat 1-4", SeatNumber = 4, IsBooked = false, IsSold = false, Price = 15.00M, IsHall = true},
            new Seat {id = "seat 1-5", SeatNumber = 5, IsBooked = false, IsSold = true, Price = 10.00M, IsHall = true},
            new Seat {id = "seat 1-6", SeatNumber = 6, IsBooked = false, IsSold = false, Price = 12.50M, IsHall = true},
            new Seat {id = "seat 1-6", SeatNumber = 7, IsBooked = false, IsSold = false, Price = 8.00M, IsHall = true},
            new Seat {id = "seat 2-1", SeatNumber = 8, IsBooked = false, IsSold = false, Price = 15.00M, IsHall = true},
            new Seat {id = "seat 2-2", SeatNumber = 9, IsBooked = false, IsSold = true, Price = 10.00M, IsHall = true},
            new Seat {id = "seat 2-3", SeatNumber = 10, IsBooked = false, IsSold = false, Price = 12.50M, IsHall = true},
            new Seat {id = "seat 2-4", SeatNumber = 11, IsBooked = false, IsSold = false, Price = 8.00M, IsHall = true},
            new Seat {id = "seat 2-5", SeatNumber = 12, IsBooked = false, IsSold = false, Price = 15.00M, IsHall=true, },
            new Seat {id = "seat 2-6", SeatNumber = 13, IsBooked = false, IsSold = true, Price = 10.00M, IsHall = true},
            new Seat {id = "seat 3-1", SeatNumber = 14, IsBooked = false, IsSold = false, Price = 12.50M, IsHall = true},
            new Seat {id = "seat 3-2", SeatNumber = 15, IsBooked = false, IsSold = false, Price = 8.00M, IsHall = true},
            new Seat {id = "seat 3-3", SeatNumber = 16, IsBooked = false, IsSold = false, Price = 15.00M, IsHall = true},
            new Seat {id = "seat 3-4", SeatNumber = 17, IsBooked = false, IsSold = false, Price = 10.00M, IsHall = true},
            new Seat {id = "seat 3-5", SeatNumber = 18, IsBooked = false, IsSold = false, Price = 12.50M, IsHall = true},
            new Seat {id = "seat 3-6", SeatNumber = 19, IsBooked = false, IsSold = false, Price = 8.00M, IsHall = true},
            new Seat {id = "seat 4-1", SeatNumber = 20, IsBooked = false, IsSold = false, Price = 15.00M,IsHall = true},
            new Seat {id = "seat 4-2", SeatNumber = 21, IsBooked = false, IsSold = false, Price = 10.00M, IsHall = true},
            new Seat {id = "seat 4-3", SeatNumber = 22, IsBooked = false, IsSold = false, Price = 15.00M, IsHall = true},
            new Seat {id = "seat 4-4", SeatNumber = 23, IsBooked = false, IsSold = true, Price = 10.00M, IsHall = true},
            new Seat {id = "seat 4-5", SeatNumber = 24, IsBooked = false, IsSold = false, Price = 15.00M, IsHall = true},
            new Seat {id = "seat 4-6", SeatNumber = 25, IsBooked = false, IsSold = true, Price = 10.00M, IsHall = true},
        };

        public ActionResult SeatView()
        {
            
            return View(seats);
        }

        public IActionResult BiilPartial()
        {
            return PartialView("_Bill");
        }    
    }
}