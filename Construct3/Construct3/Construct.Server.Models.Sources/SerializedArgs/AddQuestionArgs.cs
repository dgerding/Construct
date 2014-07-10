using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Models.Sources
{
    [DataContract]
    public class AddQuestionArgs
    {
        public AddQuestionArgs()
        {
        }

        public AddQuestionArgs(string temp)
        {
            Temp = temp;
        }

        [DataMember]
        public string Temp
        {
            get;
            set;
        }
    }
}