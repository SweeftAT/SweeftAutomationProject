using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using AutomationFW;
using static AutomationFW.Base;
namespace ChoiceTV.Tests
{
    [TestClass]
    public class BaseDesktopTest
    {
        public IWebDriver driver;
        public TestContext TestContext { get; set; }
        private BrowserType browserType;

        public string URL;

        [ClassInitialize]
        public void TestClassInit(TestContext testContext)
        {
            TestContext = testContext;
            URL = TestContext.Properties["URL"].ToString();
            browserType = TestContext.Properties["WebDriverLocation"].ToString() == "Remote" ? BrowserType.ChromeDesktopRemote : BrowserType.ChromeDesktopLocal;
        }


        [TestInitialize]
        public void StartTest()
        {
            BrowserDrivers browserDrivers = new();            
            driver = browserDrivers.GetWebDriver(browserType);
        }

        [TestCleanup]
        public void CloseBrowser()
        {
            FinishTest(driver);
        }
    }
}