using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;
using System.Diagnostics;

namespace MoviesRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieRentalContext _context;

        public HomeController(ILogger<HomeController> logger, MovieRentalContext context)
        {
            _logger = logger;
            _context = context;
        }

        

        public IActionResult Index()
        {

            // Query to get user carts with their items and total cost
            var userCarts = _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Movie)
            .Include(c => c.User)
            .Where(c => c.CartItems.Any())
            .Select(c => new
            {
                c.CartId,
                UserName = c.User.Name,
                c.CreatedAt,
                TotalItems = c.CartItems.Count,
                TotalCost = c.CartItems.Sum(ci => ci.Movie.RentalPrice * ci.Quantity),
                CartItems = c.CartItems.Select(ci => new
                {
                    ci.Movie.Title,
                    ci.Movie.Genre,
                    ci.Quantity,
                    ci.RentalDays,
                    ItemTotal = ci.Movie.RentalPrice * ci.Quantity
                }).ToList()
            })
            .ToList();

            // Display results
            foreach (var cart in userCarts)
            {
                Console.WriteLine($"Cart ID: {cart.CartId}, User: {cart.UserName}");
                Console.WriteLine($"Total Items: {cart.TotalItems}, Total Cost: ${cart.TotalCost}");
                Console.WriteLine("Items:");

                foreach (var item in cart.CartItems)
                {
                    Console.WriteLine($"  - {item.Title} ({item.Genre})");
                    Console.WriteLine($"    Qty: {item.Quantity}, Days: {item.RentalDays}, Cost: ${item.ItemTotal}");
                }
                Console.WriteLine("---");
            }



            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
