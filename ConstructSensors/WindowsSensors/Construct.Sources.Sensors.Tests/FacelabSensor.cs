using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Construct.Sensors.Tests
{
    [TestClass]
    public class FacelabSensor
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void InitializeFacelabSensorTest()
        {
            FacelabSensor sensor = new FacelabSensor();

            Assert.IsNull(sensor);
        }
    }
}
