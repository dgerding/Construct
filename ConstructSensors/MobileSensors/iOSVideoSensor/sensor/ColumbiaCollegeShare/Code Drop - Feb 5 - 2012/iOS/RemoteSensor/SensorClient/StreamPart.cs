using System;
using System.Collections.Generic;
using System.Linq;

namespace SensorSharedTypes
{
    public class StreamPart
    {
        public string SensorID { get; set; }
        public string StreamID { get; set; }
        public uint SequenceNumber { get; set; }
        public DateTime StartTime { get; set; }
        public uint DurationMilliSeconds { get; set; }
        public string FileName { get; set; }
        public string Base64Bytes { get; set; }
        public bool IsLastPart { get; set; }
    }

    public class StreamPartInfoSorter : IComparer<StreamPart>
    {

        public int Compare(StreamPart x, StreamPart y)
        {
            return x.SequenceNumber.CompareTo(y.SequenceNumber);
        }
    }
}