using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CinemaSeatBooking.Models;
namespace CinemaSeatBooking.Controllers
{
    public class FoodAndBeverageController : Controller
    {
        private readonly HttpClient _httpClient;
        public FoodAndBeverageController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api/")
            };
        }
        [HttpPost]
        public IActionResult Products([FromForm] string movieScheduleCode, [FromForm] string companyTinNumber)
        {
            // Convert the selectedSeats string to a list or array as needed
            return RedirectToAction("ProductsView", new
            {
                companyTinNumber,
                movieScheduleCode,
            });

        }

        public async Task<IActionResult> ProductsView(string companyTinNumber, string movieScheduleCode)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Product/GetProducts?orgTin={companyTinNumber}&type=Restaurant&consignee=0912141914&platform=Web&longitude=0");

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                var productsData = JsonConvert.DeserializeObject<List<Product>>(responseData);

                var viewModel = new ProductsViewModel
                {
                    MovieScheduleCode = movieScheduleCode,
                    CompanyTinNumber = companyTinNumber,
                    Products = productsData,
                };

                return View(viewModel);
            }
            else
            {
                return View(null);
            }
        }
    }

}
