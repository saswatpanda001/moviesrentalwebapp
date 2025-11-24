using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Controllers;
using MoviesRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MoviesRentalTesting.UnitTest
{
    public class MoviesControllerTests
    {
        private MoviesController GetController()
        {
            var options = new DbContextOptionsBuilder<MovieRentalContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new MovieRentalContext(options);
            return new MoviesController(context);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_ReturnsViewResult()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
