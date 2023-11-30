using CinemaSeatBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace CinemaSeatBooking.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

public class SeatLayoutController : Controller
{
    private readonly HttpClient _httpClient;
    public SeatLayoutController()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api/")
        };
    }
    [HttpPost]
    public IActionResult SeatArrangement(string spacecode, string companyTinNumber, string code)
    {
        return RedirectToAction("SeatArrangementView", new
        {
            spacecode,
            companyTinNumber,
            code,
        });
    }

    [HttpGet]
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

            HttpResponseMessage soldseatsResponse = await _httpClient.GetAsync($"cinema/GetSoldSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");
            if (soldseatsResponse.IsSuccessStatusCode)
            {
                string soldSeatData = await soldseatsResponse.Content.ReadAsStringAsync();

                List<string> soldSeats = JsonConvert.DeserializeObject<List<string>>(soldSeatData);

                seatArrangement.SoldSeats ??= soldSeats;
            }
            else
            {
                //handle the case where GetSoldSeats api fails 
                return View("Error");
            }

            HttpResponseMessage availableSeatsResponse = await _httpClient.GetAsync($"cinema/GetAvailableSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");
            if (availableSeatsResponse.IsSuccessStatusCode)
            {
                string availableSeatsData = await availableSeatsResponse.Content.ReadAsStringAsync();
                List<string> availableSeats = JsonConvert.DeserializeObject<List<string>>(availableSeatsData);

                seatArrangement.AvailableSeats ??= availableSeats;

                var anonymousObject = new
                {
                    schedule = code,
                    allSeats = seatArrangement.AvailableSeats,
                    orgTin = companyTinNumber
                };

                string jsonBody = JsonConvert.SerializeObject(anonymousObject);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage takenSeatsResponse = await _httpClient.PostAsync("ManageSeatCache/GetCachedSeatsForSchedule", content);
                if (takenSeatsResponse.IsSuccessStatusCode)
                {
                    string takenSeatsResponseData = await takenSeatsResponse.Content.ReadAsStringAsync();
                    List<string> takenSeats = JsonConvert.DeserializeObject<List<string>>(takenSeatsResponseData);

                    seatArrangement.TakenSeats ??= takenSeats;
                }
                else
                {
                    return View("Error");
                }
            }

            // Pass the updated SeatLayout instance to the view
            return View(seatArrangement);
        }

        // Handle the case when the first API call fails
        return View("Error");
    }

}
