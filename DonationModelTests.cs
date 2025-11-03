using Disaster_App.Models;
using System;
using System.Linq;
using Xunit;

namespace TestProjectDisasterApp.ModelTests
{
    public class DonationModelTests
    {
        [Fact]
        public void Donation_DefaultValues_AreSetCorrectly()
        {
            var donation = new Donation();

            Assert.Equal("Pending", donation.Status);
            Assert.NotEqual(default, donation.CreatedAt);
            Assert.NotEqual(default, donation.DonationDate);
        }

        [Fact]
        public void Donation_ValidModel_PassesValidation()
        {
            var donation = new Donation
            {
                DonationID = 1,
                DonorName = "John Doe",
                Email = "john@example.com",
                ResourceType = "Food",
                Quantity = 10,
                Description = "Canned food",
                ContactNumber = "1234567890",
                PickupAddress = "123 Street"
            };

            var results = ValidationHelper.ValidateModel(donation);

            Assert.Empty(results); // Should be valid
        }

        [Fact]
        public void Donation_MissingRequiredFields_FailsValidation()
        {
            var donation = new Donation(); // All required fields missing

            var results = ValidationHelper.ValidateModel(donation);

            Assert.True(results.Count >= 3); // DonorName, Email, ResourceType, Quantity
        }

        [Fact]
        public void Donation_InvalidEmail_FailsValidation()
        {
            var donation = new Donation
            {
                DonorName = "Test",
                Email = "invalid-email",
                ResourceType = "Food",
                Quantity = 1
            };

            var results = ValidationHelper.ValidateModel(donation);

            Assert.Contains(results, r => r.ErrorMessage.Contains("Invalid email"));
        }
    }
}
