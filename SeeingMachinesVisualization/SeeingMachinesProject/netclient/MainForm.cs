using System;
using System.Net;
using System.Windows.Forms;

namespace netclient
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			ForwardingThread.OnError += ForwardingThread_OnError;
			ForwardingThread.OnStatisticsUpdate += ForwardingThread_OnStatisticsUpdate;

			this.FormClosing += MainForm_FormClosing;
		}

		void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (ForwardingThread.ContinueThread)
				ForwardingThread.Stop();

			ForwardingThread.OnError -= ForwardingThread_OnError;
			ForwardingThread.OnStatisticsUpdate -= ForwardingThread_OnStatisticsUpdate;
		}

		void ForwardingThread_OnStatisticsUpdate(uint newBytesPerSecond)
		{
			this.Invoke((Action)delegate()
			{
				lblSpeed.Text = newBytesPerSecond + "B/s";
			});
		}

		void ForwardingThread_OnError(string errorMessage)
		{
			this.Invoke((Action)delegate()
			{
				SetRunning(false);
			});
		}

		public void UpdateUIToConfig(AppConfig config)
		{
			if (config.DestinationPort.HasValue)
				tbDestPort1.Text = config.DestinationPort.Value.ToString();

			if (config.SourcePort.HasValue)
				tbSourcePort.Text = config.SourcePort.Value.ToString();

			if (config.TargetHost != null)
				tbTargetHost1.Text = config.TargetHost.ToString();
		}

		public void SetRunning(bool forwardIsRunning)
		{
			if (forwardIsRunning)
			{
				btnStart.Enabled = false;
				btnStop.Enabled = true;

				tbDestPort1.Enabled = false;
				tbSourcePort.Enabled = false;
				tbTargetHost1.Enabled = false;

				tbDestPort2.Enabled = false;
				tbTargetHost2.Enabled = false;

 // 				lblStatus.Text = String.Format(
// 					"[Running]\n[SourcePort:{0}]\n[DestPort1:{1}]\n[DestHost1:'{2}'\n[DestPort2:{3}]\n[DestHost2:'{4}']]",
// 					Program.Config.SourcePort.Value, Program.Config.DestinationPort.Value, Program.Config.TargetHost
// 					);
				lblSpeed.Text = "[Running]";
			}
			else
			{
				btnStart.Enabled = true;
				btnStop.Enabled = false;

				tbDestPort1.Enabled = true;
				tbSourcePort.Enabled = true;
				tbTargetHost1.Enabled = true;

				tbDestPort2.Enabled = true;
				tbTargetHost2.Enabled = true;

				lblStatus.Text = "[Not Running]";

				lblSpeed.Text = "0B/s";
			}
		}

		private bool IsTarget1Set
		{
			get { return tbDestPort1.Text.Trim().Length != 0 && tbTargetHost1.Text.Trim().Length != 0; }
		}

		private bool IsTarget2Set
		{
			get { return tbDestPort2.Text.Trim().Length != 0 && tbTargetHost2.Text.Trim().Length != 0; }
		}

		private bool AddTarget1ToTargetList()
		{
			return AddTargetFromText(tbDestPort1.Text, tbTargetHost1.Text);
		}

		private bool AddTarget2ToTargetList()
		{
			return AddTargetFromText(tbDestPort2.Text, tbTargetHost2.Text);
		}

		private bool AddTargetFromText(String targetPort, String targetHost)
		{
			ushort destPort;
			if (!ushort.TryParse(targetPort, out destPort))
			{
				MessageBox.Show("The Destination Port must be a number between 0 and 65535.");
				return false;
			}

			IPAddress targetAddress = IPAddressResolver.Resolve(targetHost);
			if (targetAddress == null)
			{
				MessageBox.Show("The Target Host '{0}' could not be resolved to an IP address.", targetHost);
				return false;
			}

			ForwardingThread.AddForwardTarget(destPort, targetAddress);

			return true;
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			ushort sourcePort;
			if (!ushort.TryParse(tbSourcePort.Text, out sourcePort))
			{
				MessageBox.Show("The Source Port must be a number between 0 and 65535.");
				return;
			}

			if (!IsTarget1Set && !IsTarget2Set)
			{
				MessageBox.Show("At least one target must be set for forwarding.");
				return;
			}

			ForwardingThread.ClearForwardTargets();

			if (IsTarget1Set)
				if (!AddTarget1ToTargetList()) return;
			if (IsTarget2Set)
				if (!AddTarget2ToTargetList()) return;

			ForwardingThread.Start(sourcePort);

			SetRunning(true);
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			ForwardingThread.Stop();
			SetRunning(false);
		}
	}
}
