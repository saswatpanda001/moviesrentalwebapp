using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MoviesRental.Services;

namespace MoviesRentalTesting.UnitTest
{
    public class CartServiceTests
    {
        private MovieRentalContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MovieRentalContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new MovieRentalContext(options);
        }

        [Fact]
        public async Task GetOrCreateCartAsync_WhenUserHasNoCart_ShouldCreateNewCart()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var service = new CartService(context);
            var userId = 1;

            // Act
            var result = await service.GetOrCreateCartAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task CalculateTotal_WithEmptyCart_ShouldReturnZero()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var service = new CartService(context);
            var cart = new Cart { UserId = 1, CartItems = new List<CartItem>() };

            // Act
            var result = service.CalculateTotal(cart);

            // Assert
            Assert.Equal(0m, result);
        }
    }
}
