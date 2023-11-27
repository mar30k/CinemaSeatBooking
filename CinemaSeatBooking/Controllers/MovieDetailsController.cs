using CinemaSeatBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CinemaSeatBooking.Controllers
{
    public class MovieDetailsController : Controller
    {
        private readonly string apiKey = "1ba83335ce22421020a77845254a578e";
        private readonly string baseUrl = "https://api.themoviedb.org/3/movie/";

        [HttpPost]
        public IActionResult Details(string movieCode, string companyName, string overview,
             string posterUrl, string movieName, int movieId, string backdropPath)
        {
            return RedirectToAction("DetailsViewPage", new
            {
                movieCode = movieCode,
                companyName = companyName,
                overview = overview,
                posterUrl = posterUrl,
                movieName = movieName,
                movieId = movieId,
                backdropPath = backdropPath
            });
        }

        public async Task<IActionResult> DetailsViewPage(string movieCode, string companyName, string overview,
        string posterUrl, string movieName, int movieId ,string backdropPath)
        {
            // Construct the API URL
            string apiUrl = $"{baseUrl}{movieId}?api_key={apiKey}&append_to_response=release_dates";

            // Make the API call
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Read and parse the response content
                    string responseData = await response.Content.ReadAsStringAsync();
                    var movieDetails = JsonConvert.DeserializeObject<MovieModel>(responseData);
                    var responseObj = JsonConvert.DeserializeObject<dynamic>(responseData);

                    // Extract runtime from the API response
                    movieDetails.RunTime = responseObj.runtime?.ToObject<int?>();
                    // Set additional properties in your MovieModel
                    movieDetails.MovieCode = movieCode;
                    movieDetails.CompanyName = companyName;
                    movieDetails.Overview = overview;
                    movieDetails.PosterUrl = posterUrl;
                    movieDetails.MovieName = movieName;
                    movieDetails.BackdropPath = backdropPath;
                    // Extract genre information from movieDetails
                    var genreNames = new List<string>();
                    if (movieDetails.Genre != null)
                    {
                        foreach (var genreInfo in movieDetails.Genre)
                        {
                            if (genreInfo is string)
                            {
                                genreNames.Add((string)genreInfo);
                            }
                            else if (genreInfo is JObject)
                            {
                                string genreName = ((JObject)genreInfo)["name"].ToString();
                                genreNames.Add(genreName);
                            }
                        }
                    }

                    // Assign the genre names to the GenreNames property in MovieModel
                    movieDetails.GenreNames = genreNames;
                    string castApiUrl = $"{baseUrl}{movieId}/credits?api_key={apiKey}";

                    // Make the API call for cast details
                    HttpResponseMessage castResponse = await client.GetAsync(castApiUrl);

                    if (castResponse.IsSuccessStatusCode)
                    {
                        string castResponseData = await castResponse.Content.ReadAsStringAsync();
                        var castDetails = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(castResponseData);

                        // Extract cast names and profile paths directly
                        var castList = ((IEnumerable<dynamic>)castDetails["cast"])
                            .Select(c => new CastMember
                            {
                                Name = c["name"].ToString(),
                                ProfilePath = "https://image.tmdb.org/t/p/w500"+c["profile_path"].ToString()
                            })
                            .ToList();

                        // Assign the castList to the Cast property in MovieModel
                        movieDetails.Cast = castList;
                    }
                    return View(movieDetails);
                }
                else
                {
                    // Handle API error
                    return View("Error");
                }
            }

        }

    }
}