using Microsoft.AspNetCore.Mvc;

namespace CinemaSeatBooking.Controllers
{
    public class PopcornController : Controller
    {
        public IActionResult PopcornView()
        {
            return View();
        }
        public IActionResult BiilPartial()
        {
            return PartialView("_Bill");
        }
    }
}
