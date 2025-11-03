using Disaster_App.Models;
using System.Linq;
using Xunit;

namespace TestProjectDisasterApp.ModelTests
{
    public class VolunteerTaskModelTests
    {
        [Fact]
        public void VolunteerTask_DefaultValues_AreSetCorrectly()
        {
            var task = new VolunteerTask();

            Assert.Equal("Open", task.Status);
            Assert.NotEqual(default, task.CreatedAt);
        }

        [Fact]
        public void VolunteerTask_MissingRequiredTaskName_Fails()
        {
            var task = new VolunteerTask();

            var results = ValidationHelper.ValidateModel(task);

            Assert.Contains(results, r => r.ErrorMessage.Contains("The Task Name field is required"));
        }

        [Fact]
        public void VolunteerTask_ValidModel_Passes()
        {
            var task = new VolunteerTask
            {
                TaskName = "Deliver Supplies",
                Status = "Open"
            };

            var results = ValidationHelper.ValidateModel(task);

            Assert.Empty(results);
        }
    }
}
