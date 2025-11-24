using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;

namespace MoviesRental.Services
{
    public class CartService
    {
        private readonly MovieRentalContext _context;

        public CartService(MovieRentalContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Movie)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                // reload with includes
                cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Movie)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }

            return cart!;
        }

        public async Task AddToCartAsync(int userId, int movieId, int quantity = 1, int rentalDays = 7)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.MovieId == movieId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.RentalDays = rentalDays;
            }
            else
            {
                var cartItem = new CartItem
                {
                    MovieId = movieId,
                    Quantity = quantity,
                    RentalDays = rentalDays
                };
                cart.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(int cartItemId, int quantity, int rentalDays)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Movie)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem != null)
            {
                cartItem.Quantity = Math.Max(1, quantity);
                cartItem.RentalDays = Math.Clamp(rentalDays, 1, 30);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
        }

        public decimal CalculateTotal(Cart cart)
        {
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return 0m;

            return cart.CartItems.Sum(ci => ci.Quantity * ci.RentalDays * ci.Movie.RentalPrice);
        }
    }
}
