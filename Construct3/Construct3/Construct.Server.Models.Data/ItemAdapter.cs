using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Models
{
    [DataContract]
    public class ItemAdapter
    {
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public DateTime RecordCreationDate { get; set; }
        [DataMember]
        public Dictionary<string, string> Strings { get; set; }
        [DataMember]
        public Dictionary<string, int> Ints { get; set; }
        [DataMember]
        public Dictionary<string, Guid> Guids { get; set; }
        [DataMember]
        public Dictionary<string, bool> Bools { get; set; }
        [DataMember]
        public Dictionary<string, double> Doubles { get; set; }
        [DataMember]
        public Dictionary<string, float> Floats { get; set; }
        [DataMember]
        public Dictionary<string, byte[]> Bytes { get; set; }
    }
}