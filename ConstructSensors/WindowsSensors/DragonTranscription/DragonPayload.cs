using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonTranscription
{
    public class DragonPayload
    {
        private byte[] draFile;
        private string txtFile;

        public DragonPayload(byte[] theDraFile, string theTxtFile)
        {
            draFile = theDraFile;
            txtFile = theTxtFile;
        }

        public byte[] DraFile
        {
            get
            {
                return draFile;
            }
            set
            {
                draFile = value;
            }
        }

        public string TxtFile
        {
            get
            {
                return txtFile;
            }
            set
            {
                txtFile = value;
            }
        }
    }
}
