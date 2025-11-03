using Disaster_App.Controllers;
using Disaster_App.Data;
using Disaster_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectDisasterApp.Helpers
{
    internal static class TestHelpers
    {
        // ======================
        // Setup controller with HttpContext, Session, TempData
        // ======================
        public static HomeController SetupController(ApplicationDbContext context, ILogger<HomeController> logger = null)
        {
            logger ??= new Mock<ILogger<HomeController>>().Object;

            var controller = new HomeController(logger, context);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();

            controller.ControllerContext.HttpContext = httpContext;
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            return controller;
        }

        // ======================
        // Seed a user in the database
        // ======================
        public static async Task<User> SeedUser(ApplicationDbContext context, string email, string fullName, string role = "User")
        {
            var user = new User
            {
                Email = email,
                FullName = fullName,
                PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes("Password123")), // default password
                Role = role,
                CreatedAt = DateTime.Now
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        // ======================
        // Seed a volunteer for a user
        // ======================
        public static async Task<Volunteer> SeedVolunteer(ApplicationDbContext context, User user, string skills = "Medical", string availability = "Weekends")
        {
            var volunteer = new Volunteer
            {
                UserID = user.UserID,
                Skills = skills,
                Availability = availability,
                JoinedAt = DateTime.Now
            };

            context.Volunteers.Add(volunteer);
            await context.SaveChangesAsync();
            return volunteer;
        }

        // ======================
        // Seed an incident
        // ======================
        public static async Task<Incident> SeedIncident(ApplicationDbContext context, User user, string title, string description, string location)
        {
            var incident = new Incident
            {
                Title = title,
                Description = description,
                Location = location,
                DateReported = DateTime.Now,
                ReportedBy = user.UserID
            };

            context.Incidents.Add(incident);
            await context.SaveChangesAsync();
            return incident;
        }

        // ======================
        // Seed a donation
        // ======================
        public static async Task<Donation> SeedDonation(ApplicationDbContext context, User user, string resourceType, int quantity)
        {
            var donation = new Donation
            {
                DonorName = user.FullName,
                Email = user.Email,
                ResourceType = resourceType,
                Quantity = quantity,
                DonationDate = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            context.Donations.Add(donation);
            await context.SaveChangesAsync();
            return donation;
        }

        // ======================
        // Set session for a logged-in user
        // ======================
        public static void SetUserSession(HomeController controller, User user)
        {
            var session = controller.HttpContext.Session;
            session.SetInt32("UserId", user.UserID);
            session.SetString("UserEmail", user.Email);
            session.SetString("UserName", user.FullName);
            session.SetString("UserRole", user.Role ?? "User");
        }
    }

    // ======================
    // In-memory session for testing
    // ======================
    internal class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new();

        public IEnumerable<string> Keys => _sessionStorage.Keys;
        public string Id => "TestSessionId";
        public bool IsAvailable => true;

        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);
        public void Set(string key, byte[] value) => _sessionStorage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
    }
}
