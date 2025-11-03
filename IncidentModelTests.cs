using Disaster_App.Models;
using System.Linq;
using Xunit;

namespace TestProjectDisasterApp.ModelTests
{
    public class IncidentModelTests
    {
        [Fact]
        public void Incident_DefaultDate_Works()
        {
            var incident = new Incident();
            Assert.NotEqual(default, incident.DateReported);
        }

        [Fact]
        public void Incident_MissingTitle_FailsValidation()
        {
            var incident = new Incident
            {
                Description = "Test Description",
                ReportedBy = 1
            };

            var results = ValidationHelper.ValidateModel(incident);

            Assert.Contains(results, r => r.ErrorMessage.Contains("The Title field is required"));
        }

        [Fact]
        public void Incident_ValidModel_PassesValidation()
        {
            var incident = new Incident
            {
                Title = "Flood",
                Description = "Severe rainfall",
                Location = "Cape Town",
                ReportedBy = 1
            };

            var results = ValidationHelper.ValidateModel(incident);

            Assert.Empty(results);
        }
    }
}
