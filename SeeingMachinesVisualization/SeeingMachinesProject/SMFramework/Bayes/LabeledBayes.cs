using Accord.MachineLearning;
using Accord.MachineLearning.Bayes;
using Accord.Statistics.Distributions.Univariate;
using System;

namespace SMFramework.Bayes
{
    [Serializable]
    public class LabeledBayes
    {
        public double TrainingAccuracy
        {
            get; set;
        }

        public NaiveBayes<NormalDistribution> Bayes
        {
            get; set;
        }

        public BagOfWords WordMap
        {
            get; set;
        }
    }
}
