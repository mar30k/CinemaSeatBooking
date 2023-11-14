using Microsoft.AspNetCore.Mvc;

namespace CinemaSeatBooking.Controllers
{
    public class BeverageController : Controller
    {
        public IActionResult BeverageView()
        {
            return View();
        }
    }
}
