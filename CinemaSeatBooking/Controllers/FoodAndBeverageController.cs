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
        public IActionResult Products([FromForm] string movieScheduleCode, [FromForm] string companyTinNumber, [FromForm] string companyName, [FromForm] string movieName,
                    [FromForm] string hallName, [FromForm] string utcTime, [FromForm] string selectedDate, [FromForm] decimal price, [FromForm] string dimension, [FromForm] string spaceType, [FromForm] string articleCode, [FromForm] int numberOfElements)
        {
            // Convert the selectedSeats string to a list or array as needed
            return RedirectToAction("ProductsView", new
            {
                companyTinNumber,
                movieScheduleCode,
                companyName,
                hallName,
                utcTime,
                selectedDate,
                movieName,
                price,
                dimension,
                spaceType,
                articleCode,
                numberOfElements,
        });

        }

        public async Task<IActionResult> ProductsView(string companyTinNumber, string movieScheduleCode, string companyName, string hallName,
            string utcTime, string selectedDate, string movieName, decimal price, string dimension, string spaceType, string articleCode, string numberOfElements)
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
                    CompanyName = companyName,
                    HallName = hallName,
                    ScheduleTime = utcTime,
                    ScheduleDate = selectedDate,
                    MovieName = movieName,
                    Price = price,
                    Dimension = dimension,
                    SpaceType = spaceType,
                    ArticleCode = articleCode,
                    NumberOfSeats = numberOfElements,

                };
                TempData["NumberOfElements"] = numberOfElements;
                return View(viewModel);
            }
            else
            {
                return View(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CalculateBill(string movieName, string price, string movieScheduleCode, string companyTin, string articleCode, string numberOfSeats, string selectedItems)
        {
            List<SelectedItem> selectedItemsList = JsonConvert.DeserializeObject<List<SelectedItem>>(selectedItems);

            var lineItems = new List<object>();

            foreach (var selectedItem in selectedItemsList)
            {
                var lineItem = new
                {
                    articleName = selectedItem.name,
                    articleCode = selectedItem.articleCode,
                    price = selectedItem.unitPrice,
                    quantity = selectedItem.quantity,
                    lineItemNote = "",
                    specialFlag = ""
                };
                lineItems.Add(lineItem);
            }

            // Add the movie-related line item
            lineItems.Add(new
            {
                articleName = movieName,
                articleCode = articleCode,
                price = price,
                quantity = numberOfSeats,
                lineItemNote = "",
                specialFlag = "@CNET_MOVIE_PRODUCT"
            });

            var payload = new
            {
                Consignee = "0912141914",
                OrgTin = companyTin,
                IndustryType = "LKUP000120765",
                Schedule = movieScheduleCode,
                LineItems = lineItems
            };

            return Json("Your response");
        }

        public class SelectedItem
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public decimal unitPrice { get; set; }
            public string articleCode { get; set; }
            public decimal price { get; set; }
        }

    }

}