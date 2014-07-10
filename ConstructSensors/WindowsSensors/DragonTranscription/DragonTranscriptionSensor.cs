using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;
using Construct.SensorUtilities;
using Construct.Sensors;
using Construct.MessageBrokering;
using DragonTranscription;


namespace Construct.Sensors.DragonTranscription
{
    public class DragonTranscriptionSensor : Sensor
    {
        private readonly Dictionary<string, string>
            availableCommands;

        private Transcribe
            transcribe;
        private SpeechProfileBuilder
            speechProfileBuilder;

        private Object
             transcriptionTxtOutput,
             transcriptionDraOutput;

        private List<Rendezvous<Data>> 
            shellRendezvousList = new List<Rendezvous<Data>>();
        private Rendezvous<Data> 
            utteranceItemInboxRendezvous;

        private Queue<DateTime> transcribedFileQueue;

        string OutputDirectory;

        public DragonTranscriptionSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("1027AB45-7059-4BD0-84F0-ED2AE472B70A"), new Dictionary<string, Guid>() { { "DragonTranscription", Guid.Parse("F8512B1B-4637-404A-8A56-07518F8487E7") } })
        {
            transcribe = new Transcribe();
            speechProfileBuilder = new SpeechProfileBuilder();
            availableCommands = new Dictionary<string, string>();

            OutputDirectory = "C://ConstructData";

            transcribe.OnFileCompleted += new EventHandler(transcribe_OnFileCompleted);

            transcribedFileQueue = new Queue<DateTime>();

            //Gathers and sends telemetry for available Sensor Commands
            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();

            utteranceItemInboxRendezvous = new Rendezvous<Data>(Protocol.HTTP, Rendezvous<Data>.GetHostName(), Guid.Parse("3FE979D6-6D8C-4954-97CD-D49B43323AE6"), SourceID);
            broker.AddPeer(new Inbox<Data>(utteranceItemInboxRendezvous));

            //Allows Sensor broker to handle incoming Sensor Commands
            broker.OnCommandReceived += broker_OnCommandReceived;
            broker.OnItemReceived += broker_OnItemReceived;
        }

#region EventHandlers

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
                //case "AddForwardingRendezvous":
                //    AddForwardingRendezvous(command.Args["TargetMachine"], command.Args["TargetSensorID"]);
                //    break;
                case "AddUtteranceSensor":
                    AddUtteranceSensorConnection(command.Args["TargetMachine"], command.Args["TargetSensorID"]);
                    break;
                case "AddEnrollmentApp":
                    AddEnrollmentAppConnection(command.Args["TargetMachine"], command.Args["TargetSensorID"]);
                    break;
                case "CreateNewSpeaker":
                    CreateNewSpeaker(command.Args["TargetSpeaker"], command.Args["TargetTopic"], command.Args["TargetBaseModel"]);
                    break;
                case "SelectSpeakerProfile":
                    SelectSpeakerProfile(command.Args["TargetSpeaker"], command.Args["TargetTopic"], command.Args["TargetSession"]);
                    break;
                case "CloseSpeakerProfile":
                    CloseSpeakerProfile();
                    break;
                case "SelectOutputDirectory":
                    SelectOutputDirectory(command.Args["OutputDirectory"]);
                    break;
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                default:
                    break;
            }
        }

        private void broker_OnItemReceived(object sender, string dataString)
        {
            Data data = assistant.GetItem(dataString);
            switch (data.DataTypeSourceID.ToString().ToLowerInvariant())
            {
                case "3fe979d6-6d8c-4954-97cd-d49b43323ae6":

                    if (base.IsStarted)
                    {
                        ProcessItemsForTranscription(data);
                    }
                    break;
                case "7a1b4884-fdb4-4514-8b93-f08c8cc7cca9":

                    if (base.IsStarted)
                    {
                        ProcessItemsForProfileBuilding(data);
                    }
                    break;
                default:
                    throw new InvalidDataException();
           } 
        }

        void transcribe_OnFileCompleted(object sender, EventArgs e)
        {
            DateTime currentTranscribeFileTimeStamp = transcribedFileQueue.Dequeue();
            transcriptionTxtOutput = transcribe.TextFileToString(FileNamingUtility.GetDecoratedMediaFileName(currentTranscribeFileTimeStamp, currentTranscribeFileTimeStamp, null, "DragonTranscriptionItem", this.SourceID.ToString(), "txt"));
            transcriptionDraOutput = transcribe.DraFileToByteArray(FileNamingUtility.GetDecoratedMediaFileName(currentTranscribeFileTimeStamp, currentTranscribeFileTimeStamp, null, "DragonTranscriptionItem", this.SourceID.ToString(), "txt"));
            DragonPayload output = new DragonPayload((byte[])transcriptionDraOutput, (string)transcriptionTxtOutput);
            SendItem(output, "DragonTranscription", currentTranscribeFileTimeStamp);
        }

#endregion

#region Sensor Commands
        private void SendAvailableCommandsTelemetry()
        {
            broker.Publish(new Telemetry("AvailableSensorCommands", availableCommands));
        }

        private void GatherAvailableCommands()
        {
            if (availableCommands.Keys.Contains("SelectOutputDirectory") == false)
            {
                availableCommands.Add("SelectOutputDirectory", "OutputDirectory");
            }
            if (availableCommands.Keys.Contains("CreateNewSpeaker") == false)
            {
                availableCommands.Add("CreateNewSpeaker", "TargetSpeaker,TargetTopic,TargetBaseModel");
            }
            if (availableCommands.Keys.Contains("SelectSpeakerProfile") == false)
            {
                availableCommands.Add("SelectSpeakerProfile", "TargetSpeaker,TargetTopic,TargetSession");
            }
            if (availableCommands.Keys.Contains("CloseSpeakerProfile") == false)
            {
                availableCommands.Add("CloseSpeakerProfile", "");
            }
            if (availableCommands.Keys.Contains("AddUtteranceSensor") == false)
            {
                availableCommands.Add("AddUtteranceSensor", "TargetMachine,TargetSensorID");
            }
            if (availableCommands.Keys.Contains("AddEnrollmentApp") == false)
            {
                availableCommands.Add("AddEnrollmentApp", "TargetMachine,TargetSensorID");
            }
            //Add command names (MUST MATCH IN SWITCH STATEMENT) here, with comma separated parameters
            if (availableCommands.Keys.Contains("ResetSensorCommands") == false)
            {
                availableCommands.Add("ResetSensorCommands", "");
            }
        }

        private void ResetSensorCommands()
        {
            List<string> oldCommandKeys = new List<string>(availableCommands.Keys);

            foreach (string key in oldCommandKeys)
            {
                availableCommands.Remove(key);
            }
            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();
        }

        //Additional Sensor Command functions go below here.

        //Add user command
        //Select user command

        private void CloseSpeakerProfile()
        {
            if (transcribe != null)
            {
                transcribe.CloseSelectedSpeaker();
            }
            broker.Publish(new Telemetry("Closed Select Speaker " + Environment.MachineName, new Dictionary<string,string>()));
        }

        private void SelectOutputDirectory(string outputDirectory)
        {
            OutputDirectory = outputDirectory;
        }

        private void CreateNewSpeaker(string userName, string topic, string baseModel)
        {
            transcribe.AddNewUser(userName, topic, baseModel);
            broker.Publish(new Telemetry("Created New Speaker " + Environment.MachineName, new Dictionary<string,string>()));
        }

        private void SelectSpeakerProfile(string userName, string topic, string targetSession)
        {
            transcribe.SelectSpeaker(userName);
            transcribe.SelectTopic(topic);
            VerifyTargetDirectory(OutputDirectory + "//" +  targetSession);
            transcribe.OutputFilePath = new Uri(OutputDirectory + "//" + targetSession);
            broker.Publish(new Telemetry("Selected Speaker and Path " + Environment.MachineName, new Dictionary<string, string>()));
        }

        private void AddEnrollmentAppConnection(string targetMachine, string targetSensorID)
        {
            ConnectSensorToMyInput(targetMachine, targetSensorID, utteranceItemInboxRendezvous);

            try
            {
                InitializeUpdateSpeechProfile();
            }
            catch (Exception ex)
            {
                broker.Publish(new Telemetry("Utterance Connection Failed  " + Environment.MachineName, new Dictionary<string,string>()));
            }
            broker.Publish(new Telemetry("Utterance Connection Successful  " + Environment.MachineName, new Dictionary<string,string>()));

        }

        private void AddUtteranceSensorConnection(string targetMachine, string targetSensorID)
        {
            ConnectSensorToMyInput(targetMachine, targetSensorID, utteranceItemInboxRendezvous);
            broker.Publish(new Telemetry("Enrollment Connection Successful  " + Environment.MachineName, new Dictionary<string,string>()));
        }

        #endregion

#region Forwarding Item
        //Place function here to call SendItem with your sensor payload
        //private void AddForwardingRendezvous(string targetMachine, string targetSensorID)
        //{
        //    Rendezvous<Data> itemForwardingRendezvous = new Rendezvous<Data>(Protocol.HTTP, targetMachine, Guid.Parse("1027AB45-7059-4BD0-84F0-ED2AE472B70A"), Guid.Parse(targetSensorID));

        //    if (transcribe != null)
        //    {
        //        transcribe.SensorForwardingList.Add(itemForwardingRendezvous);
        //    }
        //}

        //public new void ForwardItem(object itemPayload, string dataName, List<Rendezvous<Data>> filteredItemList)
        //{
        //    Guid dataTypeID = DataTypes[dataName];

        //    Data data = new Data(itemPayload, SourceID, DataTypeSourceID, dataName, dataTypeID);
        //    foreach (Rendezvous<Data> rendezvous in shellRendezvousList)
        //    {
        //        if (assistant.GetJsonHeader(data.DataName) == String.Empty)
        //        {
        //            assistant.SetJsonHeader(data);
        //        }
        //        broker.PublishToInbox(rendezvous, data, assistant.GetJsonHeader(data.DataName));
        //    }

        //    foreach (Rendezvous<Data> rendezvous in filteredItemList)
        //    {
        //        if (assistant.GetJsonHeader(data.DataName) == String.Empty)
        //        {
        //            assistant.SetJsonHeader(data);
        //        }
        //        broker.PublishToInbox(rendezvous, data, assistant.GetJsonHeader(data.DataName));
        //    }
        //}

        //public new void ForwardItem(object itemPayload, DateTime manualTimeStamp, string dataName, List<Rendezvous<Data>> filteredItemList)
        //{
        //    Guid dataTypeID = DataTypes[dataName];

        //    Data data = new Data(itemPayload, SourceID, DataTypeSourceID, dataName, dataTypeID);
        //    data.TimeStamp = manualTimeStamp;
        //    foreach (Rendezvous<Data> rendezvous in shellRendezvousList)
        //    {
        //        if (assistant.GetJsonHeader(data.DataName) == String.Empty)
        //        {
        //            assistant.SetJsonHeader(data);
        //        }
        //        broker.PublishToInbox(rendezvous, data, assistant.GetJsonHeader(data.DataName));
        //    }

        //    foreach (Rendezvous<Data> rendezvous in filteredItemList)
        //    {
        //        if (assistant.GetJsonHeader(data.DataName) == String.Empty)
        //        {
        //            assistant.SetJsonHeader(data);
        //        }
        //        broker.PublishToInbox(rendezvous, data, assistant.GetJsonHeader(data.DataName));
        //    }
        //}
        #endregion

#region HandleItem
        private void ProcessItemsForTranscription(Data item)
        {
            if (transcribe != null)
            {
                transcribe.iteration++;
                transcribe.SetFileName(this.SourceID, item.TimeStamp);

                string currentFileNameNoExtension = transcribe.GetFileNameNoExtension();

                transcribe.ItemObjectToWav(item.Payload as byte[]);
                transcribe.TargetFile(transcribe.OutputFilePath.AbsolutePath.ToString());
                transcribe.StartTranscription();
                transcribedFileQueue.Enqueue(item.TimeStamp);
                //transcriptionOutput = transcribe.TextFileToString(currentFileNameNoExtension);
                ////ForwardItem(transcriptionOutput, SourceTypeID, transcribe.SensorForwardingList);
                //System.IO.File.WriteAllText("EnrollmentText" + item.BrokerID.ToString() + transcribe.iteration + ".txt", transcriptionOutput.ToString());
            }
        }

        private void ProcessItemsForProfileBuilding(Data item)
        {
            byte[] utterancePayload = (byte[])(((Dictionary<string, object>)item.Payload)["utterance"]);
            string textFilePayload = (string)(((Dictionary<string, object>)item.Payload)["textFile"]);

            if (transcribe != null)
            {
                transcribe.SetFileName(this.SourceID, item.TimeStamp);
                transcribe.ItemObjectToWav(utterancePayload);
                System.IO.File.WriteAllText(transcribe.OutputFilePath.AbsolutePath.ToString() + "//" + FileNamingUtility.GetDecoratedMediaFileName(item.TimeStamp, item.TimeStamp, null, "DragonTranscriptionItem", this.SourceID.ToString(), "txt"), textFilePayload);
                UpdateSpeechProfile(transcribe.OutputFilePath.AbsolutePath.ToString() + "//" + FileNamingUtility.GetDecoratedMediaFileName(item.TimeStamp, item.TimeStamp, null, "DragonTranscriptionItem", this.SourceID.ToString(), "txt"), transcribe.OutputFilePath.AbsolutePath.ToString() + "//" + transcribe.GetWavFileName());
                transcribe.iteration++;
            }
        }
#endregion

#region Start / Stop Sensor Functions
        protected override string Start()
        {
            //Add any calls you need to make to start your sensors functionality
            return base.Start();
        }

        protected override string Stop()
        {
            //Add any calls you need to make to start your sensors functionality
            return base.Stop();
        }
        #endregion

#region Sensor Functionality

        private void InitializeUpdateSpeechProfile()
        {
            speechProfileBuilder.SetSpeaker(transcribe.GetSpeaker());
            speechProfileBuilder.SetTopic(transcribe.GetTopic());
            speechProfileBuilder.SetEngineControl(transcribe.GetDragonEngine());
        }

        private void UpdateSpeechProfile(string targetTxtFile, string targetWavFile)
        {
            speechProfileBuilder.SetTargetTxtFile(targetTxtFile);
            speechProfileBuilder.SetTargetWavFile(targetWavFile);
            speechProfileBuilder.SetDragonStringsClass();
            speechProfileBuilder.AddFilesToDragonStringFiles();
            speechProfileBuilder.TrainSpeaker();
        }

        private static void VerifyTargetDirectory(string targetPath)
        {
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
        }


#endregion
    }
}
