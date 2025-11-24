using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MoviesRental.Controllers;
using MoviesRental.Models;
using MoviesRental.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MoviesRentalTesting.UnitTest
{
    public class CartControllerMockTests
    {


        private readonly Mock<CartService> _mockCartService;
        private readonly Mock<MovieRentalContext> _mockContext;
        private readonly CartController _controller;

        public CartControllerMockTests()
        {
            _mockCartService = new Mock<CartService>(Mock.Of<MovieRentalContext>());
            _mockContext = new Mock<MovieRentalContext>();
            _controller = new CartController(_mockCartService.Object, _mockContext.Object);
        }

        [Fact]
        public async Task Index_WhenUserNotLoggedIn_ShouldRedirectToLogin()
        {
            // Arrange - Use Reflection to simulate no user (since SessionManager is static)
            // In real scenario, you'd mock HttpContext or use proper session management
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _controller.Index();

            // Assert - This will redirect because SessionManager.UserId is null by default
            Assert.IsType<RedirectToActionResult>(result);
        }

       

        [Fact]
        public void Controller_Creation_ShouldWork()
        {
            // Arrange
            var mockCartService = new Mock<CartService>(Mock.Of<MovieRentalContext>());
            var mockContext = new Mock<MovieRentalContext>();

            // Act
            var controller = new CartController(mockCartService.Object, mockContext.Object);

            // Assert
            Assert.NotNull(controller); // Simple test - just create controller
        }

     


        [Fact]
        public void CalculateTotal_WithNullCart_ShouldReturnZero()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MovieRentalContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            var context = new MovieRentalContext(options);
            var service = new CartService(context);

            // Act
            var result = service.CalculateTotal(null);

            // Assert
            Assert.Equal(0m, result); // Simple test - null cart = zero total
        }

        [Fact]
        public void CalculateTotal_WithEmptyCart_ShouldReturnZero()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MovieRentalContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            var context = new MovieRentalContext(options);
            var service = new CartService(context);
            var cart = new Cart { CartItems = new List<CartItem>() };

            // Act
            var result = service.CalculateTotal(cart);

            // Assert
            Assert.Equal(0m, result); // Simple test - empty cart = zero total
        }

    }

}
