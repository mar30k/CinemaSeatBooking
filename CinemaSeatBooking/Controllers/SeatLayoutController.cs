using CinemaSeatBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace CinemaSeatBooking.Controllers;
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
    public IActionResult SeatArrangement(string spacecode, string companyTinNumber, string code, decimal price)
    {
        return RedirectToAction("SeatArrangementView", new
        {
            spacecode,
            companyTinNumber,
            code,
            price
        });
    }

    [HttpGet]
    public async Task<IActionResult> SeatArrangementView(string spacecode, string companyTinNumber, string code, decimal price)

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
                var bookedSeats = JsonConvert.DeserializeObject<List<string>>(bookedSeatsData);

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

                var soldSeats = JsonConvert.DeserializeObject<List<string>>(soldSeatData);

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
                var availableSeats = JsonConvert.DeserializeObject<List<string>>(availableSeatsData);

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
                    var takenSeats = JsonConvert.DeserializeObject<List<string>>(takenSeatsResponseData);

                    seatArrangement.TakenSeats ??= takenSeats;
                }
                else
                {
                    return View("Error");
                }
            }

            seatArrangement.CompanyTinNumber = companyTinNumber;
            seatArrangement.SpaceCode = spacecode;
            seatArrangement.MovieScheduleCode = code;
            seatArrangement.Price = price;
            // Pass the updated SeatLayout instance to the view
            return View(seatArrangement);
        }

        // Handle the case when the first API call fails
        return View("Error");
    }

    public async Task<IActionResult> GetUpdatedSeatInfo( string spacecode, string companyTinNumber, string code)
    {
        try
        {
            // Reuse the logic from SeatArrangementView action to fetch seat information
            HttpResponseMessage response = await _httpClient.GetAsync($"cinema/getCinemaSeatArrangment?orgTin={companyTinNumber}&spaceCode={spacecode}");

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                // Deserialize into a single SeatLayout instance
                var updatedSeatModel = JsonConvert.DeserializeObject<SeatLayout>(responseData);

                // Make a second API call to get booked seats
                HttpResponseMessage bookedSeatsResponse = await _httpClient.GetAsync($"cinema/GetBookedSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");

                if (bookedSeatsResponse.IsSuccessStatusCode)
                {
                    string bookedSeatsData = await bookedSeatsResponse.Content.ReadAsStringAsync();
                    var bookedSeats = JsonConvert.DeserializeObject<List<string>>(bookedSeatsData);
                    updatedSeatModel.BookedSeats ??= bookedSeats;
                }
                else
                {
                    // Handle the case when the second API call fails
                    return PartialView("_SeatInfoPartialView", null);
                }

                HttpResponseMessage soldseatsResponse = await _httpClient.GetAsync($"cinema/GetSoldSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");
                if (soldseatsResponse.IsSuccessStatusCode)
                {
                    string soldSeatData = await soldseatsResponse.Content.ReadAsStringAsync();
                    var soldSeats = JsonConvert.DeserializeObject<List<string>>(soldSeatData);
                    updatedSeatModel.SoldSeats ??= soldSeats;
                }
                else
                {
                    // Handle the case where GetSoldSeats API fails 
                    return PartialView("_SeatInfoPartialView", null);
                }

                HttpResponseMessage availableSeatsResponse = await _httpClient.GetAsync($"cinema/GetAvailableSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");
                if (availableSeatsResponse.IsSuccessStatusCode)
                {
                    string availableSeatsData = await availableSeatsResponse.Content.ReadAsStringAsync();
                    var availableSeats = JsonConvert.DeserializeObject<List<string>>(availableSeatsData);
                    updatedSeatModel.AvailableSeats ??= availableSeats;

                    var anonymousObject = new
                    {
                        schedule = code,
                        allSeats = updatedSeatModel.AvailableSeats,
                        orgTin = companyTinNumber
                    };

                    string jsonBody = JsonConvert.SerializeObject(anonymousObject);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage takenSeatsResponse = await _httpClient.PostAsync("ManageSeatCache/GetCachedSeatsForSchedule", content);
                    if (takenSeatsResponse.IsSuccessStatusCode)
                    {
                        string takenSeatsResponseData = await takenSeatsResponse.Content.ReadAsStringAsync();
                        var takenSeats = JsonConvert.DeserializeObject<List<string>>(takenSeatsResponseData);
                        updatedSeatModel.TakenSeats ??= takenSeats;
                    }
                    else
                    {
                        return PartialView("_SeatInfoPartialView", null);
                    }
                }

                // Pass the updated SeatLayout instance to the partial view
                return View("_SeatInfoPartialView", updatedSeatModel);
            }
            else
            {
                // Handle the case when the first API call fails
                return PartialView("_SeatInfoPartialView", null);
            }
        }
        catch (Exception)
        {
            // Handle general exceptions
            return PartialView("_SeatInfoPartialView", null);
        }
    }

}
