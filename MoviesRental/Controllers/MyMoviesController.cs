using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;
using MoviesRental.Services;

namespace MoviesRental.Controllers
{
    public class MyMoviesController : Controller
    {
        private readonly MovieRentalContext _context;
        private readonly CartService _cartService;
        private readonly MovieApiService _movieApiService;

        public MyMoviesController(MovieRentalContext context, CartService cartService, MovieApiService movieApiService)
        {
            _context = context;
            _cartService = cartService;
            _movieApiService = movieApiService;
        }





        public IActionResult SearchApi()
        {
            return View();
        }

   
        public async Task<IActionResult> Search(string searchTerm, string year = "", int page = 1)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ViewBag.Error = "Please enter a movie title to search";
                return View("SearchApi");
            }

            var result = await _movieApiService.SearchMovies(searchTerm, year, page);

            if (result == null || result.Response != "True")
            {
                ViewBag.Error = result?.Error ?? "No movies found. Please try a different search term.";
                return View("SearchApi");
            }

            ViewBag.Movies = result.Search;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SearchYear = year;
            ViewBag.TotalResults = result.TotalResults;
            ViewBag.CurrentPage = page;

            return View("SearchApi");
        }

        // Action to view movie details
        public async Task<IActionResult> MovieDetails(string imdbId)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return RedirectToAction("SearchApi");
            }

            var movie = await _movieApiService.GetMovieDetails(imdbId);

            if (movie == null || movie.Response != "True")
            {
                ViewBag.Error = "Movie details not found";
                return View("SearchApi");
            }

            return View(movie);
        }


        // GET: MyMovies with search
        public async Task<IActionResult> Index(string searchString, string genreFilter)
        {
            var movies = _context.Movies.Where(m => m.Stock > 0).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString) ||
                                          (m.Description != null && m.Description.Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(genreFilter) && genreFilter != "All")
            {
                movies = movies.Where(m => m.Genre == genreFilter);
            }

            ViewBag.Genres = await _context.Movies
                .Select(m => m.Genre)
                .Distinct()
                .Where(g => g != null)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.GenreFilter = genreFilter;

            return View(await movies.ToListAsync());
        }

        // GET: MyMovies/Details/5 - Enhanced with reviews
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Reviews)
                    .ThenInclude(r => r.User) // Include user details for reviews
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Calculate average rating
            if (movie.Reviews.Any())
            {
                ViewBag.AverageRating = movie.Reviews.Average(r => r.Rating ?? 0);
                ViewBag.TotalReviews = movie.Reviews.Count;
            }
            else
            {
                ViewBag.AverageRating = 0;
                ViewBag.TotalReviews = 0;
            }

            // Check if user has already reviewed this movie
            var userId = SessionManager.UserId;
            if (userId.HasValue)
            {
                ViewBag.HasUserReviewed = await _context.Reviews
                    .AnyAsync(r => r.MovieId == id && r.UserId == userId.Value);
            }
            else
            {
                ViewBag.HasUserReviewed = false;
            }

            return View(movie);
        }

        // POST: Add to cart from MyMovies
        [HttpPost]
        public async Task<IActionResult> AddToCart(int movieId, int quantity = 1, int rentalDays = 7)
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login first" });
            }

            try
            {
                await _cartService.AddToCartAsync(userId.Value, movieId, quantity, rentalDays);
                return Json(new { success = true, message = "Added to cart successfully" });
               
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding to cart" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(int MovieId, int Rating, string Comment)
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Check if user has already reviewed this movie
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.MovieId == MovieId && r.UserId == userId.Value);

                if (existingReview != null)
                {
                    // Update existing review
                    existingReview.Rating = Rating;
                    existingReview.Comment = Comment;
                    existingReview.CreatedAt = DateTime.Now;
                }
                else
                {
                    // Create new review
                    var review = new Review
                    {
                        UserId = userId.Value,
                        MovieId = MovieId,
                        Rating = Rating,
                        Comment = Comment,
                        CreatedAt = DateTime.Now
                    };
                    _context.Reviews.Add(review);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Review submitted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Please provide a rating and comment.";
            }

            return RedirectToAction("Details", new { id = MovieId });
        }


        // POST: Delete a review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId && r.UserId == userId.Value);

            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Review deleted successfully!";
            }

            return RedirectToAction("Details", new { id = review?.MovieId });
        }
    }
}