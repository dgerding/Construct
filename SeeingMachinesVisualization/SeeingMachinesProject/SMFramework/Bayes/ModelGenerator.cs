using Accord.MachineLearning;
using Accord.MachineLearning.Bayes;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Univariate;
using System;
using System.Collections.Generic;

namespace SMFramework.Bayes
{
    public class ModelGenerator
    {
        public Dictionary<String, List<PairedDatabase>> DataSources
        {
            get;
            private set;
        }

        public ModelGenerator()
        {
            DataSources = new Dictionary<String, List<PairedDatabase>>();
        }

        public void AddDataSource(PairedDatabase sampleRecording, String dataType)
        {
            if (sampleRecording == null)
                throw new ArgumentNullException("sampleRecording");

            if (!DataSources.ContainsKey(dataType))
                DataSources.Add(dataType, new List<PairedDatabase>());

            DataSources[dataType].Add(sampleRecording);
        }

		public SnapshotConverter ConverterSource;

        public LabeledBayes GenerateModel(out double accuracy)
        {
			if (ConverterSource.ClassCount == 0)
			{
				// (Otherwise there's nothing to use for generating a bayesian model)
				ExceptionOutputStream.SlowInstance.WriteLine("The ConverterSource must have at least one identified class.");
				accuracy = 0.0;
				return null;
			}

            int
                inputCount = ConverterSource.ClassCount,
                categoryCount = DataSources.Count;

            NaiveBayes<NormalDistribution> bayes = new NaiveBayes<NormalDistribution>(categoryCount, inputCount, new NormalDistribution());

            double[][] inputData;
            int[] categories;

            BagOfWords wordMap;
            GenerateBayesData(out inputData, out categories, out wordMap);

			NormalOptions options = new NormalOptions();
			options.Regularization = 0.1;
            //  bayes.Estimate returns the error rate, not the success rate
            accuracy = 1.0 - bayes.Estimate(inputData, categories, true, options);

            LabeledBayes result = new LabeledBayes();
            result.Bayes = bayes;
            result.WordMap = wordMap;
            result.TrainingAccuracy = accuracy;

            return result;
        }

		public static double[] GenerateBayesDataFromSnapshot(DataSnapshot source, SnapshotConverter converter)
		{
			return converter.GenerateFromSnapshot(source);
		}

		public static double[] GenerateBayesDataFromFaceData(FaceData faceData, SnapshotConverter converter)
		{
			DataSnapshot proxySnapshot = new DataSnapshot();
			FaceDataSerialization.WriteFaceDataToSnapshot(faceData, proxySnapshot);

			return GenerateBayesDataFromSnapshot(proxySnapshot, converter);
		}

        void GenerateBayesData(out double[][] sourceData, out int[] dataLabels, out BagOfWords wordMap)
        {
            List<double[]> rawData = new List<double[]>();
            List<int> rawLabels = new List<int>();

            /* Map the keys to integers starting from 0 and incrementing */
            List<String> keys = new List<String>();
            foreach (var pair in DataSources)
                keys.Add(pair.Key);
            BagOfWords bag = new BagOfWords(keys.ToArray());
            wordMap = bag;

            foreach (var labeledSet in DataSources)
            {
                foreach (PairedDatabase database in labeledSet.Value)
                {
                    foreach (DataSnapshot snapshot in database.PairingSnapshots)
                    {
                        rawData.Add(GenerateBayesDataFromSnapshot(snapshot, ConverterSource));
                        rawLabels.Add(bag.StringToCode[labeledSet.Key]);
                    }
                }
            }

            sourceData = rawData.ToArray();
            dataLabels = rawLabels.ToArray();
        }
    }
}
