using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalibrationViewer
{
	public partial class MainForm : Form
	{
		private String SanitizeVectorString(String vectorData)
		{
			vectorData = vectorData.Replace(" ", "");
			String[] components = vectorData.Split(',');

			String result = "";
			for (int i = 0; i < components.Length; i++)
			{
				result += components[i];
				if (i < components.Length - 1)
					result += ", ";
			}

			return result;
		}

		private bool ValidateVector(String vectorData)
		{
			String[] components = vectorData.Split(',');

			if (components.Length != 3)
				return false;

			for (int i = 0; i < components.Length; i++)
			{
				components[i] = components[i].Replace(" ", "");
				float buffer;
				if (!float.TryParse(components[i], out buffer))
					return false;
			}

			return true;
		}

		private Vector3 VectorFromString(String vectorData)
		{
			Vector3 result = new Vector3();

			//	We assume a valid string

			String[] components = vectorData.Split(',');
			for (int i = 0; i < components.Length; i++)
			{
				components[i] = components[i].Replace(" ", "");
			}

			result.X = float.Parse(components[0]);
			result.Y = float.Parse(components[1]);
			result.Z = float.Parse(components[2]);

			return result;
		}

		public String VectorToString(Vector3 vector)
		{
			return vector.X + ", " + vector.Y + ", " + vector.Z;
		}

		private void SynchronizeForm()
		{
			if (lbSensors.SelectedIndex == -1)
			{
				pnlSensorDataView.Hide();
				return;
			}

			SMFramework.FaceLabSignalConfiguration config = m_Configuration.FindConfigurationForLabel(lbSensors.SelectedItem as String);

			pgSensorView.SelectedObject = config;
		}

		private void SynchronizeConfiguration()
		{
		}

		String m_ConfigurationSourceFile = "";
		private SMFramework.SensorClusterConfiguration m_Configuration;

		public MainForm()
		{
			InitializeComponent();

			SMFramework.DebugOutputStream.SlowInstance = new MessageBoxOutputStream();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			m_Configuration = new SMFramework.SensorClusterConfiguration();

			SynchronizeForm();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_Configuration = new SMFramework.SensorClusterConfiguration();
			m_ConfigurationSourceFile = "";

			SynchronizeForm();
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			DialogResult result = dialog.ShowDialog();
			if (result != DialogResult.OK)
				return;

			try
			{
				m_Configuration = new SMFramework.SensorClusterConfiguration(
					dialog.FileName,
					SMFramework.SensorClusterConfiguration.FileLoadResponse.FailIfMissing
					);

				m_ConfigurationSourceFile = dialog.FileName;

				lbSensors.Items.Clear();
				foreach (SMFramework.FaceLabSignalConfiguration config in m_Configuration.SensorConfigurations)
				{
					lbSensors.Items.Add(config.Label);
				}
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("Unable to load file:\n\n" + ex.Message);
			}
		}

		private void addSensorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SMFramework.FaceLabSignalConfiguration config = new SMFramework.FaceLabSignalConfiguration();

			RequestStringBox requestForm = new RequestStringBox();
			requestForm.Text = "New Sensor Name";

			while (true)
			{
				requestForm.ShowDialog();

				if (requestForm.DialogResult != DialogResult.OK)
					return;

				if (lbSensors.Items.IndexOf(requestForm.tbInput.Text) != -1)
				{
					MessageBox.Show("A sensor with that name already exists.");
					continue;
				}

				if (requestForm.tbInput.Text.Length == 0)
				{
					MessageBox.Show("The sensor name cannot be blank.");
					continue;
				}

				requestForm.tbInput.Text = requestForm.tbInput.Text.Trim();

				if (requestForm.tbInput.Text.Length == 0)
				{
					MessageBox.Show("A sensor name cannot contain all whitespace characters.");
					continue;
				}
				
				break;
			}

			config.Label = requestForm.tbInput.Text;

			lbSensors.Items.Add(config.Label);

			m_Configuration.SensorConfigurations.Add(config);

			SynchronizeForm();
		}

		private void lbSensors_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lbSensors.SelectedIndex == -1)
			{
				pnlSensorDataView.Hide();
			}
			else
			{
				SynchronizeForm();
				pnlSensorDataView.Show();
			}
		}

		private void removeSensorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (lbSensors.SelectedIndex == -1)
				return;

			String sensorLabel = lbSensors.SelectedItem as String;
			lbSensors.Items.Remove(sensorLabel);

			m_Configuration.SensorConfigurations.Remove(m_Configuration.FindConfigurationForLabel(sensorLabel));
		}

		private void changeSensorNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (lbSensors.SelectedIndex == -1)
				return;

			String sensorLabel = lbSensors.SelectedItem as String;

			RequestStringBox request = new RequestStringBox();
			request.Text = "New Sensor Name";
			request.tbInput.Text = sensorLabel;

			while (true)
			{
				request.ShowDialog();

				if (request.DialogResult != DialogResult.OK)
					return;

				if (lbSensors.Items.IndexOf(request.tbInput.Text) != -1)
				{
					MessageBox.Show("A sensor with that name already exists.");
					continue;
				}

				if (request.tbInput.Text.Length == 0)
				{
					MessageBox.Show("The sensor name cannot be blank.");
					continue;
				}

				request.tbInput.Text = request.tbInput.Text.Trim();

				if (request.tbInput.Text.Length == 0)
				{
					MessageBox.Show("A sensor name cannot contain all whitespace characters.");
					continue;
				}

				break;
			}

			m_Configuration.FindConfigurationForLabel(sensorLabel).Label = request.tbInput.Text;
			lbSensors.Items[lbSensors.SelectedIndex] = request.tbInput.Text;
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			String targetfile = "";
			if (m_ConfigurationSourceFile.Length != 0)
			{
				targetfile = m_ConfigurationSourceFile;
			}
			else
			{
				SaveFileDialog dialog = new SaveFileDialog();
				dialog.Filter = "Config File|*.cfg";
				DialogResult result = dialog.ShowDialog();

				if (result != DialogResult.OK)
					return;

				targetfile = dialog.FileName;
			}

			m_Configuration.SaveToFile(targetfile);
			m_ConfigurationSourceFile = targetfile;
		}

		private void manageGUIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RequestStringBox request = new RequestStringBox();
			request.Text = "Configuration GUID";
			request.tbInput.Text = m_Configuration.GUID.ToString();

			while (true)
			{
				request.ShowDialog();

				if (request.DialogResult != DialogResult.OK)
					return;

				Guid newGUID;
				if (!Guid.TryParse(request.tbInput.Text, out newGUID))
				{
					MessageBox.Show("The GUID entered is invalid.");
					continue;
				}

				break;
			}
		}
	}
}
