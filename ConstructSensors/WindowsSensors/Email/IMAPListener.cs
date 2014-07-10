using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Imap4;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Security;

namespace Email
{
    public class IMAPListener
    {
        private Imap4Client 
            imapClient;
        private Mailbox 
            targetMailbox;
        private string
            imapServerHostName,
            userName,
            password,
            targetMailboxName;
        private int
            imapServerPort;
        private bool
            isSSL;

        public event EventHandler
            NewMessageReceived;
        public int 
            lastMessageRecievedNumber;
        public IMAPReader imapReader;

        public IMAPListener(string ImapServerHostName, int ImapServerPort, string UserName, string Password, bool IsSSL, string TargetMailboxName = "inbox")
        {
            imapServerHostName = ImapServerHostName;
            imapServerPort = ImapServerPort;
            userName = UserName;
            password = Password;
            targetMailboxName = TargetMailboxName;
            isSSL = IsSSL;

            imapReader = new IMAPReader(imapServerHostName, imapServerPort, userName, password, isSSL, targetMailboxName);

            InitializeIMAPConnection();
        }

        private void InitializeIMAPConnection()
        {
 	        imapClient = new Imap4Client();
            imapClient.NewMessageReceived += new ActiveUp.Net.Mail.NewMessageReceivedEventHandler(imapClient_NewMessageRecieved);

            if(isSSL)
            {
                imapClient.ConnectSsl(imapServerHostName, imapServerPort);
            }
            else
            {
                imapClient.Connect(imapServerHostName, imapServerPort);
            }

            imapClient.Login(userName, password);
            
            targetMailbox = imapClient.SelectMailbox(targetMailboxName);

            imapReader.Initialize(this);

            imapClient.StartIdle();

        }

        public void imapClient_NewMessageRecieved(object source, ActiveUp.Net.Mail.NewMessageReceivedEventArgs e)
        { 
            lastMessageRecievedNumber = e.MessageCount;
            NewMessageReceived(null, EventArgs.Empty);
        }
    }
}
