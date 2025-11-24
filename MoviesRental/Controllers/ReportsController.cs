using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using MoviesRental.Models;

namespace MoviesRental.Controllers
{
    public class ReportsController : Controller
    {
        private readonly MovieRentalContext _context;

        public ReportsController(MovieRentalContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            if (SessionManager.UserRole != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            // Get statistics for the dashboard
            ViewBag.TotalMovies = await _context.Movies.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalRentals = await _context.Rentals.CountAsync();
            ViewBag.TotalPayments = await _context.Payments.CountAsync();
            ViewBag.TotalReviews = await _context.Reviews.CountAsync();

            return View();
        }


        // Generate single CSV with all data
        public async Task<IActionResult> GenerateCompleteReport()
        {
            if (SessionManager.UserRole != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var csvBuilder = new StringBuilder();

            // Add Movies section
            csvBuilder.AppendLine("=== MOVIES ===");
            csvBuilder.AppendLine("MovieId,Title,Genre,ReleaseYear,Description,Stock,RentalPrice");
            var movies = await _context.Movies.ToListAsync();
            foreach (var movie in movies)
            {
                csvBuilder.AppendLine($"{movie.MovieId},\"{EscapeCsv(movie.Title)}\",\"{EscapeCsv(movie.Genre ?? "")}\",{movie.ReleaseYear},\"{EscapeCsv(movie.Description ?? "")}\",{movie.Stock},{movie.RentalPrice}");
            }
            csvBuilder.AppendLine();

            // Add Users section
            csvBuilder.AppendLine("=== USERS ===");
            csvBuilder.AppendLine("UserId,Name,Email,Phone,Role,CreatedAt");
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                csvBuilder.AppendLine($"{user.UserId},\"{EscapeCsv(user.Name)}\",\"{EscapeCsv(user.Email)}\",\"{EscapeCsv(user.Phone ?? "")}\",\"{EscapeCsv(user.Role ?? "")}\",{user.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            }
            csvBuilder.AppendLine();

            // Add Rentals section
            csvBuilder.AppendLine("=== RENTALS ===");
            csvBuilder.AppendLine("RentalId,UserName,MovieTitle,RentedOn,DueDate,ReturnedOn,Price");
            var rentals = await _context.Rentals
                .Include(r => r.Movie)
                .Include(r => r.User)
                .ToListAsync();
            foreach (var rental in rentals)
            {
                csvBuilder.AppendLine($"{rental.RentalId},\"{EscapeCsv(rental.User.Name)}\",\"{EscapeCsv(rental.Movie.Title)}\",{rental.RentedOn:yyyy-MM-dd HH:mm:ss},{rental.DueDate:yyyy-MM-dd HH:mm:ss},\"{(rental.ReturnedOn.HasValue ? rental.ReturnedOn.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Not Returned")}\",{rental.Price}");
            }
            csvBuilder.AppendLine();

            // Add Payments section
            csvBuilder.AppendLine("=== PAYMENTS ===");
            csvBuilder.AppendLine("PaymentId,UserName,MovieTitle,Amount,PaymentMethod,Status,PaidOn");
            var payments = await _context.Payments
                .Include(p => p.Rental)
                .ThenInclude(r => r.Movie)
                .Include(p => p.User)
                .ToListAsync();
            foreach (var payment in payments)
            {
                csvBuilder.AppendLine($"{payment.PaymentId},\"{EscapeCsv(payment.User.Name)}\",\"{EscapeCsv(payment.Rental.Movie.Title)}\",{payment.Amount},\"{EscapeCsv(payment.PaymentMethod)}\",\"{EscapeCsv(payment.Status)}\",{payment.PaidOn:yyyy-MM-dd HH:mm:ss}");
            }
            csvBuilder.AppendLine();

            // Add Reviews section
            csvBuilder.AppendLine("=== REVIEWS ===");
            csvBuilder.AppendLine("ReviewId,UserName,MovieTitle,Rating,Comment,CreatedAt");
            var reviews = await _context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .ToListAsync();
            foreach (var review in reviews)
            {
                csvBuilder.AppendLine($"{review.ReviewId},\"{EscapeCsv(review.User.Name)}\",\"{EscapeCsv(review.Movie.Title)}\",{review.Rating ?? 0},\"{EscapeCsv(review.Comment ?? "")}\",{review.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            var csvData = csvBuilder.ToString();
            return File(Encoding.UTF8.GetBytes(csvData), "text/csv", $"Complete_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        // Helper method to escape CSV special characters
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            // Escape quotes by doubling them and wrap in quotes if contains comma, newline, or quote
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                value = value.Replace("\"", "\"\"");
                value = $"\"{value}\"";
            }
            return value;
        }
    }
}