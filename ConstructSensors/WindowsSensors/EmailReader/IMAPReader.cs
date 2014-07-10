using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Imap4;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Security;

namespace Email
{
    public class IMAPReader
    {
        private Imap4Client 
            imapClient;
        private Mailbox 
            targetMailbox;
        private string
            imapServerHostName,
            password,
            targetMailboxName;
        private int
            imapServerPort;
        private bool
            isSSL;
        private IMAPListener imapListener;

        public event EventHandler
            NewMessageReady,
            NewExceptionTelemetry;
        public int 
            lastMessageRecievedNumber;
        public string
            userName,
            currentMessageText,
            currentMessageSenderName,
            currentMessgeSenderEmail,
            currentMessageSenderIP,
            currentMessageDateUTC,
            currentMessageSubject;
        public List<byte[]>
            attachmentList;
        public List<string>
            mailboxes;

        public Email
            currentMessage;

        public IMAPReader(string ImapServerHostName, int ImapServerPort, string UserName, string Password, bool IsSSL, string TargetMailboxName = "inbox")
        {
            imapServerHostName = ImapServerHostName;
            imapServerPort = ImapServerPort;
            userName = UserName;
            password = Password;
            targetMailboxName = TargetMailboxName;
            isSSL = IsSSL;
            mailboxes = new List<string>();
            attachmentList = new List<byte[]>();
        }

        public void Initialize(IMAPListener ImapListener)
        {
            imapListener = ImapListener;

            InitializeIMAPConnection();
        }

        private void InitializeIMAPConnection()
        {
 	        imapClient = new Imap4Client();
            imapListener.NewMessageReceived += new EventHandler(imapListener_NewMessageRecieved);

            if(isSSL)
            {
                imapClient.ConnectSsl(imapServerHostName, imapServerPort);
            }
            else
            {
                imapClient.Connect(imapServerHostName, imapServerPort);
            }

            imapClient.Login(userName, password);
            
            foreach (Mailbox mailbox in imapClient.Mailboxes)
            {
                mailboxes.Add(mailbox.Name);
            }

            targetMailbox = imapClient.SelectMailbox(targetMailboxName);
        }

        private void imapListener_NewMessageRecieved(object sender, EventArgs e)
        {
            ReadEmail(lastMessageRecievedNumber);
        }

        public void ReadEmail(int messageCount)
        {
            currentMessage = new Email();
            try
            {
                Message message = targetMailbox.Fetch.MessageObject(messageCount);
                foreach(MimePart mimePart in message.Attachments)
                {
                    attachmentList.Add(mimePart.GetBinaryContent());
                }
                currentMessage.MessageText = message.BodyText.Text;
                currentMessage.MessageSenderName = message.Sender.Name;
                currentMessage.MessageSenderEmail = message.Sender.Email;
                currentMessage.MessageSenderIP = message.SenderIP.ToString();
                currentMessage.MessageDateUTC = message.Date.ToUniversalTime().ToString();
                currentMessage.MessageSubject = message.Subject;
                NewMessageReady(this, EventArgs.Empty);
            }
            catch (Imap4Exception iex)
            {
                NewExceptionTelemetry(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                NewExceptionTelemetry(null, EventArgs.Empty);
            }
            finally
            {
            }
        }
    }
}
