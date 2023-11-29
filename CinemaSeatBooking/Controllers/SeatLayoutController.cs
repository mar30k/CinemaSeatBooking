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
        public IActionResult SeatArrangement(string spacecode, string companyTinNumber, string code)
        {
            return RedirectToAction("SeatArrangementView", new
            {
                spacecode = spacecode,
                companyTinNumber = companyTinNumber,
                code = code,
            });
        }

        public async Task<IActionResult> SeatArrangementView(string spacecode, string companyTinNumber, string code)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"cinema/getCinemaSeatArrangment?orgTin={companyTinNumber}&spaceCode={spacecode}");

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                // Deserialize into a single SeatLayout instance
                SeatLayout seatArrangement = JsonConvert.DeserializeObject<SeatLayout>(responseData);

                // Make a second API call to get booked seats
                HttpResponseMessage bookedSeatsResponse = await _httpClient.GetAsync($"cinema/GetBookedSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");

                if (bookedSeatsResponse.IsSuccessStatusCode)
                {
                    string bookedSeatsData = await bookedSeatsResponse.Content.ReadAsStringAsync();

                    // Deserialize the response into a List<string>
                    List<string> bookedSeats = JsonConvert.DeserializeObject<List<string>>(bookedSeatsData);

                    // Update the SeatLayout model with the booked seats information
                    seatArrangement.BookedSeats ??= bookedSeats;
                }
                else
                {
                    // Handle the case when the second API call fails
                    return View("Error");
                }

                // Pass the updated SeatLayout instance to the view
                return View(seatArrangement);
            }

            // Handle the case when the first API call fails
            return View("Error");
        }

    }
}
