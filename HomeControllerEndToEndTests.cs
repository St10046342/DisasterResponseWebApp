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

namespace TestProjectDisasterApp.SystemTests
{
    public class HomeControllerEndToEndTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly ILogger<HomeController> _logger;

        public HomeControllerEndToEndTests()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"DisasterAppE2E_{Guid.NewGuid()}")
                .Options;

            var loggerMock = new Mock<ILogger<HomeController>>();
            _logger = loggerMock.Object;
        }

        // ======================
        // Full user workflow: Register → Login → Log Incident → Donate → Volunteer
        // ======================
        [Fact]
        public async Task FullUserWorkflow_E2ETest()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            var controller = TestHelpers.SetupController(context, _logger);

            // 1. Register
            var user = new User
            {
                FullName = "E2E User",
                Email = "e2euser@example.com",
                PasswordHash = "Password123",
                Role = "User"
            };
            var registerResult = await controller.Register(user) as IActionResult;
            Assert.NotNull(registerResult);

            // 2. Login
            var loginUser = new User { Email = "e2euser@example.com", PasswordHash = "Password123" };
            var loginResult = await controller.Login(loginUser) as IActionResult;
            Assert.NotNull(loginResult);
            Assert.Equal("e2euser@example.com", controller.HttpContext.Session.GetString("UserEmail"));

            // 3. Log Incident
            TestHelpers.SetUserSession(controller, user);
            var incident = new Incident
            {
                Title = "E2E Flood",
                Description = "E2E Test Flood Incident",
                Location = "Town E2E"
            };
            var incidentResult = await controller.LogIncident(incident) as IActionResult;
            Assert.NotNull(incidentResult);
            var dbIncident = await context.Incidents.FirstOrDefaultAsync(i => i.Title == "E2E Flood");
            Assert.NotNull(dbIncident);

            // 4. Log Donation
            var donation = new Donation
            {
                ResourceType = "Food",
                Quantity = 5
            };
            var donationResult = await controller.LogDonation(donation) as IActionResult;
            Assert.NotNull(donationResult);
            var dbDonation = await context.Donations.FirstOrDefaultAsync(d => d.ResourceType == "Food");
            Assert.NotNull(dbDonation);

            // 5. Volunteer registration
            var volunteer = new Volunteer
            {
                Skills = "Medical",
                Availability = "Weekends"
            };
            var volunteerResult = await controller.Volunteerss(volunteer) as IActionResult;
            Assert.NotNull(volunteerResult);
            var dbVolunteer = await context.Volunteers.FirstOrDefaultAsync(v => v.UserID == user.UserID);
            Assert.NotNull(dbVolunteer);

            // 6. Final assertions for user role
            var updatedUser = await context.Users.FindAsync(user.UserID);
            Assert.Equal("Volunteer", updatedUser.Role);
        }
    }
}
