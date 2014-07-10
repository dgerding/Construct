using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alvas.Audio;
using System.IO;
using Construct.SensorUtilities;

namespace Utterance
{
    public class UtteranceGenerator
    {
        public string
            conversionFile,
            fileName = "UtteranceSensorOutput",
            path,
            sourceID,
            currentFileName;
        public event EventHandler
            OnFileCompleted;
        public RecorderEx
            Recorder;

        public DateTime
            startRecordingTime,
            endRecordingTime;

        private int
            filecount = 0,
            bufferSizeInMilliseconds = 100,
            silenceThresholdFactor = 10,
            formatHertz = 44100,
            silenceTimeCounter = 0,
			amplitudesHistoryCount = 600; // Approx. 1 minute of history capture
        private short
        //    silenceThresholdDecibels = 1200,
            formatChannels = 1,
            formatBitRate = 16;
        private bool 
            isRunning = false,
            timeReset = true;
        private Queue<byte[]>
            queue = new Queue<byte[]>(),
            durationQueue = new Queue<Byte[]>();

		private TimeSpan
			maxUtteranceLength = TimeSpan.FromSeconds(30);

		private Queue<float>
			averageRunningAmplitudes = new Queue<float>();

        public UtteranceGenerator()
        {
           Recorder = new RecorderEx(true);
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
        public void SetSourceID(Guid SourceID)
        {
            sourceID = SourceID.ToString();
        }

        public void setPath(string Path)
        {
            path = Path;
        }

        public void SetFileName(string setFileName)
        {
            if (!isRunning) fileName = setFileName;
        }

        public void SetBufferSizeInMilliseconds(int setBufferSizeInMilliseconds)
        {
            if (!isRunning) bufferSizeInMilliseconds = setBufferSizeInMilliseconds;
        }

        public void SetSilenceThresholdDecibels(short setSilenceThresholdDecibels)
        {
            //if (!isRunning) silenceThresholdDecibels = setSilenceThresholdDecibels;
        }

        public void SetSilenceThresholdFactor(int setSilenceThresholdFactor)
        {
            if (!isRunning) silenceThresholdFactor = setSilenceThresholdFactor;
        }



        void rex_Close(object sender, EventArgs e)
        {
            Console.WriteLine("Writing file");
            WriteDurationFile();
            WriteFile();
        }

        private void WriteDurationFile()
        {
            currentFileName = path + FileNamingUtility.GetDecoratedMediaFileName(startRecordingTime, endRecordingTime, null, fileName, sourceID, "wav");
            WaveWriter waveWriter = new WaveWriter(File.Create(currentFileName),
                 AudioCompressionManager.FormatBytes(Recorder.Format));

            for (int i = 0; i < durationQueue.Count; i++)
            {
                if (durationQueue.Count > 0)
                {
                    byte[] data = durationQueue.Dequeue();
                    waveWriter.WriteData(data);
                }
            }

            waveWriter.Close();
            waveWriter.Dispose();

            queue.Clear();
            queue.TrimExcess();
        }

		private bool IsAudioRelativelySilent(float currentAverageAudioAmplitude, IEnumerable<float> amplitudeHistory, float differenceDetectionFactor)
		{
			List<float> sortedHistory = amplitudeHistory.ToList();
			sortedHistory.Sort();

			float averageLowerQuartile = sortedHistory.Take(sortedHistory.Count / 4).Average();

			List<float> bottomHistoryValues = new List<float>();
			for (int i = 0; i < sortedHistory.Count; i++)
			{
				//	Collect history values until we hit a "large" jump from the average lower quartile amplitude ("large" relative to lowerAverage*differenceDetectionFactor)
				if (sortedHistory[i] - averageLowerQuartile > averageLowerQuartile * differenceDetectionFactor)
					break;

				bottomHistoryValues.Add(sortedHistory[i]);
			}

			float bottomHistoryAverage = bottomHistoryValues.Average();

			return currentAverageAudioAmplitude - bottomHistoryAverage < bottomHistoryAverage * differenceDetectionFactor;
		}

        void rex_Data(object sender, DataEventArgs e)
        {
            if (timeReset == true)
            {
                startRecordingTime = DateTime.UtcNow;
                timeReset = false;
            }

            queue.Enqueue(e.Data);
			durationQueue.Enqueue(e.Data);

			AudioBufferConverter bufferConverter = new AudioBufferConverter();
			float[] dataValues = bufferConverter.ConvertToFloat(e.Data, formatBitRate);

			float currentAverage = 0.0f;
			for (int i = 0; i < dataValues.Length; i++)
				currentAverage += Math.Abs(dataValues[i]);

			currentAverage /= dataValues.Length;

			bool isSilent = false;
			if (averageRunningAmplitudes.Count == amplitudesHistoryCount)
				isSilent = IsAudioRelativelySilent(currentAverage, averageRunningAmplitudes, 1.0f);

			//Console.WriteLine("{0} | {1}", currentAverage, isSilent ? "Silent" : "Loud");
			//Console.WriteLine("{0}", isSilent ? "Silent" : "Loud");

			if (averageRunningAmplitudes.Count == amplitudesHistoryCount - 1)
				Console.WriteLine("Filled history buffer");

			averageRunningAmplitudes.Enqueue(currentAverage);
			if (averageRunningAmplitudes.Count > amplitudesHistoryCount)
				averageRunningAmplitudes.Dequeue();
            
            if (isSilent)
            {
                silenceTimeCounter += 1;
                if (silenceTimeCounter >= silenceThresholdFactor)
                {
                    endRecordingTime = DateTime.UtcNow;
                    timeReset = true;
                    WriteFile();
                }
            }
            else
            {
                silenceTimeCounter = 0;
            }

			DateTime currentTime = DateTime.UtcNow;
			if ((currentTime - startRecordingTime).Ticks >= maxUtteranceLength.Ticks)
			{
				Console.WriteLine("Writing due to max utterance length");
				endRecordingTime = currentTime;
				timeReset = true;
				WriteFile();
			}
        }

        private void WriteFile()
        {
            int queueCount = queue.Count;

            if (queueCount <= silenceTimeCounter)
            {
                queue.Clear();
                queue.TrimExcess();
            }
            else
            {
                filecount++;


                currentFileName = path + FileNamingUtility.GetDecoratedMediaFileName(startRecordingTime, endRecordingTime, null, fileName, sourceID, "wav");
                WaveWriter waveWriter = new WaveWriter(File.Create(currentFileName), 
                     AudioCompressionManager.FormatBytes(Recorder.Format));


                for (int i = 0; i < queueCount + 10 - silenceTimeCounter; i++)
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
            silenceTimeCounter = 0;
        }

    }
}

