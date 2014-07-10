using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Timers;
using Newtonsoft.Json;
using Construct.Sensors;
using Construct.MessageBrokering;

namespace Construct.Sensors.MosbeLoggerSensor
{
    public class MosbeLoggerSensor : Sensor
    {
        // We may want to investigate changing this to NamedPipes, if it works there is less mucking with bytes, and ive read that
        //  there is also less overhead than TCP.
        TcpClient client;
        NetworkStream netStream;
        Timer streamTimer;
        FileStream loggerFileStream;
        byte[] loggerResponse = null;
        Process javaProcess = null;

        public MosbeLoggerSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("1DB52049-4BD1-46F3-A6C8-C7790AA8B117"), new Dictionary<string, Guid> { { "", Guid.Empty } })
        {
            loggerResponse = new byte[512];
            broker.OnCommandReceived += broker_OnCommandReceived;

            javaProcess = new Process();
            string javaArgs = GetJavaPath();
            if (javaArgs == null)
            {
                EventLog.WriteEntry("MosbeLoggerSensor", "WARNING:Could not locate java via JAVA_HOME enviornmental variable, in C:\\Program Files\\Java, or in C:\\Program Files (x86)\\Java");
            }
            else
            {
                javaProcess.StartInfo.FileName = javaArgs;
            }

            javaProcess.StartInfo.Arguments = "-cp \"HLA_logger.jar;portico.jar\" HLA_Logger";
            javaProcess.StartInfo.UseShellExecute = false;
            javaProcess.StartInfo.RedirectStandardError = true;
            loggerFileStream = File.Create(Path.Combine(Environment.CurrentDirectory, "Log.txt"));

            streamTimer = new Timer(5000);
            streamTimer.Enabled = false;
            //streamTimer.AutoReset = true;
            streamTimer.Elapsed += new ElapsedEventHandler(OnStreamTimerElapsed);

            FileInfo executingPath = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string executingDir = executingPath.DirectoryName;
            Directory.SetCurrentDirectory(executingDir);
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                default:
                    break;
            }
        }

        protected override string Start()
        {
            try
            {
                javaProcess.Start();
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("MosbeLoggerSensor", e.Message);
            }
            try
            {
                if (client == null)
                {
                    client = new TcpClient(Dns.GetHostName(), 4304);
                }
            }
            catch (SocketException e)
            {
                EventLog.WriteEntry("MosbeLoggerSensor", e.Message);
            }

            netStream = client.GetStream();
            streamTimer.Enabled = true;

            return base.Start();
        }

        private void OnStreamTimerElapsed(object source, EventArgs e)
        {
            if (netStream.DataAvailable)
            {
                int bytesRead = netStream.Read(loggerResponse, 0, loggerResponse.Length);
                byte[] consolePrintBytes = new byte[1024];
                char[] consolePrintChars = new char[512];
                for (int i = 0; i < bytesRead; ++i)
                {
                    consolePrintBytes[2 * i] = loggerResponse[i];
                    consolePrintChars[i] = BitConverter.ToChar(consolePrintBytes, (2 * i));
                }
                loggerFileStream.Write(consolePrintBytes, 0, (bytesRead * 2));
                loggerFileStream.Flush();

                //parse some string/byte array and extract strings to put into Construct Items
                String rawString = new String(consolePrintChars, 0, 512);
                SendItem(rawString.Substring(0, rawString.IndexOf('\0')), "MosbeLoggerItem", DateTime.Now);
            }
        }

        protected override string Stop()
        {
            streamTimer.Enabled = false;
            javaProcess.Close();

            return base.Stop();
        }

        private string GetJavaPath()
        {
            string javaPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            DirectoryInfo javaInfo = null;
            FileInfo[] files;

            if (javaPath != null)
            {
                javaInfo = new DirectoryInfo(Path.Combine(javaPath, "bin"));
                files = javaInfo.GetFiles("java.exe");
                if (javaInfo != null && files.Count() == 1)
                {
                    return Path.Combine(javaInfo.FullName, files[0].Name);
                }
            }

            javaPath = @"C:\Program Files (x86)\Java\jre6\bin";
            javaInfo = new DirectoryInfo(javaPath);
            files = javaInfo.GetFiles("java.exe");
            if (javaInfo != null && files.Count() == 1)
            {
                return Path.Combine(javaPath, files[0].Name);
            }

            javaPath = @"C:\Program Files\Java\jre6\bin";
            javaInfo = new DirectoryInfo(javaPath);
            files = javaInfo.GetFiles("java.exe");
            if (javaInfo != null && files.Count() == 1)
            {
                return Path.Combine(javaPath, files[0].Name);
            }

            return null;
        }
    }
}
