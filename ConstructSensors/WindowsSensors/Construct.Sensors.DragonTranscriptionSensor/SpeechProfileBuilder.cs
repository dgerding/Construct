using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    class SpeechProfileBuilder
    {
        private string
            TargetSpeaker,
            TargetTopic,
            TargetWavFile,
            TargetTxtFile,
            TrainingMessage,
            ErrorList = null;
        private DNSTools.DgnEngineControl
            DragonControl;
        private DNSTools.DgnStrings
            DragonStringFiles;
        private bool
            IsTrainingSuccessful = false;
        private bool
            IsEnrolling = false;

        public SpeechProfileBuilder()
        {

        }

        public void SetSpeaker(string speaker)
        {
            if (speaker != null && "" != speaker)
            {
                TargetSpeaker = speaker;
            }
            else
            {
                ErrorList += "No Speaker profile was provided, ";
            }
        }

        public void SetTopic(string topic)
        {
            if (topic != null && "" != topic)
            {
                TargetTopic = topic;
            }
            else
            {
                ErrorList += "No Topic was provided, ";
            }
        }

        public void SetTargetWavFile(string targetWavFile)
        {
            if (targetWavFile != null && "" != targetWavFile)
            {
                TargetWavFile = targetWavFile;
            }
            else
            {
                ErrorList += "No Wav file was provided, ";
            }
        }

        public void SetTargetTxtFile(string targetTxtFile)
        {
            if (targetTxtFile != null && "" != targetTxtFile)
            {
                TargetTxtFile = targetTxtFile;
            }
            else
            {
                ErrorList += "No Txt file was provided, ";
            }
        }

        public void SetEngineControl(DNSTools.DgnEngineControl dragonControl)
        {
            if (dragonControl != null)
            {
                DragonControl = dragonControl;
            }
            else
            {
                ErrorList += "Dragon Engine is null; please restart sensor, ";
            }
        }

        public void SetDragonStringsClass()
        {
            DragonStringFiles = new DNSTools.DgnStrings();
        }

        public void AddFilesToDragonStringFiles()
        {
            DragonStringFiles.Add(TargetTxtFile);
            DragonStringFiles.Add(TargetWavFile);
        }

        public void TrainSpeaker()
        {
            IsEnrolling = true;

            DNSTools.DgnAdapt dragonAdapt = null;            

            try
            {
                dragonAdapt = new DNSTools.DgnAdapt();
            }
            catch (Exception ex)
            {
                ErrorList += "Failed to create adaptation object. Dragon Error : " + ex.Message + ", ";
                IsEnrolling = false;
            }

            DNSTools.IDgnEyesFreeEnroll eyesFreeEnroll = null;

            try
            {
                eyesFreeEnroll = dragonAdapt.EyesFreeEnroll;
            }
            catch (Exception ex)
            {
                ErrorList += "Failed to get the Eyes-Free Enrollment object. Dragon Error: " + ex.Message + ", ";
                IsEnrolling = false;
            }
            try
            {
                eyesFreeEnroll.Run(
                    DragonStringFiles,
                    false,
                    0,
                    false
                    );
            }
            catch(Exception ex)
            {
                ErrorList += "Failed to run enrollment. Error: " + ex.Message + ", ";
                IsEnrolling = false;
            }

            IsEnrolling = false;

            bool IsTrained = false;

            try
            {
                if (DragonControl.get_SpeakerTrained(0))
                {
                    IsTrained = true;
                }
            }
            catch (Exception ex)
            {
                ErrorList += "Error querying the training state of the speaker. Dragon Error: " + ex.Message + ", ";
                return;
            }

            if (IsTrained)
            {
                TrainingMessage = "Training performed successfully.";
                IsTrainingSuccessful = true;
            }
            else
            {
                TrainingMessage = "More training must be done";
            }
        }

    }
}
