using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using DragonEnrollment;
using Alvas.Audio;
using Construct.Sensors;
using Construct.MessageBrokering;

namespace DragonEnrollment
{
    class DragonEnrollmentSensor : Sensor
    {
        private EnrollmentUtteranceGenerator
            enrollmentUtteranceGenerator;

        private byte[]
            convertedEnrollmentUtteranceFile;
        public string
            convertedEnrolmentTextFile;
        private object
            outputObject;
        private List<Rendezvous<Command>> 
            dragonTranscriptionRendezvousList;
        private EnrollmentObject
            enrollmentObject;
        public EventHandler
            onSensorStarted;

        public DragonEnrollmentSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("7A1B4884-FDB4-4514-8B93-F08C8CC7CCA9"), new Dictionary<string, Guid>() { { "DragonEnrollment", Guid.Parse("CE01713C-AE61-49E7-9F2E-D82EA1232C14") } })
        {
            dragonTranscriptionRendezvousList = new List<Rendezvous<Command>>();

            enrollmentUtteranceGenerator = new EnrollmentUtteranceGenerator();
            enrollmentUtteranceGenerator.OnFileCompleted += new EventHandler(this.enrollmentUtteranceGenerator_OnFileCompleted);

            broker.OnCommandReceived += broker_OnCommandReceived;
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    broker.Publish(new Telemetry("Enrollment ItemOutbox Added  " + Environment.MachineName, new Dictionary<string,string>()));
                    break;
                case "AddCommandOutbox":
                    Rendezvous<Command> commandRendezvous = new Rendezvous<Command>(command.Args["Uri"]);
                    if (command.Args["Uri"].Contains("1027ab45-7059-4bd0-84f0-ed2ae472b70a"))
                    {
                        dragonTranscriptionRendezvousList.Add(commandRendezvous);
                    }
                    broker.AddPeer(new Outbox<Command>(commandRendezvous));
                    break;
                default:
                    break;
            }
        }

        public void IssueCloseSpeakerCommand()
        {
            try
            {
                Dictionary<string, string> closeSpeakerArgs = new Dictionary<string, string>();
                closeSpeakerArgs.Add("BrokerID", broker.BrokerID.ToString());

                Command closeSpeaker = new Command("CloseSpeakerProfile", closeSpeakerArgs);

                foreach (Rendezvous<Command> commandRendezvous in dragonTranscriptionRendezvousList)
                {
                    broker.PublishToInbox(commandRendezvous, closeSpeaker);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ConvertFileToByteArray(string EnrollmentUtteranceFilePath)
        {
            WaveReader waveReader = new WaveReader(File.OpenRead(EnrollmentUtteranceFilePath));
            convertedEnrollmentUtteranceFile = waveReader.ReadData();
            enrollmentObject = new EnrollmentObject();
            enrollmentObject.utterance = convertedEnrollmentUtteranceFile;
            enrollmentObject.textFile = convertedEnrolmentTextFile;
            outputObject = enrollmentObject;
        }

        private void enrollmentUtteranceGenerator_OnFileCompleted(object sender, EventArgs e)
        {
            ConvertFileToByteArray(enrollmentUtteranceGenerator.conversionFile);
            SendItem(outputObject as object, "DragonEnrollment", DateTime.Now);
        }

        public void BeginRecording()
        {
            enrollmentUtteranceGenerator.Recorder.StartRecord();
        }

        protected override string Start()
        {
            onSensorStarted(null, EventArgs.Empty);

            return base.Stop();
        }

        protected override string Stop()
        {
            enrollmentUtteranceGenerator.Recorder.StopRecord();

            return base.Stop();
        }

        public void StopRecording()
        {
            enrollmentUtteranceGenerator.Recorder.StopRecord();
        }

        public void PauseRecording()
        {
            enrollmentUtteranceGenerator.Recorder.PauseRecord();
            enrollmentUtteranceGenerator.rex_Close(null, EventArgs.Empty);
        }
    }
}
