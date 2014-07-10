using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Construct.SensorUtilities
{
    public static class FileNamingUtility
    {
        public static string GetDecoratedMediaFileName(DateTime startedAt, DateTime endedAt, Location startLocation, string sensorName, string sensorID, string extension)
        {
            StringBuilder sb = new StringBuilder();

            // add the time interval (start + duration)
            double durationInMilliSeconds = endedAt.Subtract(startedAt).TotalMilliseconds;
            string startTimeText = DateTimeUtility.FormatTimeAndDurationAsIso8601(startedAt, durationInMilliSeconds);
            sb.AppendFormat("{0}", startTimeText);

            // add the first location (if any)
            string lat = "null";
            string lon = "null";
            string alt = "null";
            if (startLocation != null)
            {
                Location location1 = startLocation;
                lat = startLocation.LatitudeInDegrees.ToString();
                lon = startLocation.LongitudeInDegrees.ToString();
                alt = startLocation.AltitudeInMeters.ToString();
            }
            sb.AppendFormat("_{0}_{1}_{2}", lat, lon, alt);

            sb.AppendFormat("_{0}",sensorName);

            // add the sensor ID
            string sensorIDText = "null";
            if (string.IsNullOrEmpty(sensorID) == false)
            {
                sensorIDText = sensorID;
            }
            sb.AppendFormat("_{0}", sensorIDText);

            // add the extension
            string extensionText = "";
            if (string.IsNullOrEmpty(extension) == false)
            {
                extensionText = "." + extension.Trim().TrimStart('.');
            }
            sb.Append(extensionText);

            return sb.ToString();
        }

        
    }
}
