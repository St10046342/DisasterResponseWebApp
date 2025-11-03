using Disaster_App.Data;
using Disaster_App.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

namespace TestProjectDisasterApp.DataTests
{
    public class ApplicationDbContextTests
    {
        // ✅ Creates an In-Memory EF Core Database for testing
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_Database_" + System.Guid.NewGuid())
                .Options;

            return new ApplicationDbContext(options);
        }

        // ✅ Test 1: Ensure DbContext Creates Successfully
        [Fact]
        public void CanCreateDatabaseContext()
        {
            var context = GetInMemoryDbContext();
            Assert.NotNull(context);
        }

        // ✅ Test 2: DbSets Exist
        [Fact]
        public void DbSets_Are_Correctly_Defined()
        {
            var context = GetInMemoryDbContext();

            Assert.NotNull(context.Users);
            Assert.NotNull(context.Incidents);
            Assert.NotNull(context.Donations);
            Assert.NotNull(context.Volunteers);
            Assert.NotNull(context.VolunteerTasks);
        }

        // ✅ Test 3: Can Add & Retrieve User
        [Fact]
        public void Can_Add_And_Retrieve_User()
        {
            var context = GetInMemoryDbContext();
            var user = new User
            {
                UserID = 1,
                FullName = "John Test",
                Email = "test@test.com"
            };

            context.Users.Add(user);
            context.SaveChanges();

            var result = context.Users.Find(1);

            Assert.NotNull(result);
            Assert.Equal("John Test", result.FullName);
        }

        // ✅ Test 4: Validate Incident → Reporter Relationship
        [Fact]
        public void Incident_Reporter_Relationship_Works()
        {
            var context = GetInMemoryDbContext();

            var user = new User
            {
                UserID = 10,
                FullName = "Reporter User"
            };

            var incident = new Incident
            {
                IncidentID = 20,
                Title = "Test Flood",
                ReportedBy = 10,
                Reporter = user
            };

            context.Users.Add(user);
            context.Incidents.Add(incident);
            context.SaveChanges();

            var savedIncident = context.Incidents
                .Include(i => i.Reporter)
                .FirstOrDefault(i => i.IncidentID == 20);

            Assert.NotNull(savedIncident);
            Assert.NotNull(savedIncident.Reporter);
            Assert.Equal("Reporter User", savedIncident.Reporter.FullName);
        }
    }
}
