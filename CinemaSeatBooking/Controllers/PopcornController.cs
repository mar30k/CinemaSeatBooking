using Microsoft.AspNetCore.Mvc;

namespace CinemaSeatBooking.Controllers
{
    public class PopcornController : Controller
    {
        public IActionResult PopcornView()
        {
            return View();
        }
    }
}
