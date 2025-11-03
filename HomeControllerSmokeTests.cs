using Disaster_App.Controllers;
using Disaster_App.Data;
using Disaster_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestProjectDisasterApp.SmokeTests
{
    public class HomeControllerSmokeTests
    {
        private readonly ApplicationDbContext _context;
        private readonly HomeController _controller;

        public HomeControllerSmokeTests()
        {
            // Setup InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Add a logger mock
            var loggerMock = new Mock<ILogger<HomeController>>();

            // Initialize controller
            _controller = new HomeController(loggerMock.Object, _context);

            // Setup fake HttpContext with session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            _controller.ControllerContext.HttpContext = httpContext;
        }

        private void ClearDatabase()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.Incidents.RemoveRange(_context.Incidents);
            _context.Donations.RemoveRange(_context.Donations);
            _context.Volunteers.RemoveRange(_context.Volunteers);
            _context.VolunteerTasks.RemoveRange(_context.VolunteerTasks);
            _context.SaveChanges();
        }

        private void SetupSessionForUser(User user)
        {
            _controller.HttpContext.Session.SetInt32("UserId", user.UserID);
            _controller.HttpContext.Session.SetString("UserEmail", user.Email);
            _controller.HttpContext.Session.SetString("UserName", user.FullName);
            _controller.HttpContext.Session.SetString("UserRole", user.Role ?? "User");
        }

        // ======================
        // Smoke Tests
        // ======================

        [Fact]
        public async Task Register_NewUser_SmokeTest()
        {
            ClearDatabase();

            var user = new User
            {
                FullName = "John Doe",
                Email = "john@example.com",
                PasswordHash = "password123",
                Role = "User"
            };

            var result = await _controller.Register(user);

            Assert.NotNull(result);
            var dbUser = _context.Users.FirstOrDefault(u => u.Email == "john@example.com");
            Assert.NotNull(dbUser);
        }

        [Fact]
        public async Task Login_ValidUser_SmokeTest()
        {
            ClearDatabase();

            var password = "mypassword";
            var user = new User
            {
                FullName = "Jane Doe",
                Email = "jane@example.com",
                PasswordHash = _controller.HashPassword(password),
                Role = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginUser = new User
            {
                Email = "jane@example.com",
                PasswordHash = password
            };

            var result = await _controller.Login(loginUser);

            Assert.NotNull(result);
            Assert.Equal("jane@example.com", _controller.HttpContext.Session.GetString("UserEmail"));
            Assert.Equal("User", _controller.HttpContext.Session.GetString("UserRole"));
        }

        [Fact]
        public async Task LogIncident_SmokeTest()
        {
            ClearDatabase();

            var user = new User
            {
                FullName = "Test User",
                Email = "testuser@example.com",
                PasswordHash = "123",
                Role = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            SetupSessionForUser(user);

            var incident = new Incident
            {
                Title = "Flood",
                Description = "River overflow",
                Location = "Town A"
            };

            var result = await _controller.LogIncident(incident);

            Assert.NotNull(result);

            var dbIncident = _context.Incidents.FirstOrDefault(i => i.Title == "Flood");
            Assert.NotNull(dbIncident);
            Assert.Equal(user.UserID, dbIncident.ReportedBy);
        }

        [Fact]
        public async Task LogDonation_SmokeTest()
        {
            ClearDatabase();

            var user = new User
            {
                FullName = "Donor User",
                Email = "donor@example.com",
                PasswordHash = "123",
                Role = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            SetupSessionForUser(user);

            var donation = new Donation
            {
                ResourceType = "Food",
                Quantity = 10
            };

            var result = await _controller.LogDonation(donation);

            Assert.NotNull(result);

            var dbDonation = _context.Donations.FirstOrDefault(d => d.ResourceType == "Food");
            Assert.NotNull(dbDonation);
            Assert.Equal(user.FullName, dbDonation.DonorName);
        }

        [Fact]
        public async Task Volunteerss_SmokeTest()
        {
            ClearDatabase();

            var user = new User
            {
                FullName = "Volunteer User",
                Email = "volunteer@example.com",
                PasswordHash = "123",
                Role = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            SetupSessionForUser(user);

            var volunteer = new Volunteer
            {
                Skills = "Medical",
                Availability = "Weekends"
            };

            var result = await _controller.Volunteerss(volunteer);

            Assert.NotNull(result);

            var dbVolunteer = _context.Volunteers.FirstOrDefault(v => v.UserID == user.UserID);
            var updatedUser = _context.Users.Find(user.UserID);

            Assert.NotNull(dbVolunteer);
            Assert.Equal("Volunteer", updatedUser.Role);
        }
    }

    // ======================
    // In-memory session for testing
    // ======================
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _storage = new();

        public IEnumerable<string> Keys => _storage.Keys;
        public string Id => Guid.NewGuid().ToString();
        public bool IsAvailable => true;

        public void Clear() => _storage.Clear();
        public Task CommitAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _storage.Remove(key);
        public void Set(string key, byte[] value) => _storage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);
    }
}
