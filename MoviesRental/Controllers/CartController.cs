using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;
using MoviesRental.Services;

namespace MoviesRental.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly MovieRentalContext _context;

        public CartController(CartService cartService, MovieRentalContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetOrCreateCartAsync(userId.Value);
            ViewBag.TotalAmount = _cartService.CalculateTotal(cart);

            return View(cart);
        }

        // POST: Add to cart
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

        // POST: Update cart item
        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, int quantity, int rentalDays)
        {
            try
            {
                await _cartService.UpdateCartItemAsync(cartItemId, quantity, rentalDays);

                // Return updated total
                var userId = SessionManager.UserId;
                if (userId != null)
                {
                    var cart = await _cartService.GetOrCreateCartAsync(userId.Value);
                    var total = _cartService.CalculateTotal(cart);
                    return Json(new { success = true, total = total.ToString("C2") });
                }

                return Json(new { success = false });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        // POST: Remove from cart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(cartItemId);

                // Return updated total
                var userId = SessionManager.UserId;
                if (userId != null)
                {
                    var cart = await _cartService.GetOrCreateCartAsync(userId.Value);
                    var total = _cartService.CalculateTotal(cart);
                    return Json(new { success = true, total = total.ToString("C2") });
                }

                return Json(new { success = false });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        // GET: Checkout/Payment
        public async Task<IActionResult> Checkout()
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetOrCreateCartAsync(userId.Value);
            if (!cart.CartItems.Any())
            {
                return RedirectToAction("Index");
            }

            ViewBag.TotalAmount = _cartService.CalculateTotal(cart);
            return View(cart);
        }



        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string paymentMethod, string fullName, string email, string phone, string address, string city, string zipcode)
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login first" });
            }

            // Basic validation
            if (string.IsNullOrEmpty(paymentMethod))
            {
                return Json(new { success = false, message = "Please select a payment method" });
            }

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(address))
            {
                return Json(new { success = false, message = "Please fill in all required fields" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = await _cartService.GetOrCreateCartAsync(userId.Value);
                if (!cart.CartItems.Any())
                {
                    return Json(new { success = false, message = "Cart is empty" });
                }

                // Calculate total amount
                var totalAmount = _cartService.CalculateTotal(cart);

                // Create rentals for each cart item
                var rentals = new List<Rental>();
                foreach (var cartItem in cart.CartItems)
                {
                    var rental = new Rental
                    {
                        UserId = userId.Value,
                        MovieId = cartItem.MovieId,
                        RentedOn = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(cartItem.RentalDays),
                        Price = cartItem.Quantity * cartItem.RentalDays * cartItem.Movie.RentalPrice
                    };
                    rentals.Add(rental);
                }

                // Add rentals to database first to get RentalIds
                _context.Rentals.AddRange(rentals);
                await _context.SaveChangesAsync();

                // Create payment records (one payment per rental)
                var payments = new List<Payment>();
                foreach (var rental in rentals)
                {
                    var payment = new Payment
                    {
                        RentalId = rental.RentalId,
                        UserId = userId.Value,
                        Amount = rental.Price,
                        PaymentMethod = paymentMethod,
                        Status = "Completed",
                        PaidOn = DateTime.Now
                    };
                    payments.Add(payment);
                }

                // Add payments to database
                _context.Payments.AddRange(payments);
                await _context.SaveChangesAsync();

                // Update movie stock
                foreach (var cartItem in cart.CartItems)
                {
                    var movie = await _context.Movies.FindAsync(cartItem.MovieId);
                    if (movie != null)
                    {
                        movie.Stock -= cartItem.Quantity;
                        if (movie.Stock < 0) movie.Stock = 0;
                    }
                }

                await _context.SaveChangesAsync();

                // Clear the cart after successful payment
                await _cartService.ClearCartAsync(userId.Value);

                // Commit transaction
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = $"Payment successful! {rentals.Count} movie(s) have been rented. Delivery information has been recorded.",
                    paymentId = payments.First().PaymentId
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Error processing payment: " + ex.Message });
            }
        }



    }
}
