using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.IO;

namespace SapiSensor
{
    class SapiBase
    {
        public delegate void OnTranscribedHandler(Transcription details, DateTime transcriptionStartTime);

        public event OnTranscribedHandler OnTranscribed;

        private SpeechRecognitionEngine m_RecognitionEngine = null;

		public void ProcessUtterance(byte[] waveAudio, DateTime audioStartTime)
		{
			//	Not sure if this thing is thread-safe
			lock (m_RecognitionEngine)
			{
				m_RecognitionEngine.SetInputToWaveStream(new MemoryStream(waveAudio));

				bool isCompleted = false;
				while (!isCompleted)
				{
					try
					{
						var result = m_RecognitionEngine.Recognize();


						if (result == null)
						{
							isCompleted = true;
							continue;
						}

						DateTime transcriptionStartTime = audioStartTime + result.Audio.AudioPosition;

						if (OnTranscribed != null)
							OnTranscribed(new Transcription(result, audioStartTime), transcriptionStartTime);
					}
					catch (Exception e)
					{
						//	Should throw an exception at stream end

						//	Make sure that this *is* a stream-end 
						isCompleted = true;
					}
				}

				m_RecognitionEngine.SetInputToNull();
			}
		}

        public void Start()
        {
            if (m_RecognitionEngine != null)
                return;

			m_RecognitionEngine = new SpeechRecognitionEngine();
            m_RecognitionEngine.LoadGrammar(new DictationGrammar());
        }

        public void Stop()
        {
            if (m_RecognitionEngine == null)
                return;

            m_RecognitionEngine = null;
        }
    }
}
