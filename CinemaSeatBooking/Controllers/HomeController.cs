﻿using CinemaSeatBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace CinemaSeatBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _tmdbApiKey = "1ba83335ce22421020a77845254a578e";
        private readonly string _tmdbApiBaseUrl = "https://api.themoviedb.org/3";
        public HomeController()
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api/");

            _httpClient.DefaultRequestHeaders.Add("Api-Key", "YourApiKey");
        }
        private async Task<List<MovieModel>> GetMoviesWithPosterUrls(List<MovieModel> movies)
        {
            var moviesWithPosterUrls = new List<MovieModel>();

            // Dictionary to map genre IDs to genre names
            Dictionary<int, string> genreDictionary = new Dictionary<int, string>
            {
                { 28, "Action" },
                { 12, "Adventure" },
                { 16, "Animation" },
                { 35, "Comedy" },
                { 80, "Crime" },
                { 99, "Documentary" },
                { 18, "Drama" },
                { 10751, "Family" },
                { 14, "Fantasy" },
                { 36, "History" },
                { 27, "Horror" },
                { 10402, "Music" },
                { 9648, "Mystery" },
                { 10749, "Romance" },
                { 878, "Science Fiction" },
                { 10770, "TV Movie" },
                { 53, "Thriller" },
                { 10752, "War" },
                { 37, "Western" }
            };

            foreach (var movie in movies)
            {
                var movieTitle = Uri.EscapeDataString(movie.MovieName);
                var apiUrl = $"{_tmdbApiBaseUrl}/search/movie?api_key={_tmdbApiKey}&query={movieTitle}";

                using (var tmdbClient = new HttpClient())
                {
                    var tmdbResponse = await tmdbClient.GetAsync(apiUrl);
                    if (tmdbResponse.IsSuccessStatusCode)
                    {
                        var tmdbData = await tmdbResponse.Content.ReadAsStringAsync();
                        var movieDetails = JsonConvert.DeserializeObject<MovieDetails>(tmdbData);

                        if (movieDetails.results.Count > 0)
                        {
                            var result = movieDetails.results[0];
                            var posterUrl = "https://image.tmdb.org/t/p/w500" + result.poster_path;

                            // Assign values to MovieModel properties
                            movie.PosterUrl = posterUrl;
                            movie.Overview = result.overview;
                            movie.GenreId = result.genre_ids;
                            movie.MovieId = result.id;
                            // Map genre IDs to genre names
                            movie.Genre = result.genre_ids?.Select(id => genreDictionary.ContainsKey(id) ? genreDictionary[id] : "Unknown").ToList();

                            moviesWithPosterUrls.Add(movie);
                        }
                    }
                }
            }

            return moviesWithPosterUrls;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                string formattedDate = currentDate.ToString("yyyy-MM-dd");

                HttpResponseMessage response = await _httpClient.GetAsync($"Cinema/GetProductsForFilterAndPreview?industryType=LKUP000120765&date={formattedDate}");

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    List<MovieModel> movies = JsonConvert.DeserializeObject<List<MovieModel>>(responseData);

                    // Call the method to get movies with poster URLs
                    var moviesWithPosters = await GetMoviesWithPosterUrls(movies);

                    return View(moviesWithPosters);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (HttpRequestException)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime selectedDate)
        {
            try
            {
                // Format the selected date
                string formattedDate = selectedDate.ToString("yyyy-MM-dd");

                // Make an HTTP GET request with the selected date
                HttpResponseMessage response = await _httpClient.GetAsync($"Cinema/GetProductsForFilterAndPreview?industryType=LKUP000120765&date={formattedDate}");

                if (response.IsSuccessStatusCode)
                {
                    // If the request is successful, read and deserialize the response
                    string responseData = await response.Content.ReadAsStringAsync();
                    List<MovieModel> movies = JsonConvert.DeserializeObject<List<MovieModel>>(responseData);

                    // Call the method to get movies with poster URLs
                    var moviesWithPosters = await GetMoviesWithPosterUrls(movies);

                    // Return the view with the list of movies
                    return View(moviesWithPosters);
                }
                else
                {
                    // If the request is not successful, return an error view
                    return View("Error");
                }
            }
            catch (HttpRequestException)
            {
                // If an exception occurs during the HTTP request, return an error view
                return View("Error");
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}