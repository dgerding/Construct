using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json;
using Alvas.Audio;
using Construct.Sensors;
using Construct.MessageBrokering;
using System.Windows.Forms;

namespace Utterance
{
    public class UtteranceSensor : Sensor
    {
        public UtteranceGenerator
            utteranceGenerator;

        private readonly Dictionary<string, string>
            availableCommands;

		private DateTime
			sensorStartTime;

		/*
		 * NOTE: A formal "Utterance" type exists but is unused due to lack of formal support for item deserialization in sensors. This is relevant
		 *		for the SAPI sensor which ingests utterances
		 */

        public UtteranceSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("3fe979d6-6d8c-4954-97cd-d49b43323ae6"), new Dictionary<string, Guid>() { { "Utterance", Guid.Parse("59D0F230-164C-4033-B48A-220FC36C5FBD") } })
        {
            utteranceGenerator = new UtteranceGenerator();

            availableCommands = new Dictionary<string, string>();

            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();

            utteranceGenerator.SetSourceID(this.SourceID);

            utteranceGenerator.OnFileCompleted += new EventHandler(this.utteranceGenerator_OnFileCompleted);

            broker.OnCommandReceived += broker_OnCommandReceived;

			//	Temporary, used for hooking other Sensors to Utterance's output (TODO: Should be able to link sensors to each other without the annoying
			//		process of keeping track of/looking up their GUIDs)
			String source = "Construct Utterance Sensor";
			String entry = "Utterance sensor started with SourceID " + this.SourceID;
			if (!EventLog.SourceExists(source))
				EventLog.CreateEventSource(source, "Application");
			EventLog.WriteEntry(source, entry);
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    broker.Publish(new Telemetry("Enrollment ItemOutbox Added " + Environment.MachineName, new Dictionary<string,string>()));
                    break;
                case "SelectOutputDirectory":
                    SelectOutputDirectory(command.Args["OutputDirectory"]);
                    break;
                case "SetAudioFormatForRecording":
                    utteranceGenerator.SetAudioFormatForRecording(short.Parse(command.Args["SetFormatChannels"]), short.Parse(command.Args["SetFormatBitRate"]), int.Parse(command.Args["SetFormatHertz"]));
                    break;
                case "SetFileName":
                    utteranceGenerator.SetFileName(command.Args["SetFileName"]);
                    break;
                case "SetBufferSizeInMilliseconds":
                    utteranceGenerator.SetBufferSizeInMilliseconds(int.Parse(command.Args["SetBufferSizeInMilliseconds"]));
                    break;
                case "SetSilenceThresholdDecibels":
                    utteranceGenerator.SetSilenceThresholdDecibels(short.Parse(command.Args["SetSilenceThresholdDecibels"]));
                    break;
                case "SetSilenceThresholdFactor":
                    utteranceGenerator.SetSilenceThresholdFactor(int.Parse(command.Args["SetSilenceThresholdFactor"]));
                    break;
				case "DebugPrintInstanceID":
					Debug.WriteLine(this.SourceID);
					break;
                default:
                    break;
            }
        }

        private void GatherAvailableCommands()
        {
            if (availableCommands.Keys.Contains("SelectOutputDirectory") == false)
            {
                availableCommands.Add("SelectOutputDirectory", "OutputDirectory");
            }
        }

        private void SendAvailableCommandsTelemetry()
        {
            broker.Publish(new Telemetry("AvailableSensorCommands", availableCommands));
        }

        private void SelectOutputDirectory(string outputDirectory)
        {
            utteranceGenerator.setPath(outputDirectory + "//");
        }

        private void utteranceGenerator_OnFileCompleted(object sender, EventArgs e)
        {
            byte[] outputObject = ConvertFileToByteArray(utteranceGenerator.currentFileName);
			var utterance = new Utterance(outputObject);
            //SendItem(utterance, utteranceGenerator.startRecordingTime, "Utterance");
			SendItem(outputObject, utteranceGenerator.startRecordingTime, "Utterance");
        }

        private byte[] ConvertFileToByteArray(string FilePath)
        {
			return File.ReadAllBytes(FilePath);
        }

        protected override string Start()
        {
			sensorStartTime = DateTime.UtcNow;
            BeginRecording();
            return base.Start();
        }

        private void BeginRecording()
        {
            utteranceGenerator.Recorder.StartRecord();
        }

        protected override string Stop()
        {
            StopRecording();
            return base.Stop();
        }

        private void StopRecording()
        {
            utteranceGenerator.Recorder.StopRecord();

			byte[] output = ConvertFileToByteArray(utteranceGenerator.conversionFile + ".wav");
			var utterance = new Utterance(output);
			SendItem(output, "Utterance", sensorStartTime);
			//SendItem(utterance, "Utterance", sensorStartTime);
        }
    }
}