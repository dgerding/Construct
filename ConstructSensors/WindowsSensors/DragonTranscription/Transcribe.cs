using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using Alvas.Audio;
using Construct.SensorUtilities;
using Construct.MessageBrokering;

namespace DragonTranscription
{
    class Transcribe
    {
        private DNSTools.DgnEngineControl
            DragonEngineControl;
        public event EventHandler
             OnFileCompleted;
        private string 
            DraFileName,
            TxtFileName,
            WavFileName,
            DraFile,
            TxtFile, 
            FilePath,
            FolderPath,
            SelectedSpeaker,
            SelectedTopic,
            fileName,
            fileNameNoExtension;
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
            SensorForwardingList = new List<Rendezvous<Data>>();
        }

        public void ItemObjectToWav(Object itemObject)
        {
            byte[] itemByteArray;
            itemByteArray = (byte[])itemObject;

            try
            {
                IntPtr format = AudioCompressionManager.GetPcmFormat(1, 16, 44100);

                WaveWriter waveWriter = new WaveWriter(File.Create(OutputFilePath.AbsolutePath.ToString() + "//" +  WavFileName), AudioCompressionManager.FormatBytes(format));
                waveWriter.WriteData(itemByteArray);
                waveWriter.Close();
            }
            catch (Exception ex)
            {
                throw new AudioException("Writing byte array object to wav failed");
            }
        }

        public void SetFileName(Guid sourceID, DateTime sourceCreationTime)
        {
            string date = DateTime.UtcNow.ToString();
            WavFileName = FileNamingUtility.GetDecoratedMediaFileName(sourceCreationTime, sourceCreationTime, null, "DragonTranscriptionItem", sourceID.ToString(), "wav");
            TxtFileName = FileNamingUtility.GetDecoratedMediaFileName(sourceCreationTime, sourceCreationTime, null, "DragonTranscriptionItem", sourceID.ToString(), "txt");
            DraFileName = FileNamingUtility.GetDecoratedMediaFileName(sourceCreationTime, sourceCreationTime, null, "DragonTranscriptionItem", sourceID.ToString(), "dra");
            fileNameNoExtension = "DragonTranscriptionItem" + sourceID.ToString() + date;
            fileName = "DragonTranscriptionItem" + sourceID.ToString() + date + ".wav";
        }

        public string GetFileNameNoExtension()
        {
            return fileNameNoExtension;
        }

        public Object TextFileToString(string textFile)
        {
            string transcribeText = File.ReadAllText(OutputFilePath.AbsolutePath.ToString() + "//" + textFile);
            return (Object)transcribeText;
        }
        public Object DraFileToByteArray(string draFile)
        {
            byte[] transcribeDra = File.ReadAllBytes(OutputFilePath.AbsolutePath.ToString() + "//"+ draFile);
            return (Object)transcribeDra;
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


        public string GetWavFileName()
        {
            return WavFileName;
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
            if (!CheckTranscriptionFileNames())
            {
                return;
            }
            
            DNSTools.DgnDictCustom TranscribeData = null;

            IsTranscribing = true;

            try
            {
                TranscribeData = new DNSTools.DgnDictCustom();
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
                TranscribeData.TranscribeFileToFile(OutputFilePath.AbsolutePath.ToString() + "//" + FilePath, OutputFilePath.AbsolutePath.ToString() + "//" + TxtFileName);
            }
            catch (Exception ex)
            {
            }

            try
            {
                TranscribeData.SessionSave(OutputFilePath.AbsolutePath.ToString() + "//" + DraFileName);
            }
            catch (Exception ex)
            {
            }
            TranscribeData.TranscriptionStopped += new DNSTools._DDgnDictCustomEvents_TranscriptionStoppedEventHandler(TranscribeData_TranscriptionStopped);
            
        }

        void TranscribeData_TranscriptionStopped()
        {
            OnFileCompleted(null, EventArgs.Empty);
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
                DraFile = FolderPath + System.IO.Path.GetFileNameWithoutExtension(FilePath) + ".dra";

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
            FilePath = WavFileName;
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

    }
}

