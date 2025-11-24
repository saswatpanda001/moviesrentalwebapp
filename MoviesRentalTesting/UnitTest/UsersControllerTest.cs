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
    public class UsersControllerTests
    {
        private UsersController GetController()
        {
            var options = new DbContextOptionsBuilder<MovieRentalContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new MovieRentalContext(options);
            return new UsersController(context);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithUsersList()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
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
