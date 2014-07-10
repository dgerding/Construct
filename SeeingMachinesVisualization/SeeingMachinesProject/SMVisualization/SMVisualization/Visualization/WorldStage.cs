using Microsoft.Xna.Framework;
using SMFramework;
using SMFramework.Bayes;
using System;
using System.Collections.Generic;

namespace SMVisualization.Visualization
{
	public class BayesDataSet
	{
		public String ClassifierName;
		public LabeledBayes Classifier;
		public bool Enabled;
	}

	public class WorldStage
	{
		private SeeingModule[] m_Group;

		public List<BayesDataSet> Bayes = new List<BayesDataSet>();
		private SnapshotConverter m_Converter = new SingleDirectConverter("Sensor 1");

		public WorldRenderer	WorldModel = new Worlds.FourTopLab();
		public PersonRenderer  PersonModel = new Worlds.DefaultPersonRenderer();

		private VisualizationRenderOptions m_RenderOptions;

		public FaceData[] LastFaceData { get; private set; }

		public WorldStage(SMFramework.SensorClusterConfiguration config, VisualizationRenderOptions renderOptions)
		{
			m_Group = new SeeingModule[config.SensorConfigurations.Count];

			LastFaceData = new FaceData[config.SensorConfigurations.Count];

			for (int i = 0; i < m_Group.Length; i++)
			{
				m_Group[i] = new SeeingModule(config.SensorConfigurations[i]);
			}

			Camera.Position = new Vector3(-1.66F, 2.25F, 1.14F);
			Camera.Rotation = new Vector2(-0.51F, 5.27F);

			m_RenderOptions = renderOptions;
		}

		public void UpdateModuleToFaceData(int moduleIndex, FaceData faceData, FaceLabDataStream targetDatabase)
		{
			FaceData interpretedData = SeeingModule.EvaluateCameraData(m_Group[moduleIndex].SensorConfiguration, faceData, false);
			m_Group[moduleIndex].UseDirectCameraData(interpretedData);

			LastFaceData[moduleIndex]= interpretedData;

			if (targetDatabase != null)
				FaceDataSerialization.WriteFaceDataToDatabase(faceData, targetDatabase);
		}

		public void Draw()
		{
			if (WorldModel != null)
				WorldModel.Draw();

			if (PersonModel != null)
			{
				for (int i = 0; i < m_Group.Length; i++)
				{
					PersonModel.Draw(m_Group[i], m_Group, m_RenderOptions.SubjectOptions[i]);
				}
			}

            if (Bayes.Count != 0)
            {
				foreach (BayesDataSet bayesData in Bayes)
                {
					if (!bayesData.Enabled)
						continue;

					LabeledBayes classifier = bayesData.Classifier;

                    double[] inputData, classProbabilites;
                    double unused;

                    inputData = ModelGenerator.GenerateBayesDataFromFaceData(m_Group[0].Person.DataSource, m_Converter);
                    classifier.Bayes.Compute(inputData, out unused, out classProbabilites);

                    var intToStringMap = classifier.WordMap.CodeToString;
                    List<KeyValuePair<String, float>> classProbabilityMappings = new List<KeyValuePair<String, float>>();
                    for (int i = 0; i < intToStringMap.Count; i++)
                    {
                        classProbabilityMappings.Add(new KeyValuePair<String, float>(intToStringMap[i], (float)classProbabilites[i]));
                    }

                    classProbabilityMappings.Sort(delegate(KeyValuePair<String, float> a, KeyValuePair<String, float> b)
                    {
                        return b.Value.CompareTo(a.Value);
                    });

					DebugView.AddText("-------------------------------------------\nClassifier \"" + bayesData.ClassifierName + "\"");
                    DebugView.AddText(String.Format("Model Training Accuracy: {0:N2}%", classifier.TrainingAccuracy * 100.0));

                    DebugView.AddText("Signal1 Class Probabilities (Top 3):");
                    for (int i = 0; i < intToStringMap.Count && i < 3; i++)
                        DebugView.AddText("--- " + classProbabilityMappings[i].Key + ": " + String.Format("{0:N2}", classProbabilityMappings[i].Value * 100.0) + "%");
                }
            }
            else
            {
                DebugView.AddText("No Bayes Classifiers Loaded");
            }
		}
	}
}
