using SMFramework;
using System;
using System.IO;
using System.Windows.Forms;

namespace BatchDataTrim
{
	public partial class MainForm : Form
	{
		SMFramework.SensorClusterConfiguration m_Config;
		String[] m_SelectedFiles;

		public MainForm()
		{
			InitializeComponent();
		}

		void UpdateConvertButton()
		{
			btnTrim.Enabled = m_Config != null && cbSignals.CheckedItems.Count != 0 && m_SelectedFiles != null;
		}

		private void btnSelectCluster_Click(object sender, EventArgs e)
		{
			if (clusterFileDialog.ShowDialog() != DialogResult.OK)
				return;

			m_Config = new SMFramework.SensorClusterConfiguration(clusterFileDialog.FileName, SMFramework.SensorClusterConfiguration.FileLoadResponse.FailIfMissing);

			foreach (FaceLabSignalConfiguration config in m_Config.SensorConfigurations)
			{
				cbSignals.Items.Add(config.Label);
			}

			UpdateConvertButton();
		}

		private void btnSelectFiles_Click(object sender, EventArgs e)
		{
			if (sourceFileDialog.ShowDialog() != DialogResult.OK)
				return;

			m_SelectedFiles = sourceFileDialog.FileNames;

			for (int i = 0; i < cbSignals.Items.Count; i++)
				cbSignals.SetItemChecked(i, false);

			if (m_Config != null)
			{
				foreach (String file in m_SelectedFiles)
				{
					for (int i = 0; i < m_Config.SensorConfigurations.Count; i++)
					{
						int indexOf = file.ToLower().IndexOf(m_Config.SensorConfigurations[i].Label.ToLower());
						if (indexOf != -1)
							cbSignals.SetItemChecked(i, true);
					}
				}
			}

			this.Update();

			UpdateConvertButton();
		}

		private void btnTrim_Click(object sender, EventArgs e)
		{
			cbSignals.Enabled = false;
			this.Update();

			FaceLabSignalConfiguration[] signals = new FaceLabSignalConfiguration[cbSignals.CheckedItems.Count];
			for (int i = 0; i < signals.Length; i++)
			{
				signals[i] = m_Config.FindConfigurationForLabel(cbSignals.CheckedItems[i] as String);
			}

			int fileIndex = 0;
			foreach (String fileName in m_SelectedFiles)
			{
				++fileIndex;
				PlaybackDataStream currentDatabase = new PlaybackDataStream(fileName, m_Config, FaceData.CoordinateSystemType.Global);
				PairedDatabase newDatabase = new PairedDatabase();

				while (!currentDatabase.PlaybackIsComplete)
				{
					lblOutput.Text = String.Format("Processing chunk {0} of {1} in {2} ({3} / {4})", currentDatabase.CurrentFrame + 1, currentDatabase.NumberOfSnapshots, Path.GetFileName(fileName), fileIndex, m_SelectedFiles.Length);

					FaceData[] chunks = currentDatabase.CurrentDataInterpretation;
					newDatabase.BeginNextSnapshot(currentDatabase.TargetRecordingTime);

					foreach (FaceData currentChunk in chunks)
					{
						foreach (FaceLabSignalConfiguration currentSignal in signals)
						{
							if (currentSignal.Label != currentChunk.SignalLabel)
								continue;

							FaceDataSerialization.WriteFaceDataToDatabase(currentChunk, newDatabase);
						}
					}

					currentDatabase.BeginNextSnapshot();

					this.Update();
				}

				String destinationFile = Path.GetDirectoryName(fileName) + "\\Trimmed";
				if (!Directory.Exists(destinationFile))
					Directory.CreateDirectory(destinationFile);
				destinationFile += "\\" + Path.GetFileNameWithoutExtension(fileName) + ".csv";
				if (File.Exists(destinationFile))
					File.Delete(destinationFile);
				newDatabase.WriteToDisk(destinationFile);
			}

			lblOutput.Text = "Done!";
			cbSignals.Enabled = true;
		}

		private void cbSignals_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateConvertButton();
		}
	}
}
