using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using Alvas.Audio;
using Construct.Dataflow.Brokering;
using Construct.Dataflow.Brokering.Messaging;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    class Transcribe
    {
        private DNSTools.DgnEngineControl
            DragonEngineControl;
        public event EventHandler
            DragonUserUnloaded;
        private string 
            XmlFile,
            TxtFile, 
            FilePath,
            FolderPath,
            SelectedSpeaker,
            SelectedTopic,
            fileName;
        public int 
            iteration;
        public Uri
            OutputFilePath;
        private bool
            IsTranscribing;
        public List<Rendezvous<Data>> SensorForwardingList;

        public Transcribe()
        {
            try
            {
                DragonEngineControl = new DNSTools.DgnEngineControl();
                iteration = 0;
            }
            catch (Exception ex)
            {
            }
            DragonEngineControl.SpeakerClosing += new DNSTools._DDgnEngineControlEvents_SpeakerClosingEventHandler(DragonEngineControl_SpeakerClosing);
            SensorForwardingList = new List<Rendezvous<Data>>();
        }

        void Transcribe_DragonUserUnloaded(object sender, EventArgs e)
        {
            DragonUserUnloaded(this, e);
        }

        public void ItemObjectToWav(Object itemObject)
        {
            byte[] itemByteArray;
            itemByteArray = (byte[])itemObject;

            try
            {
                IntPtr format = AudioCompressionManager.GetPcmFormat(1, 16, 44100);

                WaveWriter waveWriter = new WaveWriter(File.Create(fileName), AudioCompressionManager.FormatBytes(format));
                waveWriter.WriteData(itemByteArray);
                waveWriter.Close();
            }
            catch (Exception ex)
            {
                throw new AudioException("Writing byte array object to wav failed");
            }
        }

        public void SetFileName(Guid utteranceSourceID)
        {
            fileName = "Utterance" + utteranceSourceID.ToString() + iteration + ".wav";
        }

        public Object TextFileToString(Guid utteranceSourceID)
        {
            string transcribeText = File.ReadAllText(OutputFilePath.AbsolutePath.ToString() + @"/Utterance" + utteranceSourceID.ToString() + iteration + ".txt");
            return (Object)transcribeText;
        }
        public Object XmlFileToString(Guid utteranceSourceID)
        {
            string transcribeXml = File.ReadAllText(OutputFilePath.AbsolutePath.ToString() + @"/Utterance" + utteranceSourceID.ToString() + iteration + ".xml");
            return (Object)transcribeXml;
            //not used yet still needs work on item side
        }

        public DNSTools.DgnEngineControl GetDragonEngine()
        {
            return DragonEngineControl;
        }

        public string GetSpeaker()
        {
            return SelectedSpeaker;
        }

        public string GetTopic()
        {
            return SelectedTopic;
        }


        public string GetFileName()
        {
            return fileName;
        }

        private void RefreshSpeakerInfo(string CurrentSpeakerProfile)
        {
            DNSTools.DgnStrings SpeakerTopics;
            try
            {
                SpeakerTopics = DragonEngineControl.get_Topics(CurrentSpeakerProfile);
                try
                {
                    EnumerateTopics(SpeakerTopics.GetEnumerator());
                }
                catch (Exception ex)
                {
                }
                
            }
            catch (Exception ex)
            {
            }
        }

        private void EnumerateTopics(IEnumerator EnumeratedTopics)
        {
            while (EnumeratedTopics.MoveNext())
            {
                Console.WriteLine((string)EnumeratedTopics.Current);
            }
        }

        public void LoadSpeakerAndTopic(string CurrentSpeakerProfile, string CurrentTopicProfile)
        {
            try
            {
                DragonEngineControl.SpeakerTopicSelect(CurrentSpeakerProfile, CurrentTopicProfile);
            }
            catch (Exception ex)
            {
            }
        }

        private void TranscribeFile()
        {
            LoadSpeakerAndTopic(SelectedSpeaker, SelectedTopic);

            if (!CheckTranscriptionFileNames())
            {
                return;
            }
            


            DNSTools.DgnTranscription TranscribeData = null;

            IsTranscribing = true;

            try
            {
                TranscribeData = new DNSTools.DgnTranscription();
            }
            catch (Exception ex)
            {
            }

            try
            {
                TranscribeData.Register();
            }
            catch (Exception ex)
            {
            }

            try
            {
                TranscribeData.TranscribeFileToFile(FilePath, TxtFile, XmlFile, 0);
            }
            catch (Exception ex)
            {
            }
            CloseSelectedSpeaker();
        }

        private bool CheckTranscriptionFileNames()
        {
            if ("" == FilePath.Trim() || "" == FolderPath.Trim())
            {
                return false;
            }
            if (!FolderPath.EndsWith("\\"))
            {
                FolderPath += "\\";
            }
            try
            {
                XmlFile = FolderPath + System.IO.Path.GetFileNameWithoutExtension(FilePath) + ".xml";

                TxtFile = FolderPath + System.IO.Path.GetFileNameWithoutExtension(FilePath) + ".txt";
            }
            catch (Exception ex)
            {
                 return false;
            }

            return true;
        }

        public void TargetFile(string folder)
        {
            FilePath = fileName;
            FolderPath = folder;
        }

        public void SelectSpeaker(string speaker)
        {
            SelectedSpeaker = speaker;
            RefreshSpeakerInfo(SelectedSpeaker);
        }

        public void SelectTopic(string topic)
        {
            SelectedTopic = topic;
            LoadSpeakerAndTopic(SelectedSpeaker, SelectedTopic);
        }

        public void StartTranscription()
        {
            TranscribeFile();
        }

        public void AddNewUser(string userName, string topic, string baseModel)
        {
            try
            {
                DragonEngineControl.SpeakerCreate(userName, baseModel);
            }
            catch(Exception ex)
            {
            }

            try
            {
                DragonEngineControl.TopicCreate(userName, topic, topic);
            }
            catch (Exception ex)
            {
            }
        }
        public void CloseSelectedSpeaker()
        {
            DragonEngineControl.SpeakerClose();
        }

        public DNSTools._DDgnEngineControlEvents_SpeakerClosingEventHandler DragonEngineControl_SpeakerClosing { get; set; }
    }
}

