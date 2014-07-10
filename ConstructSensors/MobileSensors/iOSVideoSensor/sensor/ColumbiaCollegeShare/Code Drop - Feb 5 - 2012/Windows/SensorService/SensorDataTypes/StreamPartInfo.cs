using System;
using System.Collections.Generic;
using System.Linq;


namespace SensorDataTypes
{
    public class StreamPartInfo
    {
        public string SensorID { get; set; }
        public uint SequenceNumber { get; set; }
        public DateTime DeliveryStartTime { get; set; }
        public DateTime RecordingStartTime { get; set; }
        public DateTime RecordingEndTime { get; set; }
        public string FileName { get; set; }
        public string Base64Bytes { get; set; }
    }

    public class StreamPartInfoSorter : IComparer<StreamPartInfo>
    {

        public int Compare(StreamPartInfo x, StreamPartInfo y)
        {
            return x.SequenceNumber.CompareTo(y.SequenceNumber);
        }
    }
}