using Disaster_App.Models;
using System.Linq;
using Xunit;

namespace TestProjectDisasterApp.ModelTests
{
    public class VolunteerModelTests
    {
        [Fact]
        public void Volunteer_MissingRequiredUserID_FailsValidation()
        {
            var volunteer = new Volunteer
            {
                Skills = "First Aid"
            };

            var results = ValidationHelper.ValidateModel(volunteer);

            Assert.Contains(results, r => r.ErrorMessage.Contains("The UserID field is required"));
        }

        [Fact]
        public void Volunteer_ValidModel_PassesValidation()
        {
            var volunteer = new Volunteer
            {
                VolunteerID = 1,
                UserID = 10,
                Skills = "CPR",
                Availability = "Weekends"
            };

            var results = ValidationHelper.ValidateModel(volunteer);

            Assert.Empty(results);
        }
    }
}
