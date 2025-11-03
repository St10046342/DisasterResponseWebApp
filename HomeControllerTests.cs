using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using DisasterApp.UITests.Pages;
using Xunit;

namespace DisasterApp.UITests
{
    public class HomeControllerTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly LoginPage _loginPage;
        private readonly HomePage _homePage;
        private readonly string _baseUrl = "https://localhost:7241"; // UPDATED TO YOUR URL

        public HomeControllerTests()
        {
            // Chrome driver setup
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-notifications");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            // Initialize page objects
            _loginPage = new LoginPage(_driver);
            _homePage = new HomePage(_driver);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Fact]
        public void Test_UserRegistrationAndLogin()
        {
            try
            {
                // Step 1: Go to registration page
                _driver.Navigate().GoToUrl(_baseUrl + "/Home/Register");

                // Fill registration form
                _driver.FindElement(By.Id("FullName")).SendKeys("Selenium Test User");
                _driver.FindElement(By.Id("Email")).SendKeys("selenium_test@example.com");
                _driver.FindElement(By.Id("PasswordHash")).SendKeys("Password123!");

                // Submit form
                _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                // Wait for redirect to login page
                _wait.Until(d => d.Url.Contains("Login"));

                // Verify success message
                Assert.Contains("Registration successful", _driver.PageSource);

                // Take screenshot for evidence
                TakeScreenshot("Registration_Success");
            }
            catch (Exception ex)
            {
                TakeScreenshot("Registration_Error");
                throw;
            }
        }

        [Fact]
        public void Test_UserLogin()
        {
            try
            {
                // Step 1: Navigate to login page
                _loginPage.NavigateToLogin();

                // Step 2: Login with credentials
                _loginPage.Login("testuser@example.com", "Password123!");

                // Step 3: Verify successful login
                _wait.Until(d => d.Url.Contains("UserHome"));
                Assert.Contains("Welcome", _driver.PageSource);

                TakeScreenshot("Login_Success");
            }
            catch (Exception ex)
            {
                TakeScreenshot("Login_Error");
                throw;
            }
        }

        [Fact]
        public void Test_LogIncident()
        {
            try
            {
                // Login first
                _loginPage.NavigateToLogin();
                _loginPage.Login("testuser@example.com", "Password123!");
                _wait.Until(d => d.Url.Contains("UserHome"));

                // Navigate to Log Incident
                _homePage.GoToLogIncident();
                _wait.Until(d => d.Title.Contains("Log Incident"));

                // Fill incident form
                _driver.FindElement(By.Id("Title")).SendKeys("Automated Test Incident");
                _driver.FindElement(By.Id("Description")).SendKeys("This incident was created by Selenium automated testing");
                _driver.FindElement(By.Id("Location")).SendKeys("Test Location");

                // Submit form
                _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                // Verify success
                _wait.Until(d => d.Url.Contains("UserHome"));
                Assert.Contains("Incident reported successfully", _driver.PageSource);

                TakeScreenshot("Incident_Logged");
            }
            catch (Exception ex)
            {
                TakeScreenshot("Incident_Error");
                throw;
            }
        }

        [Fact]
        public void Test_LogDonation()
        {
            try
            {
                // Login first
                _loginPage.NavigateToLogin();
                _loginPage.Login("testuser@example.com", "Password123!");
                _wait.Until(d => d.Url.Contains("UserHome"));

                // Navigate to Log Donation
                _homePage.GoToLogDonation();
                _wait.Until(d => d.Title.Contains("Log Donation"));

                // Fill donation form
                _driver.FindElement(By.Id("DonorName")).SendKeys("Selenium Donor");
                _driver.FindElement(By.Id("Email")).SendKeys("donor@test.com");
                _driver.FindElement(By.Id("Amount")).SendKeys("250.00");
                _driver.FindElement(By.Id("DonationType")).SendKeys("Money");

                // Submit form
                _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                // Verify success
                _wait.Until(d => d.Url.Contains("UserHome"));
                Assert.Contains("Thank you for your donation", _driver.PageSource);

                TakeScreenshot("Donation_Logged");
            }
            catch (Exception ex)
            {
                TakeScreenshot("Donation_Error");
                throw;
            }
        }

        [Fact]
        public void Test_VolunteerRegistration()
        {
            try
            {
                // Login first
                _loginPage.NavigateToLogin();
                _loginPage.Login("testuser@example.com", "Password123!");
                _wait.Until(d => d.Url.Contains("UserHome"));

                // Navigate to Volunteer Registration
                _homePage.GoToVolunteerRegistration();
                _wait.Until(d => d.Title.Contains("Volunteer"));

                // Fill volunteer form
                _driver.FindElement(By.Id("Skills")).SendKeys("First Aid, Communication");
                _driver.FindElement(By.Id("Availability")).SendKeys("Weekends");
                _driver.FindElement(By.Id("Location")).SendKeys("Test City");
                _driver.FindElement(By.Id("PhoneNumber")).SendKeys("1234567890");

                // Submit form
                _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                // Verify success
                _wait.Until(d => d.Url.Contains("UserHome"));
                Assert.Contains("Thank you for registering as a volunteer", _driver.PageSource);

                TakeScreenshot("Volunteer_Registration_Success");
            }
            catch (Exception ex)
            {
                TakeScreenshot("Volunteer_Registration_Error");
                throw;
            }
        }

        [Fact]
        public void Test_Navigation()
        {
            try
            {
                // Test all navigation links
                _driver.Navigate().GoToUrl(_baseUrl);

                string[] pagesToTest = { "Privacy", "Contact", "About" };

                foreach (var page in pagesToTest)
                {
                    _driver.FindElement(By.LinkText(page)).Click();
                    _wait.Until(d => d.Title.Contains(page) || d.Url.Contains(page));

                    TakeScreenshot($"Navigation_{page}");

                    // Go back to home
                    _driver.Navigate().GoToUrl(_baseUrl);
                }
            }
            catch (Exception ex)
            {
                TakeScreenshot("Navigation_Error");
                throw;
            }
        }

        [Fact]
        public void Test_Logout()
        {
            try
            {
                // Login first
                _loginPage.NavigateToLogin();
                _loginPage.Login("testuser@example.com", "Password123!");
                _wait.Until(d => d.Url.Contains("UserHome"));

                // Logout
                _homePage.Logout();

                // Verify redirect to login or home page
                _wait.Until(d => d.Url.Contains("Login") || d.Url.Contains("Index"));

                TakeScreenshot("Logout_Success");
            }
            catch (Exception ex)
            {
                TakeScreenshot("Logout_Error");
                throw;
            }
        }

        private void TakeScreenshot(string testName)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                string fileName = $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                screenshot.SaveAsFile(fileName);
                Console.WriteLine($"Screenshot saved: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not take screenshot: {ex.Message}");
            }
        }
    }
}