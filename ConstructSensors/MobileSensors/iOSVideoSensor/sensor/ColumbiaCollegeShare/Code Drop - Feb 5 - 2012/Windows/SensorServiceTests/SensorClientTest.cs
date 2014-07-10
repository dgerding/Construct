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
    public class SensorClientTest
    {
        SensorClient.SensorClient _sensorClient;
        ControllerClient.ControllerClient _controllerClient;

        [TestInitialize()]
        public void TestInitialize()
        {
            _sensorClient = new SensorClient.SensorClient("http://localhost:8888", Guid.NewGuid().ToString(), "Steve's Sensor", 600, 5000);
            _controllerClient = new ControllerClient.ControllerClient("http://localhost:8888");
        }

        [TestMethod()]
        public void Ping()
        {
            string ping = Guid.NewGuid().ToString();
            string pong = _sensorClient.Ping(ping);
            Assert.AreEqual(ping, pong);
        }

        [TestMethod()]
        public void CanConnect()
        {
            Assert.IsTrue( _sensorClient.CanConnect() );
        }

        [TestMethod()]
        public void ConnectSensor()
        {
            string message;
            bool connected = _sensorClient.ConnectSensor(out message);
            Assert.IsTrue( connected );
        }

        [TestMethod()]
        public void DisconnectSensor()
        {
            string message;
            bool connected = _sensorClient.ConnectSensor(out message);
            Assert.IsTrue(connected);

            bool disconnected = _sensorClient.DisconnectSensor(out message);
            Assert.IsTrue(disconnected);

            SensorClient.SensorClient newClient = new SensorClient.SensorClient("http://localhost:8888", Guid.NewGuid().ToString(), "Test Sensor 2", 600, 5000);
            disconnected = newClient.DisconnectSensor(out message);
            Assert.IsTrue(disconnected);
            Assert.AreEqual(message, "");
        }


        [TestMethod()]
        public void CommandGetAndSet()
        {
            string message;
            bool connected = _sensorClient.ConnectSensor(out message);
            Assert.IsTrue(connected);

            SensorCommand command = _sensorClient.GetSensorCommand( out message );
            Assert.AreEqual( command, SensorCommand.None );
            Assert.AreEqual(message, "None");

            // set it and then confirm it via get
            bool sentIt;
            string sensorID = _sensorClient.SensorID;
            foreach ( SensorCommand commandToSend in Enum.GetValues(typeof(SensorCommand)) )
            {
                sentIt = _controllerClient.SetSensorCommand(commandToSend, sensorID, out message);
                Assert.IsTrue(sentIt);
                Assert.AreEqual(message, commandToSend.ToString());

                command = _sensorClient.GetSensorCommand(out message);
                Assert.AreEqual(message, command.ToString());

                command = _sensorClient.GetSensorCommand(out message);
                Assert.AreEqual(message, "None");
            }

            sentIt = _controllerClient.SetSensorCommand(SensorCommand.None, sensorID, out message);
            Assert.IsTrue(sentIt);
            Assert.AreEqual(message, "None");

            command = _sensorClient.GetSensorCommand(out message);
            Assert.AreEqual(message, "None");
        }



        [TestMethod()]
        public void UploadStreamPartTest_BeforeConnecting()
        {
            string streamID = Guid.NewGuid().ToString();

            var info = new StreamPart();
            info.StartTime = DateTime.Now;
            info.SensorID = _sensorClient.SensorID;
            info.StreamID = streamID;
            info.SequenceNumber = 0;
            info.IsLastPart = false;
            info.FileName = "some_made_up_file_name.mov";
            byte[] part = MovieAPI.GetMovieFragment();
            info.Base64Bytes = Convert.ToBase64String( part );
            _sensorClient.Uploader.AddPart(info);

            info = new StreamPart();
            info.StartTime = DateTime.Now;
            info.SensorID = _sensorClient.SensorID;
            info.StreamID = streamID;
            info.SequenceNumber = 1;
            info.IsLastPart = true;
            info.FileName = "some_made_up_file_name.mov";
            info.Base64Bytes = Convert.ToBase64String(MovieAPI.GetMovieFragment());
            _sensorClient.Uploader.AddPart(info);

            int numPartsCompleted = 0;
            int numUploadFails = 0;
            int numUploadDrops = 0;

            _sensorClient.Uploader.StreamPartUploaded += (s, e) =>
            {
                lock (this)
                {
                    numPartsCompleted++;
                }
            };
            _sensorClient.Uploader.StreamPartUploadFailed += (s, e) =>
            {
                lock (this)
                {
                    numUploadFails++;
                }
            };
            _sensorClient.Uploader.StreamPartUploadDropped += (s, e) =>
            {
                lock (this)
                {
                    numUploadDrops++;
                }
            };
            // even though we are not connected it should still pass because the uploader BG thread
            // should force the connect on its first failure
            _sensorClient.Uploader.Start();
            DateTime start = DateTime.Now;
            while (numPartsCompleted < 2)
            {
                if ( DateTime.Now.Subtract(start) > TimeSpan.FromSeconds(10) )
                {
                    Assert.Fail("Uploader did not upload stream parts in a timely fashion");
                }
                System.Threading.Thread.Sleep(100);
            }
            _sensorClient.Uploader.Stop();

            Assert.AreEqual( numPartsCompleted, 2 );
            Assert.AreEqual(numUploadFails, 0);
            Assert.AreEqual(numUploadDrops, 0);
            
        }

        [TestMethod()]
        public void UploadStreamPartTest_AfterConnecting()
        {
            string streamID = Guid.NewGuid().ToString();

            string message;
            bool connected = _sensorClient.ConnectSensor( out message );
            Assert.IsTrue( connected );

            var info = new StreamPart();
            info.StartTime = DateTime.Now;
            info.SensorID = _sensorClient.SensorID;
            info.StreamID = streamID;
            info.SequenceNumber = 0;
            info.IsLastPart = false;
            info.FileName = "some_made_up_file_name.mov";
            byte[] part = MovieAPI.GetMovieFragment();
            info.Base64Bytes = Convert.ToBase64String(part);
            _sensorClient.Uploader.AddPart(info);

            info = new StreamPart();
            info.StartTime = DateTime.Now;
            info.SensorID = _sensorClient.SensorID;
            info.StreamID = streamID;
            info.SequenceNumber = 1;
            info.IsLastPart = true;
            info.FileName = "some_made_up_file_name.mov";
            info.Base64Bytes = Convert.ToBase64String(MovieAPI.GetMovieFragment());
            _sensorClient.Uploader.AddPart(info);

            int numPartsCompleted = 0;
            int numUploadFails = 0;
            int numUploadDrops = 0;

            _sensorClient.Uploader.StreamPartUploaded += (s, e) =>
            {
                lock (this)
                {
                    numPartsCompleted++;
                }
            };
            _sensorClient.Uploader.StreamPartUploadFailed += (s, e) =>
            {
                lock (this)
                {
                    numUploadFails++;
                }
            };
            _sensorClient.Uploader.StreamPartUploadDropped += (s, e) =>
            {
                lock (this)
                {
                    numUploadDrops++;
                }
            };

            _sensorClient.Uploader.Start();
            DateTime start = DateTime.Now;
            while (numPartsCompleted < 2)
            {
                if (DateTime.Now.Subtract(start) > TimeSpan.FromSeconds(10))
                {
                    Assert.Fail("Uploader did not upload stream parts in a timely fashion");
                }
                System.Threading.Thread.Sleep(100);
            }
            _sensorClient.Uploader.Stop();

            Assert.AreEqual(numPartsCompleted, 2);
            Assert.AreEqual(numUploadFails, 0);
            Assert.AreEqual(numUploadDrops, 0);

        }



        [TestMethod()]
        public void CommandMonitoring()
        {

            string message;
            bool connected = _sensorClient.ConnectSensor(out message);
            Assert.IsTrue(connected);

            int numCommandsReceived = 0;
            SensorCommand commandReceived = SensorCommand.None;
            _sensorClient.CommandMonitor.SensorCommandReceived += (s, e) =>
            {
                lock (this)
                {
                    numCommandsReceived++;
                    commandReceived = e.Command;
                }
            };
            _sensorClient.CommandMonitor.Start();

            Assert.AreEqual(numCommandsReceived, 0);

            string sensorID = _sensorClient.SensorID;
            _controllerClient.SetSensorCommand(SensorCommand.StartUpload_MediumRes, sensorID, out message);

            DateTime start = DateTime.Now;
            while (numCommandsReceived < 1)
            {
                if (DateTime.Now.Subtract(start) > TimeSpan.FromSeconds(10))
                {
                    Assert.Fail("command monitor did not detect any commands in a timely fashion");
                }
                System.Threading.Thread.Sleep(100);
            }
            _sensorClient.CommandMonitor.Stop();

            Assert.AreEqual(numCommandsReceived, 1);
            Assert.AreEqual(commandReceived, SensorCommand.StartUpload_MediumRes);
        }




    }
}
