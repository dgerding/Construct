using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SMFramework;
using Microsoft.Xna.Framework;

namespace FaceDataGenerator
{
	public partial class MainForm : Form
	{

		SMFramework.SensorClusterConfiguration m_CurrentConfig;
		List<GenerationProperty> m_GenerationProperties;

		public MainForm()
		{
			InitializeComponent();

			tbFps.Validating += tbFps_Validating;
			tbDuration.Validating += tbDuration_Validating;

			SMFramework.DebugOutputStream.SlowInstance = new MessageBoxOutputStream();
			SMFramework.DebugOutputStream.FastInstance = new MessageBoxOutputStream();
		}

		void tbDuration_Validating(object sender, CancelEventArgs e)
		{
			float result;
			if (!float.TryParse(tbDuration.Text, out result))
			{
				MessageBox.Show("Time must be a floating-point value.");
				e.Cancel = true;
				return;
			}

			if (result <= 0)
			{
				MessageBox.Show("Recording time must be greater than 0.");
				e.Cancel = true;
				return;
			}
		}

		void tbFps_Validating(object sender, CancelEventArgs e)
		{
			uint result;
			if (!uint.TryParse(tbFps.Text, out result))
			{
				MessageBox.Show("FPS must be a positive integer value.");
				e.Cancel = true;
				return;
			}

			if (result == 0)
			{
				MessageBox.Show("FPS cannot be 0.");
				e.Cancel = true;
				return;
			}
		}

		private void btnSetCluster_Click(object sender, EventArgs e)
		{
			OpenFileDialog of = new OpenFileDialog();
			of.Filter = "Config File|*.cfg";
			DialogResult result = of.ShowDialog();
			if (result != DialogResult.OK)
				return;

			try
			{
				m_CurrentConfig = new SensorClusterConfiguration(of.FileName, SensorClusterConfiguration.FileLoadResponse.FailIfMissing);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Couldn't load file: " + ex.Message);
				return;
			}

			if (m_CurrentConfig.SensorConfigurations == null)
				return;

			lblCurrentCluster.Text = of.FileName;

			lbSensors.Items.Clear();

			m_GenerationProperties = new List<GenerationProperty>();
			for (int i = 0; i < m_CurrentConfig.SensorConfigurations.Count; i++)
			{
				GenerationProperty generationProperty = new GenerationProperty();
				generationProperty.IOD = 0.055F;
				m_GenerationProperties.Add(new GenerationProperty());
				lbSensors.Items.Add(m_CurrentConfig.SensorConfigurations[i].Label);
			}
		}

		private void lbSensors_SelectedIndexChanged(object sender, EventArgs e)
		{
			pgCurrentCluster.SelectedObject = m_GenerationProperties[lbSensors.SelectedIndex];
		}

		private float RandomRange(Random random, float range)
		{
			return (float)(random.NextDouble() * 2.0f - 1.0f) * range;
		}

		private Vector3 RandomVector(Random random, float range)
		{
			return new Vector3(
						RandomRange(random, range),
						RandomRange(random, range),
						RandomRange(random, range)
						);
		}

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			String targetFileName;

			SaveFileDialog sd = new SaveFileDialog();
			sd.Filter = "CSV File|*.csv";
			DialogResult result = sd.ShowDialog();
			if (result != DialogResult.OK)
				return;

			targetFileName = sd.FileName;

			PairedDatabase targetDatabase = new PairedDatabase();

			uint fps = uint.Parse(tbFps.Text);
			float time = float.Parse(tbDuration.Text);

			Random generator = new Random();

			for (uint i = 0; i < (uint)(fps * time); i++)
			{
				DateTime currentTime = new DateTime(0, DateTimeKind.Utc);
				currentTime = currentTime.AddSeconds((double)i / (double)fps);

				targetDatabase.BeginNextSnapshot(currentTime);

				for (int sensor = 0; sensor < m_GenerationProperties.Count; sensor++)
				{
					FaceData currentData = new FaceData();
					currentData.SignalLabel = m_CurrentConfig.SensorConfigurations[sensor].Label;

					currentData.HeadPosition =
						m_GenerationProperties[sensor].HeadPosition +
						RandomVector(generator, m_GenerationProperties[sensor].HeadPositionJitter);

					currentData.HeadRotation =
						m_GenerationProperties[sensor].HeadOrientation +
						RandomVector(generator, m_GenerationProperties[sensor].HeadOrientationJitter);

					currentData.VergencePosition =
						m_GenerationProperties[sensor].VergencePosition +
						RandomVector(generator, m_GenerationProperties[sensor].VergencePositionJitter);


					Matrix headRotationMatrix =
						Matrix.CreateRotationZ(currentData.HeadRotation.Z) *
						Matrix.CreateRotationX(currentData.HeadRotation.X) *
						Matrix.CreateRotationY(currentData.HeadRotation.Y);

					currentData.LeftEyePosition = currentData.HeadPosition;
					currentData.LeftEyePosition += Vector3.Transform(
															new Vector3(-m_GenerationProperties[sensor].IOD / 2.0F, 0.0F, 0.0F),
															headRotationMatrix
														);

					currentData.RightEyePosition = currentData.HeadPosition;
					currentData.RightEyePosition += Vector3.Transform(
															new Vector3(+m_GenerationProperties[sensor].IOD / 2.0F, 0.0F, 0.0F),
															headRotationMatrix
														);

					currentData.LeftPupilPosition = currentData.LeftEyePosition;
					currentData.RightPupilPosition = currentData.RightEyePosition;

					if (m_GenerationProperties[sensor].AutoTransformData)
						currentData.CoordinateSystem = FaceData.CoordinateSystemType.Local;
					else
						currentData.CoordinateSystem = FaceData.CoordinateSystemType.Global;

					currentData = SeeingModule.EvaluateCameraData(m_CurrentConfig.SensorConfigurations[sensor], currentData, false);

					FaceDataSerialization.WriteFaceDataToDatabase(currentData, targetDatabase);
				}
			}

			targetDatabase.WriteToDisk(targetFileName);

			MessageBox.Show("Success!");
		}

		class MessageBoxOutputStream : SMFramework.DebugOutputStream
		{
			public override void WriteLine(string text)
			{
				MessageBox.Show(text);
			}
		}
	}
}
