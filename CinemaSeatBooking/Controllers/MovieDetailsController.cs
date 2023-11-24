using Microsoft.AspNetCore.Mvc;
using CinemaSeatBooking.Models;
using Newtonsoft.Json;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace CinemaSeatBooking.Controllers
{
    public class MovieDetailsController : Controller
    {
        [HttpPost]
        public IActionResult Details(string movieCode, string companyName, string overview, string posterUrl, string movieName)
        {
            // Your logic to handle the movie details
            // You can use the received parameters to fetch additional data or perform any necessary operations.

            // Redirect to the details page with the received parameters
            return RedirectToAction("DetailsViewPage", new
            {
                movieCode = movieCode,
                companyName = companyName,
                overview = overview,
                posterUrl = posterUrl,
                movieName = movieName   
            });
        }

        public IActionResult DetailsViewPage(string movieCode, string companyName, string overview, string posterUrl, string movieName)
        {
            // Your logic to fetch additional data or process the received values
            var model = new MovieModel
            {
                MovieCode = movieCode,
                CompanyName = companyName,
                Overview = overview,
                PosterUrl = posterUrl,
                MovieName = movieName
            };

            return View(model);
        }


    }
}
