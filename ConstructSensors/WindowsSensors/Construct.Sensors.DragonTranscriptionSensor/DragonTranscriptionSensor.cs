using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Construct.Dataflow.Brokering;
using Construct.Sensors;
using System.Diagnostics;
using System.ServiceModel;
using Newtonsoft.Json;
using Construct.Base.Wcf;
using Construct.Dataflow.Brokering.Messaging;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class DragonTranscriptionSensor : Sensor
    {
        private Dictionary<Guid, Transcribe>
            sensorTranscribePairs;
        private Dictionary<Guid, SpeechProfileBuilder>
            sensorProfileBuilderPairs;
        private Transcribe
            transcribe;
        private Object
            transcriptionOutput;
        private readonly Dictionary<string, string>
            availableCommands;
        private List<Rendezvous<Data>> shellRendezvousList = new List<Rendezvous<Data>>();
        private Rendezvous<Data> utteranceItemInboxRendezvous;

        private Queue<DragonObject>
            dragonObjectQueue;

        public DragonTranscriptionSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("1027AB45-7059-4BD0-84F0-ED2AE472B70A"))
        {
            sensorTranscribePairs = new Dictionary<Guid, Transcribe>();
            sensorProfileBuilderPairs = new Dictionary<Guid, SpeechProfileBuilder>();

            transcribe = new Transcribe();
            availableCommands = new Dictionary<string, string>();
            SendAvailableCommandsTelemetry();

            utteranceItemInboxRendezvous = new Rendezvous<Data>(Protocol.HTTP, Rendezvous<Data>.GetHostName(), Guid.Parse("3FE979D6-6D8C-4954-97CD-D49B43323AE6"), ProcessID);
            broker.AddPeer(new Inbox<Data>(utteranceItemInboxRendezvous));

            broker.OnCommandReceived += broker_OnCommandReceived;
            broker.OnItemReceived += broker_OnItemReceived;
        }

        private void AddUtteranceSensorConnection(string targetMachine, string targetSensorID, string targetSpeaker, string targetTopic, string targetPath)
        {
            ConnectSensorToMyInput(targetMachine, targetSensorID, utteranceItemInboxRendezvous);

            VerifyTargetDirectory(targetPath);

            sensorTranscribePairs.Add(Guid.Parse(targetSensorID.ToLowerInvariant()), new Transcribe());
            try
            {
                Transcribe selectedTranscribe = null;
                selectedTranscribe = sensorTranscribePairs
                .Single(s => s.Key == Guid.Parse(targetSensorID))
                .Value;
                InitializeTranscribe(selectedTranscribe, targetSpeaker, targetTopic, targetPath);

                selectedTranscribe.CloseSelectedSpeaker();
            }
            catch (Exception ex)
            {
            }
        }

        private static void VerifyTargetDirectory(string targetPath)
        {
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
        }

        private void AddEnrollmentAppConnection(string targetMachine, string targetSensorID, string targetSpeaker, string targetTopic, string targetBaseModel, string targetPath)
        {
            ConnectSensorToMyInput(targetMachine, targetSensorID, utteranceItemInboxRendezvous);

            ConnectSensorToSensorCommands(targetMachine, targetSensorID, utteranceItemInboxRendezvous);

            VerifyTargetDirectory(targetPath);

            sensorTranscribePairs.Add(Guid.Parse(targetSensorID.ToLowerInvariant()), new Transcribe());
            sensorProfileBuilderPairs.Add(Guid.Parse(targetSensorID.ToLowerInvariant()), new SpeechProfileBuilder());
            try
            {
                Transcribe selectedTranscribe = null;
                selectedTranscribe = sensorTranscribePairs
                .Single(s => s.Key == Guid.Parse(targetSensorID))
                .Value;
                try
                {
                    selectedTranscribe.AddNewUser(targetSpeaker, targetTopic, targetBaseModel);
                    
                }
                catch (Exception ex)
                {
                }
                InitializeTranscribe(selectedTranscribe, targetSpeaker, targetTopic, targetPath);
                try
                {
                    SpeechProfileBuilder selectedProfileBuilder = null;
                    selectedProfileBuilder = sensorProfileBuilderPairs
                    .Single(s => s.Key == Guid.Parse(targetSensorID))
                    .Value;
                    try
                    {
                        InitializeUpdateSpeechProfile(selectedTranscribe, selectedProfileBuilder);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                catch (Exception ex)
                {
                }
                selectedTranscribe.CloseSelectedSpeaker();
            }
            catch (Exception ex)
            {
            }
        }


        private void SendAvailableCommandsTelemetry()
        {
            GatherAvailableCommands();
            broker.Publish(new Telemetry("AvailableSensorCommands", availableCommands));
        }

        private void GatherAvailableCommands()
        {
            if (availableCommands.Keys.Contains("TestGenericCommand") == false)
            {
                availableCommands.Add("TestGenericCommand", "Uri");
            }
            if (availableCommands.Keys.Contains("AddUtteranceSensor") == false)
            {
                availableCommands.Add("AddUtteranceSensor", "TargetMachine,TargetSensorID,TargetSpeaker,TargetTopic,TargetPath");
            }
            if (availableCommands.Keys.Contains("AddEnrollmentApp") == false)
            {
                availableCommands.Add("AddEnrollmentApp", "TargetMachine,TargetSensorID,TargetSpeaker,TargetTopic,TargetBaseModel,TargetPath");
            }
        }

        private void InitializeTranscribe(Transcribe sensorTranscribe, string speakerID, string speakerTopic, string path)
        {
            sensorTranscribe.SelectSpeaker(speakerID);
            sensorTranscribe.SelectTopic(speakerTopic);
            sensorTranscribe.OutputFilePath = new Uri(path);
        }

        private void broker_OnItemReceived(object sender, string dataString)
        {
            Data data = assistant.GetItem(dataString);
            switch (data.SourceTypeID.ToString().ToLowerInvariant())
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

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
                case "AddForwardingRendezvous":
                    AddForwardingRendezvous(command.Args["TargetMachine"], command.Args["TargetSensorID"], command.Args["SourceSensorID"]);
                    break;
                case "AddUtteranceSensor":
                    AddUtteranceSensorConnection(command.Args["TargetMachine"], command.Args["TargetSensorID"], command.Args["TargetSpeaker"], command.Args["TargetTopic"], command.Args["TargetPath"]);
                    break;
                case "AddEnrollmentApp":
                    AddEnrollmentAppConnection(command.Args["TargetMachine"], command.Args["TargetSensorID"], command.Args["TargetSpeaker"], command.Args["TargetTopic"], command.Args["TargetBaseModel"], command.Args["TargetPath"]);
                    break;
                case "CloseSpeakerProfile":
                    CloseSpeakerProfile(command.Args["BrokerID"]);
                    break;
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    if (command.Args["Uri"].Contains("2339bd10-a933-41cc-b684-34b15d7516e6"))
                    {
                        shellRendezvousList.Add(itemRendezvous);
                    }
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                case "TestGenericCommand":
                    TestGenericCommand(command.Args["Uri"]);
                    break;
                default:
                    break;
            }
        }

        private void CloseSpeakerProfile(string sensorBrokerID)
        {
            Transcribe currentItemTranscriber = null;
            currentItemTranscriber = sensorTranscribePairs
            .Single(s => s.Key == Guid.Parse(sensorBrokerID))
            .Value;

            if (currentItemTranscriber != null)
            {
                currentItemTranscriber.CloseSelectedSpeaker();
            }
        }

        private void TestGenericCommand(string p)
        {
            throw new NotImplementedException();
        }

        private void UpdateSpeechProfile(string targetTxtFile, string targetWavFile, SpeechProfileBuilder selectedProfileBuilder)
        {

            selectedProfileBuilder.SetTargetTxtFile(Directory.GetCurrentDirectory().ToString() + "\\" + targetTxtFile);
            selectedProfileBuilder.SetTargetWavFile(Directory.GetCurrentDirectory().ToString() + "\\" + targetWavFile);
            selectedProfileBuilder.SetDragonStringsClass();
            selectedProfileBuilder.AddFilesToDragonStringFiles();
            selectedProfileBuilder.TrainSpeaker();
        }

        private void InitializeUpdateSpeechProfile(Transcribe selectedTranscribe, SpeechProfileBuilder selectedProfileBuilder)
        {
            selectedProfileBuilder.SetSpeaker(selectedTranscribe.GetSpeaker());
            selectedProfileBuilder.SetTopic(selectedTranscribe.GetTopic());
            selectedProfileBuilder.SetEngineControl(selectedTranscribe.GetDragonEngine());
        }

        private void AddForwardingRendezvous(string targetMachine, string targetSensorID, string sourceSensorID)
        {
            Rendezvous<Data> itemForwardingRendezvous = new Rendezvous<Data>(Protocol.HTTP, targetMachine, Guid.Parse("1027AB45-7059-4BD0-84F0-ED2AE472B70A"), Guid.Parse(targetSensorID));
            Transcribe selectedTranscribe = null;
            selectedTranscribe = sensorTranscribePairs
            .Single(s => s.Key == Guid.Parse(sourceSensorID))
            .Value;
            if (selectedTranscribe != null)
            {
                selectedTranscribe.SensorForwardingList.Add(itemForwardingRendezvous);
            }
        }

        private void ProcessItemsForTranscription(Data item)
        {
            Transcribe currentItemTranscriber = null;
            currentItemTranscriber = sensorTranscribePairs
            .Single(s => s.Key == item.BrokerID)
            .Value;

            if (currentItemTranscriber != null)
            {
                currentItemTranscriber.SetFileName(item.BrokerID);
                currentItemTranscriber.ItemObjectToWav(item.Payload as byte[]);
                currentItemTranscriber.TargetFile(currentItemTranscriber.OutputFilePath.AbsolutePath.ToString());
                currentItemTranscriber.StartTranscription();
                transcriptionOutput = currentItemTranscriber.TextFileToString(item.BrokerID);
                //ForwardItem(transcriptionOutput, SourceTypeID, currentItemTranscriber.SensorForwardingList);
                currentItemTranscriber.iteration++;
            }
        }

        private void ProcessItemsForProfileBuilding(Data item)
        {
            byte[] utterancePayload = (byte[])(((Dictionary<string, object>)item.Payload)["utterance"]);
            string textFilePayload = (string)(((Dictionary<string, object>)item.Payload)["textFile"]);
            Transcribe currentItemTranscriber = null;
            SpeechProfileBuilder currentItemProfileBuilder = null; 
            currentItemTranscriber = sensorTranscribePairs
            .Single(s => s.Key == item.BrokerID)
            .Value;

            currentItemProfileBuilder = sensorProfileBuilderPairs
            .Single(s => s.Key == item.BrokerID)
            .Value;

            if (currentItemTranscriber != null)
            {
                currentItemTranscriber.SetFileName(item.BrokerID);
                currentItemTranscriber.ItemObjectToWav(utterancePayload);
                System.IO.File.WriteAllText("EnrollmentText" + item.BrokerID.ToString() + currentItemTranscriber.iteration + ".txt", textFilePayload);
                //currentItemTranscriber.LoadSpeakerAndTopic(currentItemTranscriber.GetSpeaker(), currentItemTranscriber.GetTopic());
                DragonObject currentItem = new DragonObject(DragonObject.DragonSystemType.ProfileUpdater, currentItemTranscriber.GetSpeaker, currentItemTranscriber.GetTopic,
                    currentItemTranscriber.GetFileName(), "EnrollmentText" + item.BrokerID.ToString() + currentItemTranscriber.iteration + ".txt");

                //UpdateSpeechProfile("EnrollmentText" + item.BrokerID.ToString() + currentItemTranscriber.iteration + ".txt", currentItemTranscriber.GetFileName(), currentItemProfileBuilder);
                //currentItemTranscriber.CloseSelectedSpeaker();
                currentItemTranscriber.iteration++;

                dragonObjectQueue.Enqueue(currentItem);
            }
        }

        public new void ForwardItem(object itemPayload, Guid sourceTypeID, List<Rendezvous<Data>> filteredItemList)
        {
            Data data = new Data(itemPayload, sourceTypeID, "DragonTranscription");
            foreach (Rendezvous<Data> rendezvous in shellRendezvousList)
            {
                if (assistant.GetJsonHeader(data.DataName) == String.Empty)
                {
                    assistant.SetJsonHeader(data);
                }
                broker.PublishToInbox(rendezvous, data, assistant.GetJsonHeader(data.DataName));
            }

            foreach (Rendezvous<Data> rendezvous in filteredItemList)
            {
                if (assistant.GetJsonHeader(data.DataName) == String.Empty)
                {
                    assistant.SetJsonHeader(data);
                }
                broker.PublishToInbox(rendezvous, data, assistant.GetJsonHeader(data.DataName));
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

