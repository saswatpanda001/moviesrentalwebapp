using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;

namespace MoviesRental.Controllers
{
    public class MyRentalsController : Controller
    {
        private readonly MovieRentalContext _context;

        public MyRentalsController(MovieRentalContext context)
        {
            _context = context;
        }

        // GET: My Rentals (for users)
        public async Task<IActionResult> Index(int? paymentId)
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rentals = await _context.Rentals
                .Include(r => r.Movie)
                .Include(r => r.Payment)
                .Include(r => r.User)
                .Where(r => r.UserId == userId.Value)
                .OrderByDescending(r => r.RentedOn)
                .ToListAsync();

            if (paymentId.HasValue)
            {
                ViewBag.SuccessMessage = $"Payment successful! Payment ID: {paymentId.Value}";
            }

            return View(rentals);
        }

        // GET: Rental Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Movie)
                .Include(r => r.User)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.RentalId == id);

            if (rental == null)
            {
                return NotFound();
            }

            // Check if user owns this rental or is admin
            if (SessionManager.UserRole != "Admin" && rental.UserId != SessionManager.UserId)
            {
                return RedirectToAction("Index");
            }

            return View(rental);
        }

        // Return a rental
        [HttpPost]
        public async Task<IActionResult> ReturnRental(int rentalId)
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rental = await _context.Rentals
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId && r.UserId == userId.Value);

            if (rental == null)
            {
                return NotFound();
            }

            if (rental.ReturnedOn != null)
            {
                TempData["ErrorMessage"] = "This rental has already been returned.";
                return RedirectToAction("Index");
            }

            // Calculate late fee if applicable
            decimal lateFee = 0;
            if (DateTime.Now > rental.DueDate)
            {
                var daysLate = (DateTime.Now - rental.DueDate).Days;
                lateFee = daysLate * 2.00m; // $2 per day late fee
            }

            rental.ReturnedOn = DateTime.Now;

            // Update movie stock
            rental.Movie.Stock += 1;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = lateFee > 0
                ? $"Movie returned successfully! Late fee applied: ${lateFee}"
                : "Movie returned successfully!";

            return RedirectToAction("Index");
        }
    }
}