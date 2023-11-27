﻿using CinemaSeatBooking.Models;
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
    string posterUrl, string movieName, int movieId, string backdropPath)
        {
            // Construct the API URL for videos
            string videosApiUrl = $"{baseUrl}{movieId}/videos?api_key={apiKey}";

            // Make the API call for videos
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage videosResponse = await client.GetAsync(videosApiUrl);

                if (videosResponse.IsSuccessStatusCode)
                {
                    // Read and parse the videos response content
                    string videosResponseData = await videosResponse.Content.ReadAsStringAsync();
                    var videosResponseObj = JsonConvert.DeserializeObject<dynamic>(videosResponseData);

                    // Extract the key of the first trailer
                    string trailerKey = null;
                    foreach (var result in videosResponseObj.results)
                    {
                        if (result.type == "Trailer" && result.site == "YouTube")
                        {
                            trailerKey = result.key.ToString();
                            break;
                        }
                    }


                    // Construct the API URL for movie details
                    string detailsApiUrl = $"{baseUrl}{movieId}?api_key={apiKey}&append_to_response=release_dates";

                    // Make the API call for movie details
                    HttpResponseMessage detailsResponse = await client.GetAsync(detailsApiUrl);

                    if (detailsResponse.IsSuccessStatusCode)
                    {
                        // Read and parse the movie details response content
                        string detailsResponseData = await detailsResponse.Content.ReadAsStringAsync();
                        var movieDetails = JsonConvert.DeserializeObject<MovieModel>(detailsResponseData);

                        // Set additional properties in your MovieModel
                        movieDetails.MovieCode = movieCode;
                        movieDetails.CompanyName = companyName;
                        movieDetails.Overview = overview;
                        movieDetails.PosterUrl = posterUrl;
                        movieDetails.MovieName = movieName;
                        movieDetails.BackdropPath = backdropPath;
                        movieDetails.YoutubeKey = trailerKey; // Add the YoutubeKey property

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

                        // Make the API call for cast details
                        string castApiUrl = $"{baseUrl}{movieId}/credits?api_key={apiKey}";
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
                                    ProfilePath = "https://image.tmdb.org/t/p/w500" + c["profile_path"].ToString()
                                })
                                .ToList();

                            // Assign the castList to the Cast property in MovieModel
                            movieDetails.Cast = castList;
                        }

                        return View(movieDetails);
                    }
                    else
                    {
                        // Handle movie details API error
                        return View("Error");
                    }
                }
                else
                {
                    // Handle videos API error
                    return View("Error");
                }
            }
        }


    }
}