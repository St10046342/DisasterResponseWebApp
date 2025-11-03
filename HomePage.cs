using OpenQA.Selenium;

namespace DisasterApp.UITests.Pages
{
    public class HomePage
    {
        private readonly IWebDriver _driver;

        // Navigation elements
        private IWebElement LogIncidentLink => _driver.FindElement(By.LinkText("Log Incident"));
        private IWebElement LogDonationLink => _driver.FindElement(By.LinkText("Make Donation"));
        private IWebElement VolunteerLink => _driver.FindElement(By.LinkText("Become Volunteer"));
        private IWebElement LogoutLink => _driver.FindElement(By.LinkText("Logout"));
        private IWebElement WelcomeMessage => _driver.FindElement(By.CssSelector("h1, h2"));

        public HomePage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void GoToLogIncident()
        {
            LogIncidentLink.Click();
        }

        public void GoToLogDonation()
        {
            LogDonationLink.Click();
        }

        public void GoToVolunteerRegistration()
        {
            VolunteerLink.Click();
        }

        public void Logout()
        {
            LogoutLink.Click();
        }

        public bool IsUserHomePage()
        {
            return _driver.Url.Contains("UserHome") && WelcomeMessage.Displayed;
        }
    }
}