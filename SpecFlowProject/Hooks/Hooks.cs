using AutomationFW;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using SpecFlowProject.Helpers.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using static AutomationFW.Base;

[assembly: Parallelize(Scope = ExecutionScope.ClassLevel, Workers = 0)]
namespace SpecFlowProject.Hooks
{
    [Binding]
    public sealed class Hooks
    {
        [ThreadStatic] BrowserDrivers browserDrivers;
        [ThreadStatic] private readonly ISpecFlowOutputHelper specFlowOutputHelper;
        [ThreadStatic] ScenarioContext scenarioContext;
        IWebDriver driver;
        static bool RemoteSession;
        static bool TakeScreenshot;
        static ConfigurationBuilder builder;
        static IConfigurationRoot configuration;
        static TestConfig testConfig;
        static string HomePageUrl;


        public Hooks(BrowserDrivers browserDrivers, ISpecFlowOutputHelper specFlowOutputHelper, ScenarioContext scenarioContext)
        {
            this.browserDrivers = browserDrivers;
            this.specFlowOutputHelper = specFlowOutputHelper;
            this.scenarioContext = scenarioContext;
            this.scenarioContext.Add("HomePage", HomePageUrl);
        }

        [BeforeTestRun]
        public static void TestConfiguration()
        {
            builder = new();
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent;
            builder.AddJsonFile($"{path}/TestConfig.json");
            configuration = builder.Build();

            testConfig = new();
            configuration.Bind(testConfig);
            RemoteSession = testConfig.RemoteSession;
            TakeScreenshot = testConfig.TakeaScreenshot;
            HomePageUrl = testConfig.HomePageUrl;            
        }

        [BeforeScenario("Mobile")]
        public void BeforeMobileScenario()
        {

            driver = RemoteSession ?
                browserDrivers.GetWebDriver(BrowserType.ChromeMobileRemote) :
                browserDrivers.GetWebDriver(BrowserType.ChromeMobileLocal);

            scenarioContext.Add("CurrentDriver", driver);

        }

        [BeforeScenario("Desktop")]
        public void BeforeDesktopScenario()
        {
            driver = RemoteSession ?
                browserDrivers.GetWebDriver(BrowserType.ChromeDesktopRemote) :
                browserDrivers.GetWebDriver(BrowserType.ChromeDesktopLocal);

            scenarioContext.Add("CurrentDriver", driver);
        }


        [AfterScenario]
        public void AfterScenario()
        {
            FinishTest(driver);
        }

        [AfterStep]
        public void TakeScreenshotAfterStep()
        {
            if (TakeScreenshot)
                if (scenarioContext.TestError != null)
                    specFlowOutputHelper.AddAttachment(UploadImage(driver));
        }
    }
}
