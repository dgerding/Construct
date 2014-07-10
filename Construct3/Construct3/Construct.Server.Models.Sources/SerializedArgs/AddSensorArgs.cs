using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Construct.MessageBrokering;
using System.Runtime.Serialization;

namespace Construct.Server.Models.Sources
{
    [DataContract]
    public class AddSensorArgs
    {
        public AddSensorArgs()
        {
        }

        public AddSensorArgs(string downloadUri, string zippedFileName, string humanName, string version, string overwrite)
        {
            DownloadUri = downloadUri;
            ZippedFileName = zippedFileName;
            HumanName = humanName;
            Version = version;
            Overwrite = overwrite;
        }

        [DataMember]
        public string DownloadUri
        {
            get;
            set;
        }

        [DataMember]
        public string ZippedFileName
        {
            get;
            set;
        }

        [DataMember]
        public string HumanName
        {
            get;
            set;
        }


        [DataMember]
        public string Version
        {
            get;
            set;
        }

        [DataMember]
        public string Overwrite
        {
            get;
            set;
        }

    }
}