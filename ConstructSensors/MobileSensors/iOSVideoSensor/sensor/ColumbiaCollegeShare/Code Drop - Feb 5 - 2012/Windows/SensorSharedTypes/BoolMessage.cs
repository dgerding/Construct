using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorSharedTypes
{
    public class BoolMessage
    {
        public BoolMessage() : this(false, "") 
        { 
        }

        public BoolMessage(bool result, string message)
        {
            this.Result = result;
            this.Message = message;
        }

        public bool Result { get; set; }
        public string Message { get; set; }
    }
}
