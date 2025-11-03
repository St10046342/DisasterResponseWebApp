using Disaster_App.Models;
using System.Linq;
using Xunit;

namespace TestProjectDisasterApp.ModelTests
{
    public class UserModelTests
    {
        [Fact]
        public void User_ValidModel_PassesValidation()
        {
            var user = new User
            {
                UserID = 1,
                FullName = "Test User",
                Email = "test@test.com",
                PasswordHash = "hashedpassword",
                Role = "User"
            };

            var results = ValidationHelper.ValidateModel(user);

            Assert.Empty(results);
        }

        [Fact]
        public void User_MissingRequired_FailsValidation()
        {
            var user = new User(); // Missing name, email, role, password

            var results = ValidationHelper.ValidateModel(user);

            Assert.True(results.Count >= 3);
        }

        [Fact]
        public void User_InvalidEmail_FailsValidation()
        {
            var user = new User
            {
                FullName = "Test",
                Email = "wrongEmail",
                PasswordHash = "pass",
                Role = "User"
            };

            var results = ValidationHelper.ValidateModel(user);

            Assert.Contains(results, r => r.ErrorMessage.Contains("email"));
        }
    }
}
