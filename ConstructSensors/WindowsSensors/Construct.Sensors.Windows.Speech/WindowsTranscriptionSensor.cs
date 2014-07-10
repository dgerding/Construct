using System;
using Alvas.Audio;
using System.IO;
using System.ServiceModel;
using System.Collections.Generic;
using System.Speech;
using System.Globalization;
using System.Diagnostics;
using System.Speech.AudioFormat;
using Construct.Sensors;
using Newtonsoft.Json;
using Construct.MessageBrokering;

namespace Construct.Sensors.WindowsTranscriptionSensor
{
    public class WindowsTranscriptionSensor : Sensor
    {
        private string AudioFileName = "SourceAudio";
        private string PreviousAudioFile;
        private int fileCount = 0;
        private CultureInfo
            cultureInfo = CultureInfo.InstalledUICulture;
        private Recognizer 
            recognizer;

        public WindowsTranscriptionSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("DC060EBE-7D1D-4CFD-8471-4BE50CF41D65"), new Dictionary<string, Guid> { { "", Guid.Empty } })
        {
            recognizer = new Recognizer(cultureInfo);

            recognizer.OnFileCompleted += new EventHandler(this.recognizer_OnFileCompleted);
            broker.OnCommandReceived += broker_OnCommandReceived;
            broker.OnItemReceived += broker_OnItemReceived;

            Rendezvous<Command> utteranceUri = new Rendezvous<Command>(args[3]);
            Rendezvous<Data> utteranceItemRendezvous = new Rendezvous<Data>(Protocol.NetNamedPipes,
                "localhost", Guid.Parse("3fe979d6-6d8c-4954-97cd-d49b43323ae6"),
                SourceID);

            broker.AddPeer(new Inbox<Data>(utteranceItemRendezvous));
            broker.AddPeer(new Outbox<Command>(utteranceUri));

            Dictionary<string, string> utteranceArgs = new Dictionary<string, string>();
            utteranceArgs.Add("Uri", utteranceItemRendezvous.Uri.AbsoluteUri);

            Command utteranceCommand = new Command("AddItemOutbox", utteranceArgs);

            broker.PublishToInbox(utteranceUri, utteranceCommand);

        }

        private void recognizer_OnFileCompleted(object sender, EventArgs e)
        {
            SendItem(getTranscriptionFromFile(), "WindowsTranscriptionItem", DateTime.Now);
        }

        private string getTranscriptionFromFile()
        {
            StreamReader reader = File.OpenText(recognizer.OutputFile);
            string streamOutput = reader.ReadToEnd();
            reader.Close();
            return streamOutput;
        }

        private void ItemObjectToWav(Object blob)
        {
            byte[] blobarray;
            blobarray = (byte[])blob;

            try
            {
                IntPtr format = AudioCompressionManager.GetPcmFormat(1, 16, 44100);
                PreviousAudioFile = AudioFileName + fileCount + ".wav";
                WaveWriter waveWriter = new WaveWriter(File.Create(AudioFileName + fileCount + ".wav"), AudioCompressionManager.FormatBytes(format));
                waveWriter.WriteData(blobarray);
                waveWriter.Close();
                fileCount++;

            }
            catch (Exception ex)
            {
                throw new AudioException("Writing byte array object to wav failed");
            }

        }

        private void broker_OnItemReceived(object sender, string dataString)
        {
            Data data = assistant.GetItem(dataString);

            switch (data.DataTypeSourceID.ToString().ToLowerInvariant())
            {
                case "3fe979d6-6d8c-4954-97cd-d49b43323ae6":
                    if (IsStarted)
                    {
                        ItemObjectToWav(data.Payload as byte[]);
                        recognizer.SetAudioInputMethodToWavFile(PreviousAudioFile);
                    }
                    break;
                default:
                    throw new InvalidDataException();
            }
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);
            Dictionary<string, string> args = command.Args;

            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                case "SetFileName":
                    recognizer.SetOutputFileName(args["setOutPutFileName"]);
                    break;
                case "SetFilePath":
                    recognizer.SetOutputFilePath(args["setOutputFilePath"]);
                    break;
                case "SetAudioInputMethodToAudioStream":
                    Stream audioStream = new FileStream(args["audioStream"], FileMode.Open, FileAccess.ReadWrite);
                    AudioBitsPerSample bitsPerSample = new AudioBitsPerSample();
                    AudioChannel audioChannel = new AudioChannel();
                    SpeechAudioFormatInfo format = new SpeechAudioFormatInfo(int.Parse(args["samplesPerSecond"]), bitsPerSample, audioChannel);
                    recognizer.SetAudioInputMethodToAudioStream(audioStream, format);
                    break;
                case "SetAudioInputMethodToDefaultAudioDevice":
                    recognizer.SetAudioInputMethodToDefaultAudioDevice();
                    break;
                case "SetAudioInputMethodToNull":
                    recognizer.SetAudioInputMethodToNull();
                    break;
                case "SetAudioInputMethodToWavFile":
                    recognizer.SetAudioInputMethodToWavFile(args["audioFileLocation"]);
                    break;
                case "SetAudioInputMethodToWavStream":
                    Stream wavStream = new FileStream(args["audioStream"], FileMode.Open, FileAccess.ReadWrite);
                    recognizer.SetAudioInputMethodToWavStream(wavStream);
                    break;
                default:
                    break;
            }
        }

        protected override string Start()
        {
            return base.Start();
        }

        protected override string Stop()
        {
            return base.Stop();
        }
    }
}
