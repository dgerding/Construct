using System;

namespace Utterance
{
    class UtteranceDriver
    {
        static void Main(string[] args)
        {
            UtteranceSensor obj = new UtteranceSensor(args);

			Console.ReadLine();

			//	Debugging code
// 			UtteranceGenerator utteranceGenerator = new UtteranceGenerator();
// 
// 			utteranceGenerator.OnFileCompleted += utteranceGenerator_OnFileCompleted;
// 			utteranceGenerator.Recorder.StartRecord();
// 
// 			Console.ReadLine();
// 
// 			utteranceGenerator.Recorder.StopRecord();
        }

// 		static void utteranceGenerator_OnFileCompleted(object sender, EventArgs e)
// 		{
// 			Console.WriteLine("Wrote file");
// 		}
    }
}
