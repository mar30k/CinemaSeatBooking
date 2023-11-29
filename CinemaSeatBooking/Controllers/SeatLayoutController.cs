using CinemaSeatBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
namespace CinemaSeatBooking.Controllers
{
    public class SeatLayoutController : Controller
    {
        private readonly HttpClient _httpClient;
        public SeatLayoutController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api/");
        }
        [HttpPost]
        public IActionResult SeatArrangement(string spacecode, string companyTinNumber)
        {
            return RedirectToAction("SeatArrangementView", new
            {
                spacecode = spacecode,
                companyTinNumber = companyTinNumber,
            });
        }

        public async Task<IActionResult> SeatArrangementView(string spacecode, string companyTinNumber)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"cinema/getCinemaSeatArrangment?orgTin={companyTinNumber}&spaceCode={spacecode}");

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                // Deserialize into a single SeatLayout instance
                SeatLayout seatArrangement = JsonConvert.DeserializeObject<SeatLayout>(responseData);

                // Pass the single SeatLayout instance to the view
                return View(seatArrangement);
            }

            // Handle the case when the response is not successful
            return View("Error"); // You can create an "Error.cshtml" view for error handling
        }

    }
}
