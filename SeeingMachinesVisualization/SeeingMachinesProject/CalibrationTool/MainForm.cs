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

namespace CalibrationTool
{
	public partial class MainForm : Form
	{
		List<DataSlot> m_Chunks = new List<DataSlot>();
		FaceLabSignalConfiguration m_Signal;

		int m_TotalItems = 0;

		private void SetActiveSlot(int index)
		{
			lbChunks.SelectedIndex = index;

			DataSlot currentSlot = m_Chunks[index];
			switch (currentSlot.DataType)
			{
				case DataSlot.Type.DataCapture:
					gbLiveStream.Enabled = true;
					gbRecording.Enabled = false;

					rbStream.Checked = true;
					break;

				case DataSlot.Type.Recording:
					gbLiveStream.Enabled = false;
					gbRecording.Enabled = true;

					rbRecording.Checked = true;
					break;
			}
		}

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			OpenFileDialog configDialog = new OpenFileDialog();
			configDialog.Filter = "Cluster Configurations|*.cfg";

			DialogResult result = configDialog.ShowDialog();
			if (result != DialogResult.OK)
			{
				this.Close();
				return;
			}

			SensorClusterConfiguration cluster = new SensorClusterConfiguration();
			cluster.LoadFromFile(configDialog.FileName);

			SignalSelect signalSelect = new SignalSelect();
			signalSelect.SourceCluster = cluster;
			signalSelect.ShowDialog();

			m_Signal = signalSelect.SelectedSignal;

			Text = "Auto-Config for " + m_Signal.Label;

			scMainContainer.Panel2.Enabled = false;

			this.Activate();
		}

		private void btnNew_Click(object sender, EventArgs e)
		{
			scMainContainer.Panel2.Enabled = true;

			DataSlot currentSlot = new DataSlot();
			currentSlot.DataType = DataSlot.Type.DataCapture;
			currentSlot.Index = m_TotalItems++;
			m_Chunks.Add(currentSlot);
			lbChunks.Items.Add("Chunk " + currentSlot.Index);

			SetActiveSlot(lbChunks.Items.Count - 1);
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (m_Chunks.Count == 0 || lbChunks.SelectedIndex == -1)
				return;

			int currentIndex = lbChunks.SelectedIndex;

			m_Chunks.RemoveAt(currentIndex);
			lbChunks.Items.RemoveAt(currentIndex);

			if (m_Chunks.Count > 0)
				SetActiveSlot(Math.Min(currentIndex, m_Chunks.Count - 1));
			else
				scMainContainer.Panel2.Enabled = false;
		}

		private void rbRecording_CheckedChanged(object sender, EventArgs e)
		{
			if (rbRecording.Checked)
			{
				int index = lbChunks.SelectedIndex;
				DataSlot slot = m_Chunks[index];
				slot.DataType = DataSlot.Type.Recording;

				SetActiveSlot(index);
			}
		}

		private void rbStream_CheckedChanged(object sender, EventArgs e)
		{
			if (rbStream.Checked)
			{
				int index = lbChunks.SelectedIndex;
				DataSlot slot = m_Chunks[index];
				slot.DataType = DataSlot.Type.DataCapture;

				SetActiveSlot(index);
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			gbInputMethod.Enabled = false;
			lbChunks.Enabled = false;
			btnStart.Enabled = false;
			btnForceStop.Enabled = true;
		}

		private void btnForceStop_Click(object sender, EventArgs e)
		{
			gbInputMethod.Enabled = true;
			lbChunks.Enabled = true;
			btnForceStop.Enabled = false;
			btnStart.Enabled = true;
		}

		private void lbChunks_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lbChunks.SelectedIndex != -1)
				SetActiveSlot(lbChunks.SelectedIndex);
		}
	}
}
