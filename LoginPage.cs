using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DisasterApp.UITests.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;

        // Page elements
        private IWebElement EmailField => _driver.FindElement(By.Id("Email"));
        private IWebElement PasswordField => _driver.FindElement(By.Id("PasswordHash"));
        private IWebElement LoginButton => _driver.FindElement(By.CssSelector("button[type='submit']"));
        private IWebElement RegisterLink => _driver.FindElement(By.LinkText("Register"));

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void NavigateToLogin()
        {
            _driver.Navigate().GoToUrl("https://localhost:7241/Home/Login");
        }

        public void Login(string email, string password)
        {
            EmailField.Clear();
            EmailField.SendKeys(email);

            PasswordField.Clear();
            PasswordField.SendKeys(password);

            LoginButton.Click();
        }

        public void GoToRegistration()
        {
            RegisterLink.Click();
        }

        public bool IsLoginPage()
        {
            return _driver.Url.Contains("Login") && _driver.Title.Contains("Login");
        }
    }
}