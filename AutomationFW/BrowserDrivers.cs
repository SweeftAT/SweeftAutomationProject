using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;

namespace AutomationFW
{
    public class BrowserDrivers
    {
        public IWebDriver GetWebDriver(BrowserType browserType)
        {
            return browserType switch
            {
                BrowserType.ChromeMobileLocal => LocalChromeMobile,
                BrowserType.ChromeMobileRemote => RemoteChromeMobile,
                BrowserType.ChromeDesktopLocal => LocalChromeDesktop,
                BrowserType.ChromeDesktopRemote => RemoteChromeDesktop,
                _ => throw new NotSupportedException("not supported browser: <null>"),
            };
        }

        private static ChromeOptions Options()
        {
            ChromeOptions options = new();
            options.AddArgument("--disable-notifications");
            options.AddArguments("--disable-infobars");
            options.AddUserProfilePreference("profile.default_content_setting_values.plugins", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            options.AddArguments("--ignore-certificate-errors");
            options.AddArguments("start-maximized");
            options.AddArguments("--disable-extensions");
            options.AddArgument("no-sandbox");
            options.AddArguments("--disable-popup-blocking");

            return options;

        }

        private static Uri RemoteWDAddress = new ("http://3.126.250.190:1234");

        private static IWebDriver RemoteChromeMobile
        {
            get
            {
                ChromeOptions options = Options();
                //options.AddArguments("--headless");
                options.EnableMobileEmulation("iPhone X");
                return new RemoteWebDriver(RemoteWDAddress, options);
            }
        }



        private static IWebDriver RemoteChromeDesktop
        {
            get
            {
                ChromeOptions options = Options();
                //options.AddArguments("--headless");
                return new RemoteWebDriver(RemoteWDAddress, options);
            }
        }

        private static IWebDriver LocalChromeMobile
        {
            get
            {
                ChromeOptions options = Options();
                //options.AddArguments("--headless");
                options.EnableMobileEmulation("iPhone X");
                return new RemoteWebDriver(new Uri("http://localhost:4444"), options);
            }
        }

        private static IWebDriver LocalChromeDesktop
        {
            get
            {
                ChromeOptions options = Options();
                //options.AddArguments("--headless");
                return new RemoteWebDriver(new Uri("http://localhost:4444"), options);

            }
        }
    }

    public enum BrowserType
    {
        ChromeMobileLocal,
        ChromeMobileRemote,
        ChromeDesktopLocal,
        ChromeDesktopRemote,
    }
}
