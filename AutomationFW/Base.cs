using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace AutomationFW
{
    public static class Base
    {
        public static DefaultWait<IWebDriver> Wait(IWebDriver driver, int timer = 5000)
        {
            return new DefaultWait<IWebDriver>(driver)
            {
                PollingInterval = TimeSpan.FromMilliseconds(timer / 10),
                Timeout = TimeSpan.FromMilliseconds(timer)
            };
        }

        public static bool ElementVanished(IWebDriver driver, string locator, int timer = 5000)
        {
            try
            {
                Wait(driver, timer).Until(de => de.FindElements(GetSelector(locator)).Count == 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void WaitFewSeconds(IWebDriver driver, int timer = 5000)
        {
            try
            {
                Wait(driver, timer).Until(dr => false);
            }
            catch
            {

            }
        }

        public static void WaitUntilUrlChanges(IWebDriver driver, string currentUrl, int timer = 5000)
        {
            try
            {
                Wait(driver, timer).Until(dr => dr.Url != currentUrl);
            }
            catch
            {

            }
        }

        public static void ClickToElement(IWebDriver driver, string locator, string errorMessage = "Object not found", int timer = 10000)
        {
            Actions action = new(driver);
            By selector = GetSelector(locator);
            IWebElement element;
            try
            {
                Wait(driver, timer).Until(de => de.FindElements(selector).Count > 0);
                Wait(driver, timer).Until(de => de.FindElement(selector).Enabled);
                element = driver.FindElement(selector);
                action.MoveToElement(element).Perform();
                action.Click(element).Perform();
            }
            catch
            {
                throw new Exception(errorMessage);
            }
        }

        public static IWebElement FindElement(IWebDriver driver, string locator, string errorMessage = "Object Not Found", int timer = 10000)
        {
            Actions action = new(driver);
            By selector = GetSelector(locator);
            IWebElement elemen;
            try
            {
                Wait(driver, timer).Until(de => FindElements(de, locator).Count > 0);
                elemen = driver.FindElement(selector);
                action.MoveToElement(elemen).Perform();
                return elemen;
            }
            catch
            {
                throw new Exception(errorMessage);
            }

        }

        public static void MoveMouseAway(IWebDriver driver)
        {
            try
            {
                Actions action = new(driver);
                IWebElement element = driver.FindElement(By.XPath("/html/body"));
                action.MoveToElement(element).Perform();
            }
            catch (Exception)
            {

            }
        }

        public static void MoveMouseToElement(IWebDriver driver, string locator)
        {
            Actions action = new(driver);
            By by = GetSelector(locator);
            IWebElement element = driver.FindElement(by);
            action.MoveToElement(element).Perform();
        }

        public static void SendKeys(IWebDriver driver, string locator, string text, string errorMessage = "Object not found", int timer = 5000)
        {
            try
            {
                ClearField(driver, locator, errorMessage, timer);
                FindElement(driver, locator, errorMessage, timer).SendKeys(text);
            }
            catch
            {
                throw new Exception(errorMessage);
            }
        }

        public static void ClearField(IWebDriver driver, string locator, string errorMessage = "Object not found", int timer = 5000) => FindElement(driver, locator, errorMessage, timer).Clear();

        public static bool ElementExists(IWebDriver driver, string locator, int timer = 10000)
        {
            try
            {
                Wait(driver, timer).Until(de => FindElements(driver, locator).Count > 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void DragAndDrop(IWebDriver driver, string from, string to, string errorMessage = "Object not found", int timer = 5000)
        {
            Actions actions = new(driver);

            try
            {
                IWebElement source = FindElement(driver, from, timer: timer);
                IWebElement target = FindElement(driver, to, timer: timer);

                actions.DragAndDrop(source, target).Perform();
            }
            catch
            {
                throw new Exception(errorMessage);
            }
        }

        public static void FinishTest(IWebDriver driver)
        {
            driver.Quit();
            driver.Dispose();
        }
        public static void Scroll(IWebDriver driver, int by = 500)
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript($"window.scrollBy(0,{by});");
        }

        public static void GoToUrl(IWebDriver driver, string url) => driver.Navigate().GoToUrl(url);

        public static string GetTextContent(IWebDriver driver, string element)
        {
            IWebElement webElement = FindElement(driver, element);
            IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
            if (executor == null || webElement == null)
            {
                return string.Empty;    //returns empty string
            }
            else
            {
                return executor.ExecuteScript("return arguments[0].textContent;", webElement).ToString().Trim();
            }
        }

        public static bool ElementVisible(IWebDriver driver, string locator, string errorMessage = "Object Not Found")
        {
            try
            {
                return driver.FindElement(GetSelector(locator)).Displayed;
            }
            catch
            {
                throw new Exception(errorMessage);
            }
        }

        public static void WaitUntilElementVanishes(IWebDriver driver, string locator, int timer = 5000)
        {
            try
            {
                Wait(driver, timer).Until(de => FindElements(de, locator).Count == 0);
            }
            catch (Exception)
            {
            }
        }

        public static ReadOnlyCollection<IWebElement> FindElements(IWebDriver driver, string locator) =>
            driver.FindElements(GetSelector(locator));


        public static void SafeClickToElement(IWebDriver driver, string locator, int timer = 5000)
        {
            Actions action = new(driver);
            By selector = GetSelector(locator);
            IWebElement element;
            try
            {
                Wait(driver, timer).Until(de => de.FindElements(selector).Count > 0);
                Wait(driver, timer).Until(de => FindElement(de, locator, timer: timer).Enabled);
                element = driver.FindElement(selector);
                action.MoveToElement(element).Perform();
                action.Click(element).Perform();
            }
            catch
            {
            }
        }

        public static void OpenNewTab(IWebDriver driver, string url)
        {
            _ = ((IJavaScriptExecutor)driver).ExecuteScript(string.Format($"window.open('{url}', '_blank');"));
            driver.SwitchTo().Window(driver.WindowHandles[1]);
        }

        public static void OpenUrlInNewTab(IWebDriver driver, string url)
        {
            driver.SwitchTo().NewWindow(WindowType.Tab);
            GoToUrl(driver, url);   
        }

        public static void OpenNewTab(IWebDriver driver) => driver.SwitchTo().NewWindow(WindowType.Tab);

        public static void CloseTab(IWebDriver driver)
        {
            //((IJavaScriptExecutor)driver).ExecuteScript(string.Format("window.close();"));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);
        }

        public static By GetSelector(string locator) => (locator.StartsWith('/') || locator.StartsWith('(')) ? By.XPath(locator) : By.CssSelector(locator);       

        public static void Reload(IWebDriver driver) => driver.Navigate().Refresh();

        public static void HardReload(IWebDriver driver)
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript("location.reload(true);");
        }

        public static void OpenHomePage(IWebDriver driver, string branch) => GoToUrl(driver, $"https://{branch}.adjarabet.com");

        public static void MaximizeWindow(IWebDriver driver) => driver.Manage().Window.Maximize();

        public static int TabCount(IWebDriver driver) => driver.WindowHandles.Count;

        public static void SwitchToFrame(IWebDriver driver, string iFrame) => driver.SwitchTo().Frame(FindElement(driver, iFrame));

        public static void SwitchToFrame(IWebDriver driver, IWebElement iFrame) => driver.SwitchTo().Frame(iFrame);

        public static void SwitchToDefaultFrame(IWebDriver driver) => driver.SwitchTo().DefaultContent();

        public static bool CheckUrl(IWebDriver driver, string url) => driver.Url.Contains(url);

        public static void SwitchToTab(IWebDriver driver, int to = 1) => driver.SwitchTo().Window(driver.WindowHandles[to]);

        public static Bitmap TakeAScreenshot(IWebDriver driver)
        {
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            return new Bitmap(new MemoryStream(screenshot.AsByteArray));
        }

        public static string UploadImage(IWebDriver driver)
        {
            try
            {
                if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\Screenshot\\"))
                    Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\Screenshot\\");
                string imageName = $"{DateTime.Now:yyyy-MM-dd-HH_mm_ss}.{ScreenshotImageFormat.Jpeg}";
                string filePath =
                    $"{Directory.GetCurrentDirectory()}\\Screenshot\\{imageName}";
                //sweefttest@gmail.com
                //Paroli12#

                Account account = new(
                    "dqaxd8l1h",
                    "321895723619388",
                    "EiUUgTbi2f-m0sGxCcQOsu8VFFo");

                Cloudinary cloudinary = new(account);
                Bitmap imageFile = TakeAScreenshot(driver);
                imageFile.Save(filePath);
                ImageUploadParams uploadParams = new()
                {
                    File = new FileDescription(filePath)
                };
                var uploadResult = cloudinary.Upload(uploadParams);
                Uri uploadURL = uploadResult.Url;
                if (uploadURL == null)
                    return "Couldn't upload screenshot.";

                if (File.Exists(filePath))
                    File.Delete(filePath);

                return uploadResult.Url.ToString();
            }
            catch (Exception e)
            {
                return $"Couldn't upload screenshot {e.Message}";
            }
        }

        public static void SendMail(string Subject, string Attachment, string email)
        {
            MailMessage mail = new()
            {
                From = new MailAddress("sweefttest@gmail.com"),
                Subject = Subject,
                Body = "Please download and open attached file."
            };

            SmtpClient SmtpServer = new("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("sweefttest@gmail.com", "Paroli1#"),
                EnableSsl = true
            };

            mail.To.Add(email);
            mail.Attachments.Add(new Attachment(Attachment));
            SmtpServer.Send(mail);

        }
    }
}
