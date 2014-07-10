using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace SMILExtractWrapper
{
    /*
     * Assumes the SMILExtract_native.exe executable is in the same directory as SMILExtractWrapper.exe. Additionally 
     * assumes that the output location of the SMILExtract_native.exe classification is in a textfile named
     * '__________' in the same directory as both executables.
     * */
    public class SMILExtractWrapper
    {
        private string correctUsageString = "Correct usage'SMILExtractWrapper.exe <audiofile> <textfile.txt>'";
        public string CorrectUsageString
        {
            get
            {
                return correctUsageString;
            }
        }

        private string openEarExecutable = "SMILExtract.exe";
        public string OpenEarExecutable
        {
            get
            {
                return openEarExecutable;
            }
        }

        public bool ValidateArgs(string[] args)
        {
            if (args.Length > 2)
            {
                Console.WriteLine("Warning: SMILExtractWrapper.exe only accepts 2 parameters, ignoring all subsequent parameters");
                Console.WriteLine(correctUsageString);
            }

            else if (args[1].Substring(args[1].Length - 4) != ".txt")
            {
                Console.WriteLine("Second parameter must be a text file (.txt), containing audio transcription");
                Console.WriteLine(correctUsageString);
                return false;
            }
            return true;
        }

        // Returns double representation of classification OR double.MinValue on exception
        public double GetClassification(StreamReader reader)
        {
            string[] splitLine = reader.ReadLine().Split(new string[] { "~~>" }, StringSplitOptions.RemoveEmptyEntries);
            if (splitLine.Length >= 2)
            {
                string stringToParse = splitLine[1].Trim().Substring(0, 4);
                return double.Parse(stringToParse);
            }
            return double.MinValue;
            // TODO: Use logger here to record errors
        }
    }

    public class OpenEarWrapperHarness
    {
        static void Main(string[] args)
        {
            SMILExtractWrapper wrapper = new SMILExtractWrapper();
            // wrapper.ValidateArgs(args);

            Process process = new Process();
            StringBuilder processArgs = new StringBuilder();

            foreach (string str in args)
            {
                processArgs.Append(str + " ");
            }
            process.StartInfo.FileName = wrapper.OpenEarExecutable;
            process.StartInfo.Arguments = processArgs.ToString();
            process.Start();

            string outputFile = "results.txt";
            string[] outputFileSplit = processArgs.ToString().Split(new string[]{"-O"}, StringSplitOptions.RemoveEmptyEntries);
            if (outputFileSplit.Length == 2)
            {
                string[] secondSplit = outputFileSplit[1].Split('-');
                outputFile = secondSplit[0].Trim();
            }

            
            bool foundProcessOutput = false;
            while (!foundProcessOutput)
            {
                // checks for a file named <textfile.txt>.out, where <textfile.txt> is the filename of the textfile param 
                // passed into the SMILExtractWrapper. Can easily change this to change where it looks for file output from openEar.
                if (File.Exists(outputFile))
                {
                    StreamReader reader = File.OpenText(outputFile);
                    double classification = wrapper.GetClassification(reader);
                    // do something with 'classification'

                    foundProcessOutput = true;
                    reader.Close();
                }
            }
            File.Delete(outputFile);
        }
    }
}
