using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using Construct.Sensors;

namespace Email
{
    public struct Email
    {
        private string
            userName;
        private string
            messageSenderName;
        private string
            messageSenderEmail;
        private string
            messageSenderIP;
        private string
            messageDateUTC;
        private string
            messageSubject;
        private string
            messageText;

        private List<byte[]>
            attatchmentList;

        public string UserName
        {
            get { return userName; }

            set { userName = value; }
        }

        public string MessageSenderName
        {
            get { return messageSenderName; }

            set { messageSenderName = value; }
        }

        public string MessageSenderEmail
        {
            get { return messageSenderEmail; }

            set { messageSenderEmail = value; }
        }

        public string MessageSenderIP
        {
            get { return messageSenderIP; }

            set { messageSenderIP = value; }
        }

        public string MessageDateUTC
        {
            get { return messageDateUTC; }

            set { messageDateUTC = value; }
        }

        public string MessageSubject
        {
            get { return messageSubject; }

            set { messageSubject = value; }
        }

        public string MessageText
        {
            get { return messageText; }

            set { messageText = value; }
        }

        public List<byte[]> AttachmentList
        {
            get { return attatchmentList; }

            set { attatchmentList = value; }
        }

    }
}