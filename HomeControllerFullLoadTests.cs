using Disaster_App.Controllers;
using Disaster_App.Data;
using Disaster_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestProjectDisasterApp.Helpers;
using Xunit;

namespace TestProjectDisasterApp.LoadTests
{
    public class HomeControllerLoadTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly ILogger<HomeController> _logger;

        public HomeControllerLoadTests()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"DisasterAppLoadTest_{Guid.NewGuid()}")
                .Options;

            var loggerMock = new Mock<ILogger<HomeController>>();
            _logger = loggerMock.Object;
        }

        // ======================
        // Helper to create controller with HttpContext + Session
        // ======================
        private HomeController CreateController(ApplicationDbContext context)
        {
            return TestHelpers.SetupController(context, _logger);
        }

        // ======================
        // Simulate multiple concurrent users
        // ======================
        private async Task RunConcurrentUsers(Func<HomeController, int, Task> userAction, int userCount = 10)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < userCount; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var context = new ApplicationDbContext(_dbOptions);
                    var controller = CreateController(context);
                    await userAction(controller, i);
                }));
            }

            await Task.WhenAll(tasks);
        }

        // ======================
        // Load Test: Register
        // ======================
        [Fact]
        public async Task LoadTest_Register()
        {
            await RunConcurrentUsers(async (controller, index) =>
            {
                var email = $"user{index}@example.com";
                var user = new User
                {
                    FullName = $"User {index}",
                    Email = email,
                    PasswordHash = "password123",
                    Role = "User"
                };

                var result = await controller.Register(user);
                Assert.NotNull(result);

                var dbUser = await controller._context.Users.FirstOrDefaultAsync(u => u.Email == email);
                Assert.NotNull(dbUser);
            });
        }

        // ======================
        // Load Test: Login
        // ======================
        [Fact]
        public async Task LoadTest_Login()
        {
            await RunConcurrentUsers(async (controller, index) =>
            {
                // Seed user
                var email = $"login{index}@example.com";
                var password = "mypassword";
                var user = await TestHelpers.SeedUser(controller._context, email, $"LoginUser {index}");
                user.PasswordHash = controller.HashPassword(password);
                await controller._context.SaveChangesAsync();

                var loginUser = new User { Email = email, PasswordHash = password };
                var result = await controller.Login(loginUser);

                Assert.NotNull(result);
                Assert.Equal(email, controller.HttpContext.Session.GetString("UserEmail"));
                Assert.Equal("User", controller.HttpContext.Session.GetString("UserRole"));
            });
        }

        // ======================
        // Load Test: LogIncident
        // ======================
        [Fact]
        public async Task LoadTest_LogIncident()
        {
            await RunConcurrentUsers(async (controller, index) =>
            {
                var user = await TestHelpers.SeedUser(controller._context, $"incident{index}@example.com", $"IncidentUser {index}");
                TestHelpers.SetUserSession(controller, user);

                var incident = new Incident
                {
                    Title = $"Incident {index}",
                    Description = "Test incident",
                    Location = $"Location {index}"
                };

                var result = await controller.LogIncident(incident);
                Assert.NotNull(result);

                var dbIncident = await controller._context.Incidents.FirstOrDefaultAsync(i => i.Title == incident.Title);
                Assert.NotNull(dbIncident);
            });
        }

        // ======================
        // Load Test: LogDonation
        // ======================
        [Fact]
        public async Task LoadTest_LogDonation()
        {
            await RunConcurrentUsers(async (controller, index) =>
            {
                var user = await TestHelpers.SeedUser(controller._context, $"donor{index}@example.com", $"DonorUser {index}");
                TestHelpers.SetUserSession(controller, user);

                var donation = new Donation
                {
                    ResourceType = $"Resource {index}",
                    Quantity = index + 1
                };

                var result = await controller.LogDonation(donation);
                Assert.NotNull(result);

                var dbDonation = await controller._context.Donations.FirstOrDefaultAsync(d => d.ResourceType == donation.ResourceType);
                Assert.NotNull(dbDonation);
            });
        }

        // ======================
        // Load Test: Volunteer Registration
        // ======================
        [Fact]
        public async Task LoadTest_VolunteerRegistration()
        {
            await RunConcurrentUsers(async (controller, index) =>
            {
                var user = await TestHelpers.SeedUser(controller._context, $"volunteer{index}@example.com", $"VolunteerUser {index}");
                TestHelpers.SetUserSession(controller, user);

                var volunteer = new Volunteer
                {
                    Skills = "Medical",
                    Availability = "Weekends"
                };

                var result = await controller.Volunteerss(volunteer);
                Assert.NotNull(result);

                var dbVolunteer = await controller._context.Volunteers.FirstOrDefaultAsync(v => v.UserID == user.UserID);
                Assert.NotNull(dbVolunteer);
                Assert.Equal("Volunteer", (await controller._context.Users.FindAsync(user.UserID)).Role);
            });
        }
    }
}
