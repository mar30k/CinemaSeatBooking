using Microsoft.AspNetCore.Mvc;

namespace CinemaSeatBooking.Controllers
{
    public class BurgersController : Controller
    {
        public IActionResult BurgerView()
        {
            return View();
        }
    }
}
