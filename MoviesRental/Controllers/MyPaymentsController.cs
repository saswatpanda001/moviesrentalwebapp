using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;

namespace MoviesRental.Controllers
{
    public class MyPaymentsController : Controller
    {
        private readonly MovieRentalContext _context;

        public MyPaymentsController(MovieRentalContext context)
        {
            _context = context;
        }

        // GET: My Payments (for users)
        public async Task<IActionResult> Index()
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var payments = await _context.Payments
                .Include(p => p.Rental)
                .ThenInclude(r => r.Movie) // Include Rental -> Movie
                .Where(p => p.UserId == userId.Value)
                .OrderByDescending(p => p.PaidOn)
                .ToListAsync();

            return View(payments);
        }

     
        // GET: Payment Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Rental)
                .ThenInclude(r => r.Movie)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            // Check if user owns this payment or is admin
            if (SessionManager.UserRole != "Admin" && payment.UserId != SessionManager.UserId)
            {
                return RedirectToAction("MyPayments");
            }

            return View(payment);
        }
    }
}