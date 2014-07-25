using SMFramework;
using SMFramework.Bayes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace SMVisualization
{
	public partial class ScrubberForm : Form
	{
		private String m_DefaultPathText;
		private bool m_BypassOnStreamChangeEvent = false;
        private List<String> m_FullBayesFileNames = new List<String>();

		private ConnectionSignalR m_RemoteConnection;

		public AsyncPairedDatabase StreamedDatabase;
		FaceLabDataStream m_PlaybackStream = null;

		public ScrubberForm(AsyncPairedDatabase database)
		{
			InitializeComponent();

			m_DefaultPathText = lblDataSourcePath.Text;

			UpdateControlStates();

			barPlaybackLocation.Enabled = false;
			barPlaybackLocation.ValueChanged += barPlaybackLocation_ValueChanged;

			ShowInTaskbar = false;

			StreamedDatabase = database;
		}



		#region Data Event Handlers

		void m_RemoteConnection_OnTimeChange(DateTime newTime)
		{
			//PlaybackDataStream stream = SMVisualization.Instance.PlaybackStream;
			StreamingPlaybackDataStream stream = SMVisualization.Instance.PlaybackStream;
			if (stream.TargetRecordingTime != newTime)
				stream.TargetRecordingTime = newTime;
		}

		void Connection_StateChanged(Microsoft.AspNet.SignalR.Client.StateChange obj)
		{
			this.Invoke((MethodInvoker)delegate()
			{
				lblSignalRState.Text = obj.NewState.ToString();

				switch (obj.NewState)
				{
					case Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected:
						{
							CloseSignalRConnection();
							break;
						}

					default:
						{
							break;
						}
				}
			});
		}

		//	Used for procedural logic
		bool m_BarLocationChangeWasUser = true;

		void barPlaybackLocation_ValueChanged(object sender, EventArgs e)
		{
			UpdateNavbarDisplayText();

			if (chbxUseRecording.Checked && SourceIsSet())
			{
				StreamingPlaybackDataStream stream = SMVisualization.Instance.PlaybackStream;
				//PlaybackDataStream stream = SMVisualization.Instance.PlaybackStream;

				if (m_BarLocationChangeWasUser)
				{
					m_BypassOnStreamChangeEvent = true;
					stream.TargetRecordingTime = stream.FirstSnapshot.TimeStamp + TimeSpan.FromSeconds(barPlaybackLocation.Value / 1000.0F);
					m_BypassOnStreamChangeEvent = false;
				}


				if (m_RemoteConnection != null && m_RemoteConnection.CurrentTimeRequest != stream.TargetRecordingTime)
					m_RemoteConnection.CurrentTimeRequest = stream.TargetRecordingTime;
			}
		}

		void newStream_FrameChanged(StreamingPlaybackDataStream stream, DataSnapshot newFrame)
		{
			if (m_BypassOnStreamChangeEvent || !chbxUseRecording.Checked)
				return;

			barPlaybackLocation.Invoke((MethodInvoker)delegate()
			{
				m_BarLocationChangeWasUser = false;
				barPlaybackLocation.Value = (int)((newFrame.TimeStamp - stream.FirstSnapshot.TimeStamp).TotalMilliseconds);
				m_BarLocationChangeWasUser = true;
			});
		}

		#endregion


		#region Window Style/Behavior Management

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams baseParams = base.CreateParams;

				baseParams.Style = 0;
				baseParams.Style |= 0x00C00000; // WS_CAPTION

				return baseParams;
			}
		}

		/* Disable movement from the user, they should instead move the visualization window */
		const int WM_SYSCOMMAND = 0x0112;
		const int SC_MOVE = 0xF010;
		protected override void WndProc(ref Message message)
        {            
			switch(message.Msg)
			{
				case WM_SYSCOMMAND:
					int command = message.WParam.ToInt32() & 0xfff0;
					if (command == SC_MOVE)
						return;
				break;
            }
			base.WndProc(ref message);   
        }

		#endregion


		#region Form Event Handlers

		private void btnRestart_Click(object sender, EventArgs e)
		{
			RestartPlayback();
		}

		private void btnDataLocation_Click(object sender, EventArgs e)
		{
			String target = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Data";
			System.Diagnostics.Process.Start("explorer.exe", target);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			StreamedDatabase.Pause();

			if (saveRecordingDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				StreamedDatabase.Resume();
				return;
			}

			String sourceFile = StreamedDatabase.Stop();

			if (sourceFile == saveRecordingDialog.FileName)
				return;

			File.Copy(sourceFile, saveRecordingDialog.FileName, true);
			File.Delete(sourceFile);

			StreamedDatabase.Start(DatabaseFormatMapping.GenerateRecordingFilename());
		}

		private void btnClearRecording_Click(object sender, EventArgs e)
		{
			StreamedDatabase.Stop(false);
			StreamedDatabase.Start(DatabaseFormatMapping.GenerateRecordingFilename());
		}

		private void btnSignalRStartStop_Click(object sender, EventArgs e)
		{
			if (m_RemoteConnection == null)
			{
				InitializeNewSignalRConnection();
				return;
			}

			switch (m_RemoteConnection.Connection.State)
			{
				case Microsoft.AspNet.SignalR.Client.ConnectionState.Reconnecting:
				case Microsoft.AspNet.SignalR.Client.ConnectionState.Connecting:
				case Microsoft.AspNet.SignalR.Client.ConnectionState.Connected:
					{
						CloseSignalRConnection();
						break;
					}

				case Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected:
					{
						InitializeNewSignalRConnection();
						break;
					}
			}
		}

		private void chbxEnabled_CheckedChanged(object sender, EventArgs e)
		{
			UpdateControlStates();

			if (chbxUseRecording.Checked)
			{
				SetSpecificSourceStream(m_PlaybackStream);
			}
			else
			{
				ResetSourceStream();
			}
		}

		private void btnPlayPause_Click(object sender, EventArgs e)
		{
			if (SMVisualization.Instance.IsUsingPlayback)
				PausePlayback();
			else
				ResumePlayback();
		}

		private void btnSetSource_Click(object sender, EventArgs e)
		{
			OpenFileDialog of = new OpenFileDialog();
			if (!SourceIsSet())
			{
				of.InitialDirectory = System.IO.Path.GetDirectoryName(lblDataSourcePath.Text);
			}

			of.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

			if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				StreamingPlaybackDataStream newStream;

#if !DEBUG
				try
				{
					newStream = new StreamingPlaybackDataStream(of.FileName, SMVisualization.Instance.Sensors, FaceData.CoordinateSystemType.Local);
				}
				catch (Exception exception)
				{
					MessageBox.Show(exception.Message);
					return;
				}
#else
				newStream = new StreamingPlaybackDataStream(of.FileName, SMVisualization.Instance.Sensors, FaceData.CoordinateSystemType.Local);
#endif

                int maxLength = 25;
                if (of.FileName.Length > maxLength)
                    lblDataSourcePath.Text = "..." + of.FileName.Substring(of.FileName.Length - maxLength);
                else
                    lblDataSourcePath.Text = of.FileName;
				SMVisualization.Instance.IsUsingPlayback = false;
				newStream.FrameChanged += newStream_FrameChanged;
				m_PlaybackStream = newStream;

				double endTime;
				endTime = (newStream.LastSnapshot.TimeStamp - newStream.FirstSnapshot.TimeStamp).TotalMilliseconds / 1000.0F;

				barPlaybackLocation.Minimum = 0;
				barPlaybackLocation.Maximum = (int)Math.Floor(endTime * 1000.0f);

				barPlaybackLocation.Value = 0;

				UpdateNavbarDisplayText();
				SetSpecificSourceStream(m_PlaybackStream);
			}

			UpdateControlStates();

			chbxUseRecording.Checked = true;
		}

		#endregion


		private bool SourceIsSet()
		{
			return lblDataSourcePath.Text != m_DefaultPathText;
		}

		private void ResetSourceStream()
		{
			SetSpecificSourceStream(null);
		}

		private void SetSpecificSourceStream(FaceLabDataStream stream)
		{
			if (stream != null)
			{
				btnPlayPause.Enabled = true;
				btnPlayPause.Text = "Play";
				SMVisualization.Instance.IsUsingPlayback = false;
			}
			else
			{
				btnPlayPause.Enabled = false;
				PausePlayback();
			}

			//SMVisualization.Instance.PlaybackStream = stream as PlaybackDataStream;
			SMVisualization.Instance.PlaybackStream = stream as StreamingPlaybackDataStream;
		}

		private void ResumePlayback()
		{
			SMVisualization.Instance.IsUsingPlayback = true;
			btnPlayPause.Text = "Pause";
		}

		private void PausePlayback()
		{
			SMVisualization.Instance.IsUsingPlayback = false;
			btnPlayPause.Text = "Play";
		}

		private void RestartPlayback()
		{
			barPlaybackLocation.Value = 0;
		}

		private String ParseMillisecondsToTimeStamp(int milliseconds)
		{
			String displayText = "";

			int
				currentMilliseconds = (milliseconds / (1)) % 1000,
				currentSeconds = (milliseconds / (1000)) % 60,
				currentMinutes = (milliseconds / (1000 * 60)) % 60,
				currentHours = (milliseconds / (1000 * 60 * 60));

			if (currentHours != 0)
				displayText += currentHours + ":";

			if (currentHours != 0)
				displayText += currentMinutes.ToString("00") + ":";
			else if (currentMinutes != 0)
				displayText += currentMinutes.ToString() + ":";

			if (currentMinutes != 0)
				displayText += currentSeconds.ToString("00") + ".";
			else
				displayText += currentSeconds.ToString() + ".";

			displayText += currentMilliseconds.ToString("000");

			return displayText;
		}

		private void UpdateNavbarDisplayText()
		{
			int currentPosition = barPlaybackLocation.Value;
			int maxPosition = barPlaybackLocation.Maximum;

			lblPlayTime.Text = ParseMillisecondsToTimeStamp(currentPosition) + " / " + ParseMillisecondsToTimeStamp(maxPosition);
		}

		private void UpdateControlStates()
		{
			if (chbxUseRecording.Checked && SourceIsSet())
			{
				btnPlayPause.Enabled = true;
			}
			else
			{
				btnPlayPause.Enabled = false;
			}

			if (SourceIsSet())
			{
				barPlaybackLocation.Enabled = true;
				btnRestart.Enabled = true;
			}
			else
			{
				barPlaybackLocation.Enabled = false;
				btnRestart.Enabled = false;
			}

			if (SMVisualization.Instance.IsUsingPlayback)
				btnPlayPause.Text = "Pause";
			else
				btnPlayPause.Text = "Play";
		}

		private void InitializeNewSignalRConnection()
		{
			m_RemoteConnection = new ConnectionSignalR(cbxServer.Text, "ScrubbeRHub");
			m_RemoteConnection.Connection.StateChanged += Connection_StateChanged;
			m_RemoteConnection.TimeChanged += m_RemoteConnection_OnTimeChange;

			try
			{
				m_RemoteConnection.BeginSync();
			}
			catch (Exception e)
			{
				MessageBox.Show("Invalid server name, must be in format 'http://servername/Website.Name'\n\nError: " + e.Message);
				CloseSignalRConnection();
				return;
			}

			btnSignalRStartStop.Text = "Stop";
			if (!cbxServer.Items.Contains(cbxServer.Text))
			{
				cbxServer.Items.Add(cbxServer.Text);
			}
			cbxServer.Enabled = false;
		}

		private void CloseSignalRConnection()
		{
			m_RemoteConnection.StopSync();
			btnSignalRStartStop.Text = "Start";
			cbxServer.Enabled = true;
		}

		private void btnReloadCluster_Click(object sender, EventArgs e)
		{
			SMVisualization.Instance.ReloadContent();
        }

        private void btnAddBayes_Click(object sender, EventArgs e)
        {
            if (bayesOpenDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (String fileName in bayesOpenDialog.FileNames)
            {
                if (m_FullBayesFileNames.IndexOf(fileName) != -1)
                    continue;

                using (Stream fileStream = File.OpenRead(fileName))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    LabeledBayes bayesObject = formatter.Deserialize(fileStream) as LabeledBayes;

					Visualization.BayesDataSet visualizationBayes = new Visualization.BayesDataSet();
					visualizationBayes.Classifier = bayesObject;
					visualizationBayes.ClassifierName = Path.GetFileNameWithoutExtension(fileName);
					visualizationBayes.Enabled = true;

					if (bayesObject != null)
					{
						SMVisualization.Instance.World.Bayes.Add(visualizationBayes);
						m_FullBayesFileNames.Add(fileName);
						clbBayesSources.Items.Add(Path.GetFileName(fileName), CheckState.Checked);
					}
					else
						MessageBox.Show(String.Format("The ML file {0} was of invalid format and could not be loaded.", fileName));
                }
            }
		}

		private void btnRemoveBayes_Click(object sender, EventArgs e)
		{
			SMVisualization.Instance.World.Bayes.RemoveAt(clbBayesSources.SelectedIndex);
			m_FullBayesFileNames.RemoveAt(clbBayesSources.SelectedIndex);
			clbBayesSources.Items.RemoveAt(clbBayesSources.SelectedIndex);
		}

		void clbBayesSources_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			SMVisualization.Instance.World.Bayes[e.Index].Enabled = e.NewValue == CheckState.Checked;
		}

		private void clbBayesSources_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnRemoveBayes.Enabled = clbBayesSources.SelectedItems.Count != 0;
		}

		private void btnShowRenderOptions_Click(object sender, EventArgs e)
		{
			SMVisualization.Instance.SpawnRenderOptions();
		}
	}
}