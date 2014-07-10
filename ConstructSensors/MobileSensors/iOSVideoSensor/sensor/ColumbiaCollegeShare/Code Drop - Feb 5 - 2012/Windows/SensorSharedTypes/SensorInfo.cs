using System;
using System.Collections.Generic;
using System.Linq;

namespace SensorSharedTypes
{
    public class SensorInfo
    {
        public enum StreamingResolution { None, High, Medium, Low }

        public SensorInfo()
        {
            LastStreamPartReceivedAt = DateTime.MinValue;
            LastCommandCheckAt = DateTime.MinValue;
            ResolutionAsInt = (int)StreamingResolution.None;
            PendingCommandAsInt = (int)SensorCommand.None;
        }

        public string SensorID { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastStreamPartReceivedAt { get; set; }
        public DateTime LastCommandCheckAt { get; set; }
        public bool IsConnected { get; set; }
        public int PendingCommandAsInt { get; set; }
        public int ResolutionAsInt { get; set; }

        public StreamingResolution GetStreamingResolution()
        {
            return (StreamingResolution)ResolutionAsInt;
        }

        public SensorCommand GetPendingCommand()
        {
            return (SensorCommand)PendingCommandAsInt;
        }

        public static SensorSorter GetSorter()
        {
            return new SensorSorter();
        }

        public class SensorSorter : IComparer<SensorInfo>
        {
            public int Compare(SensorInfo x, SensorInfo y)
            {
                int result = x.DisplayName.CompareTo( y.DisplayName );
                if (result == 0)
                {
                    return x.SensorID.CompareTo(y.SensorID);
                }
                else
                {
                    return result;
                }
            }
        }


    }
}