using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alvas.Audio;
using System.IO;

namespace DragonEnrollment
{
    class EnrollmentUtteranceGenerator
    {
        public string
            conversionFile,
            fileName = "DragonEnrollmentSensorOutput";
        public event EventHandler
            OnFileCompleted;
        public RecorderEx
            Recorder;
        private int
            filecount = 0,
            bufferSizeInMilliseconds = 100,
            formatHertz = 44100;
        private short
            formatChannels = 1,
            formatBitRate = 16;
        private bool 
            isRunning = false;
        private Queue<byte[]>
            queue = new Queue<byte[]>();

        public EnrollmentUtteranceGenerator()
        {
           Recorder = new RecorderEx();
           Init();
        }

        public void Init()
        {
            if (!isRunning)
            {
                Recorder.BufferSizeInMS = bufferSizeInMilliseconds;
                Recorder.Format = AudioCompressionManager.GetPcmFormat(formatChannels, formatBitRate, formatHertz);
                Recorder.Data += new RecorderEx.DataEventHandler(rex_Data);
                Recorder.Close += new EventHandler(rex_Close);

                isRunning = true;
            }
        }

        public void SetAudioFormatForRecording(short setFormatChannels, short setFormatBitRate, int setFormatHertz)
        {
            if (!isRunning)
            {
                formatChannels = setFormatChannels;
                formatBitRate = setFormatBitRate;
                formatHertz = setFormatHertz;
            }
        }

        public void SetFileName(string setFileName)
        {
            if (!isRunning) fileName = setFileName;
        }

        public void SetBufferSizeInMilliseconds(int setBufferSizeInMilliseconds)
        {
            if (!isRunning) bufferSizeInMilliseconds = setBufferSizeInMilliseconds;
        }


        public void rex_Close(object sender, EventArgs e)
        {
            Console.WriteLine("Writing file");
            WriteFile();
        }

        void rex_Data(object sender, DataEventArgs e)
        {
            queue.Enqueue(e.Data);
        }

        private void WriteFile()
        {
            int queueCount = queue.Count;
            
            filecount++;
            conversionFile = string.Format("{0}{1}.wav", fileName, filecount);

            WaveWriter waveWriter = new WaveWriter(File.Create(string.Format("{0}{1}.wav", fileName, filecount)),
            AudioCompressionManager.FormatBytes(Recorder.Format));

            for (int i = 0; i < queueCount + 10; i++)
            {
                if (queue.Count > 0)
                {
                    byte[] data = queue.Dequeue();
                    waveWriter.WriteData(data);
                }
            }

            waveWriter.Close();
            waveWriter.Dispose();

            OnFileCompleted(null, EventArgs.Empty);
            queue.Clear();
            queue.TrimExcess();
            
        }

    }
}
