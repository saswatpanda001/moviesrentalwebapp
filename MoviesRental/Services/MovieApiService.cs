using System.Text.Json;
using MoviesRental.Models;

namespace MoviesRental.Services
{
    public class MovieApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "12a8c26";
        private const string BaseUrl = "http://www.omdbapi.com/";

        public MovieApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Search for multiple movies
        public async Task<SearchResult> SearchMovies(string searchTerm, string year = "", int page = 1)
        {
            try
            {
                var url = $"{BaseUrl}?apikey={ApiKey}&s={Uri.EscapeDataString(searchTerm)}&y={year}&page={page}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<SearchResult>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new SearchResult { Error = ex.Message };
            }

            return new SearchResult { Error = "API request failed" };
        }

        // Get detailed info for single movie
        public async Task<MovieApi> GetMovieDetails(string imdbId)
        {
            try
            {
                var url = $"{BaseUrl}?apikey={ApiKey}&i={imdbId}&plot=full";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var movie = JsonSerializer.Deserialize<MovieApi>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return movie;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
            }

            return null;
        }
    }
}