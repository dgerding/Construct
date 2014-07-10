using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;
using System.IO;
using SensorSharedTypes;

namespace MobileVideoSensorService
{
    public class SensorAPIController : Controller
    {
        // GET: /SensorAPI/Ping?val={data}
        public JsonResult Ping(string val)
        {
            try
            {
                return this.Json(new BoolMessage( true, val ), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogAPI.LogSensorServiceException(ex);
                return null;
            }
        }

        // POST: /SensorAPI/UploadStreamPart
        [HttpPost]
        public JsonResult UploadStreamPart( StreamPart streamInfo )
        {
            try
            {
                assertSensorIsConnected( streamInfo.SensorID );
                SensorManager.UploadStreamPart(streamInfo);
                return this.Json(new BoolMessage(true, ""), JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                LogAPI.LogSensorServiceException(ex);
                return this.Json(new BoolMessage(false, ex.Message), JsonRequestBehavior.DenyGet);
            }
        }

        // GET: /SensorAPI/ConnectSensor?sensorID=xxx&displayName=yyy
        public JsonResult ConnectSensor( string sensorID, string displayName )
        {
            try
            {
                LogAPI.SensorServiceLog.InfoFormat("sensorID: {0} with name of {1} was connected", sensorID, displayName);
                SensorManager.ConnectSensor(sensorID, displayName);
                return this.Json(new BoolMessage(true, ""), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogAPI.LogSensorServiceException(ex);
                return this.Json(new BoolMessage(false, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /SensorAPI/DisconnectSensor?sensorID=xxx
        public JsonResult DisconnectSensor(string sensorID)
        {
            try
            {
                LogAPI.SensorServiceLog.InfoFormat("sensorID: {0} was disconnected", sensorID);
                SensorManager.DisconnectSensor(sensorID);
                return this.Json(new BoolMessage(true, ""), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogAPI.LogSensorServiceException(ex);
                return this.Json(new BoolMessage(false, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /SensorAPI/GetSensors
        public JsonResult GetSensors()
        {
            try
            {
                List<SensorInfo> sensors = SensorManager.GetSensors();
                return this.Json(sensors, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogAPI.LogSensorServiceException(ex);
                return this.Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /SensorAPI/GetSensors
        public JsonResult DropAllSensors()
        {
            try
            {
                SensorManager.DropAllSensors();
                return this.Json(new BoolMessage(true, ""), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogAPI.LogSensorServiceException(ex);
                return this.Json(new BoolMessage(false, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /SensorAPI/GetSensorCommand?sensorID=xxx
        public JsonResult GetSensorCommand(string sensorID)
        {
            try
            {
                assertSensorIsConnected(sensorID);
                SensorCommand command = SensorManager.GetSensorCommand(sensorID);
                string commandText = command.ToString();
                return this.Json(new BoolMessage(true, commandText), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogAPI.LogSensorServiceException(ex);
                return this.Json(new BoolMessage(false, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /SensorAPI/SetSensorCommand?sensorID=xxx
        public JsonResult SetSensorCommand(string sensorID, string sensorCommand)
        {
            try
            {
                if (string.IsNullOrEmpty(sensorCommand) == true)
                {
                    SensorManager.SetSensorCommand(sensorID, SensorCommand.None);
                    return this.Json(new BoolMessage(true, "None"), JsonRequestBehavior.AllowGet);
                }

                SensorCommand command = SensorCommand.None;
                foreach ( SensorCommand value in Enum.GetValues( typeof(SensorCommand) ) )
                {
                    if ( string.Compare( value.ToString(), sensorCommand, true ) == 0 )
                    {
                        command = value;
                        break;
                    }
                }
                if (command == SensorCommand.None)
                {
                    SensorManager.SetSensorCommand(sensorID, command);
                    return this.Json(new BoolMessage(false, "unrecognized command"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    SensorManager.SetSensorCommand(sensorID, command);
                    string commandText = command.ToString();
                    return this.Json(new BoolMessage(true, commandText), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogAPI.LogSensorServiceException(ex);
                return this.Json(new BoolMessage(false, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        private void assertSensorIsConnected(string sensorID)
        {
            if ( SensorManager.IsSensorConnected( sensorID ) == false )
            {
                throw new InvalidOperationException( "NOT_CONNECTED");
            }
        }


    }
}
