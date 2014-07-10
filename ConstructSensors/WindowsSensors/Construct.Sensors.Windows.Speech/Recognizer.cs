using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Speech;
using System.IO;

namespace Construct.Sensors.WindowsTranscriptionSensor
{
    class Recognizer
    {
        public string 
            OutputFileName = "WindowsTranscriptionSensor",
            OutputFile,
            OutputFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public event EventHandler
            OnFileCompleted;

        private int 
            FileCount = 0;
        private System.Speech.Recognition.SpeechRecognitionEngine
           RecognitionEngine;
        private StreamWriter
            outputFileStream;
        private bool
            isStreamOpen = false;

        public Recognizer(CultureInfo cultureInfo)
        {
            Initialize(cultureInfo);
        }

        public void SetOutputFileName(string setOutputFileName)
        {
            OutputFileName = setOutputFileName;
        }

        public void SetOutputFilePath(string setOutputFilePath)
        {
            OutputFilePath = setOutputFilePath;
        }

        private void Initialize(CultureInfo cultureInfo)
        {
            RecognitionEngine = new System.Speech.Recognition.SpeechRecognitionEngine(cultureInfo);

            RecognitionEngine.LoadGrammar(new System.Speech.Recognition.DictationGrammar());

            RecognitionEngine.SpeechRecognized += new EventHandler<System.Speech.Recognition.SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            RecognitionEngine.RecognizeCompleted += new EventHandler<System.Speech.Recognition.RecognizeCompletedEventArgs>(recognizer_RecognizerCompleted);
        }

        public void SetAudioInputMethodToAudioStream(System.IO.Stream audioStream, System.Speech.AudioFormat.SpeechAudioFormatInfo audioFormatInfo)
        {
            RecognitionEngine.SetInputToNull();
            
            RecognitionEngine.SetInputToAudioStream(audioStream, audioFormatInfo);

            RecognitionEngine.RecognizeAsync(System.Speech.Recognition.RecognizeMode.Multiple);
        }

        public void SetAudioInputMethodToDefaultAudioDevice()
        {
            RecognitionEngine.SetInputToNull();

            RecognitionEngine.SetInputToDefaultAudioDevice();

            RecognitionEngine.RecognizeAsync(System.Speech.Recognition.RecognizeMode.Multiple);
        }

        public void SetAudioInputMethodToNull()
        {
            RecognitionEngine.SetInputToNull();

            RecognitionEngine.RecognizeAsync(System.Speech.Recognition.RecognizeMode.Multiple);
        }

        public void SetAudioInputMethodToWavFile(string filePath)
        {
            RecognitionEngine.SetInputToNull();
            
            RecognitionEngine.SetInputToWaveFile(filePath);

            RecognitionEngine.RecognizeAsync(System.Speech.Recognition.RecognizeMode.Multiple);
        }

        public void SetAudioInputMethodToWavStream(System.IO.Stream audioStream)
        {
            RecognitionEngine.SetInputToNull();

            RecognitionEngine.SetInputToWaveStream(audioStream);

            RecognitionEngine.RecognizeAsync(System.Speech.Recognition.RecognizeMode.Multiple);
        }

        private void recognizer_SpeechRecognized(object sender, System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            OutputFile = OutputFilePath + OutputFileName + FileCount.ToString() + ".txt";
            if (!isStreamOpen)
            {
                outputFileStream = new StreamWriter(OutputFile);
                FileCount++;
                isStreamOpen = true;
            }            
            outputFileStream.Write(e.Result.Text + " ");
        }

        private void recognizer_RecognizerCompleted(object sender, System.Speech.Recognition.RecognizeCompletedEventArgs e)
        {
            outputFileStream.Close();
            isStreamOpen = false;
            OnFileCompleted(null, EventArgs.Empty);
        }
    }
}

