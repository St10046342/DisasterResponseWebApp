using Disaster_App.Controllers;
using Disaster_App.Data;
using Disaster_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using TestProjectDisasterApp.Helpers;
using Xunit;

namespace TestProjectDisasterApp.ViewTests
{
    public class HomeControllerViewTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly ILogger<HomeController> _logger;

        public HomeControllerViewTests()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"DisasterAppViewTest_{Guid.NewGuid()}")
                .Options;

            _logger = new Mock<ILogger<HomeController>>().Object;
        }

        private HomeController CreateController(ApplicationDbContext context)
        {
            return TestHelpers.SetupController(context, _logger);
        }

        // ======================
        // Test Index View
        // ======================
        [Fact]
        public async Task Index_Returns_ViewResult()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            var controller = CreateController(context);

            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Index");
        }

        // ======================
        // Test Register View GET
        // ======================
        [Fact]
        public async Task Register_GET_Returns_ViewResult()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            var controller = CreateController(context);

            var result = controller.Register() as ViewResult;

            Assert.NotNull(result);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Register");
        }

        // ======================
        // Test Login View GET
        // ======================
        [Fact]
        public async Task Login_GET_Returns_ViewResult()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            var controller = CreateController(context);

            var result = controller.Login() as ViewResult;

            Assert.NotNull(result);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Login");
        }

        // ======================
        // Test Volunteer View
        // ======================
        [Fact]
        public async Task Volunteer_GET_Returns_ViewResult()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            var controller = CreateController(context);

            // Seed a user and set session
            var user = await TestHelpers.SeedUser(context, "volunteer@test.com", "Volunteer User");
            TestHelpers.SetUserSession(controller, user);

            var result = controller.Volunteerss() as ViewResult;

            Assert.NotNull(result);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Volunteerss");
        }

        // ======================
        // Test Incident Details View
        // ======================
        [Fact]
        public async Task IncidentDetails_Returns_ViewResult_With_Model()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            var controller = CreateController(context);

            var user = await TestHelpers.SeedUser(context, "incident@test.com", "Incident User");
            TestHelpers.SetUserSession(controller, user);

            var incident = await TestHelpers.SeedIncident(context, user, "Flood", "Test Flood", "Town A");

            var result = controller.LogIncident() as ViewResult; // or any GET method
            Assert.NotNull(result);

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsType<Incident>(result.Model);
        }
    }
}
