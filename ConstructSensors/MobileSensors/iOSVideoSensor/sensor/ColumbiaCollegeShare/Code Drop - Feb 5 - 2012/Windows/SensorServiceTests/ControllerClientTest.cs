using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utilities;
using TestUtilities;
using SensorServiceTests;
using SensorSharedTypes;

namespace SensorServiceTests
{
    [TestClass()]
    public class ControllerClientTest
    {
        private SensorClient.SensorClient _sensorClient;
        private ControllerClient.ControllerClient _controllerClient;
        private string _sensorName = "Steve's Sensor";
        private string _sensorID;

        [TestInitialize()]
        public void TestInitialize()
        {
            createNewSensorClient();
            createNewControllerClient();
        }

        private void createNewSensorClient()
        {
            _sensorID = Guid.NewGuid().ToString();
            _sensorClient = new SensorClient.SensorClient( "http://localhost:8888", _sensorID, _sensorName, 600, 5000);
        }

        private void createNewControllerClient()
        {
            _controllerClient = new ControllerClient.ControllerClient("http://localhost:8888");
        }

        [TestMethod()]
        public void Ping()
        {
            string ping = Guid.NewGuid().ToString();
            string pong = _controllerClient.Ping(ping);
            Assert.AreEqual(ping, pong);
        }

        [TestMethod()]
        public void CanConnect()
        {
            Assert.IsTrue(_controllerClient.CanConnect());
        }


        [TestMethod()]
        public void GetSensors()
        {
            string message;

            _controllerClient.DropAllSensors();

            List<SensorInfo> sensors = _controllerClient.GetSensors();
            Assert.AreEqual( sensors.Count, 0 );

            for (int i = 0; i < 100; i++ )
            {
                createNewSensorClient();

                bool connected = _sensorClient.ConnectSensor(out message);
                Assert.IsTrue(connected);

                sensors = _controllerClient.GetSensors();
                SensorInfo sensor = sensors[i];
                Assert.AreEqual(sensors.Count, i + 1);
                Assert.AreEqual(sensor.DisplayName, _sensorName);
                Assert.IsTrue(sensor.IsConnected);
                Guid guid;
                Assert.IsTrue( Guid.TryParse(sensor.SensorID, out guid) );
                Assert.AreEqual(sensor.GetPendingCommand(), SensorCommand.None);
                Assert.AreEqual(sensor.LastStreamPartReceivedAt.Year, DateTime.MinValue.Year);
            }

            _controllerClient.DropAllSensors();

            sensors = _controllerClient.GetSensors();
            Assert.AreEqual(sensors.Count, 0);

        }

    }
}
