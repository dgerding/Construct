using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ServiceProcess;
using System.Linq;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace Construct.SensorHosts.WindowsSensorHost.Tests
{
    /// <summary>
    ///This is a test class for SensorHostWindowsServiceTest and is intended
    ///to contain all SensorHostWindowsServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SensorHostWindowsServiceTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for Installing the SensorHost
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Construct.SensorHosts.WindowsSensorHost.exe")]
        public void InstallSensorHostTest()
        {
            ServiceController[] services = ServiceController.GetServices();
            ServiceController sensorHostServiceHandle = services.Where(s => s.ServiceName == "Construct.SensorHost").SingleOrDefault();
            string constructServicePath = null;
            if (sensorHostServiceHandle != null)
            {
                Process uninstaller = new Process();

                try
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Construct.SensorHost");
                    constructServicePath = (string)key.GetValue("ImagePath");
                    uninstaller.StartInfo.FileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
                    uninstaller.StartInfo.Arguments = String.Format("/u {0}", constructServicePath);
                    uninstaller.Start();
                    uninstaller.WaitForExit();
                }
                catch
                {
                    EventLog.WriteEntry("Install Sensor Host Test", 
                        "I guess the registry look up failed while uninstalling? Check if \"Computer\\HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Construct.SensorHost\" actually exists");
                } 
            }

            try
            {
                Process installer = new Process();
                installer.StartInfo.FileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
                if (constructServicePath == null)
                {
                    if (File.Exists("C:\\CC2C\\Construct2\\Construct.SensorHosts\\WindowsSensorHost\\Construct.SensorHosts.WindowsSensorHost\\bin\\Debug\\Construct.SensorHosts.WindowsSensorHost.exe"))
                    {
                        constructServicePath = "C:\\CC2C\\Construct2\\Construct.SensorHosts\\WindowsSensorHost\\Construct.SensorHosts.WindowsSensorHost\\bin\\Debug\\Construct.SensorHosts.WindowsSensorHost.exe";
                    }
                    else if (File.Exists("E:\\CC2C\\Construct2\\Construct.SensorHosts\\WindowsSensorHost\\Construct.SensorHosts.WindowsSensorHost\\bin\\Debug\\Construct.SensorHosts.WindowsSensorHost.exe"))
                    {
                        constructServicePath = "E:\\CC2C\\Construct2\\Construct.SensorHosts\\WindowsSensorHost\\Construct.SensorHosts.WindowsSensorHost\\bin\\Debug\\Construct.SensorHosts.WindowsSensorHost.exe";
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                installer.StartInfo.Arguments = String.Format("{0}", constructServicePath);
                installer.Start();
                installer.WaitForExit();
            }
            catch
            {
                EventLog.WriteEntry("Install Sensor Host Test", "Reinstalling failed, probably because of permissions or trying to interact with textboxes on install");
            }

            ServiceController[] postServices = ServiceController.GetServices();
            ServiceController postSensorHostServiceHandle = postServices.Where(s => s.ServiceName == "Construct.SensorHost").SingleOrDefault();
            Assert.IsNotNull(postSensorHostServiceHandle);
        }
    }
}
