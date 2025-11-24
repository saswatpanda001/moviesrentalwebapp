using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Controllers;
using MoviesRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesRental.Services;


namespace MoviesRentalTesting.InterfaceTest
{
    public class IntegrationTests
    {
        private MovieRentalContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MovieRentalContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new MovieRentalContext(options);
        }

        [Fact]
        public async Task UsersController_Index_ReturnsViewWithUsers()
        {
            // Arrange
            using var context = GetInMemoryContext();
            context.Users.Add(new User { UserId = 1, Name = "Test User", Email = "test@test.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();

            var controller = new UsersController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task MoviesController_Index_ReturnsViewWithMovies()
        {
            // Arrange
            using var context = GetInMemoryContext();
            context.Movies.Add(new Movie { MovieId = 1, Title = "Test Movie", Stock = 5, RentalPrice = 3.99m });
            await context.SaveChangesAsync();

            var controller = new MoviesController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task CartService_GetOrCreateCartAsync_CreatesNewCart()
        {
            // Arrange
            using var context = GetInMemoryContext();
            context.Users.Add(new User { UserId = 1, Name = "Test User", Email = "test@test.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();

            var service = new CartService(context);

            // Act
            var cart = await service.GetOrCreateCartAsync(1);

            // Assert
            Assert.NotNull(cart);
            Assert.Equal(1, cart.UserId);
        }

        [Fact]
        public void CartService_CalculateTotal_WithEmptyCart_ReturnsZero()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var service = new CartService(context);
            var cart = new Cart { CartItems = new List<CartItem>() };

            // Act
            var total = service.CalculateTotal(cart);

            // Assert
            Assert.Equal(0m, total);
        }

        [Fact]
        public async Task RentalsController_Index_ReturnsView()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var controller = new RentalsController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ReviewsController_Index_ReturnsView()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var controller = new ReviewsController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task PaymentsController_Index_ReturnsView()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var controller = new PaymentsController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
