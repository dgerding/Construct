using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SensorSharedTypes;
using Utilities;


namespace SensorCommandTestUI
{
    public partial class MainForm : Form
    {
        private List<SensorInfo> sensors = new List<SensorInfo>();
        private string webServiceBaseUrl = "http://localhost:8000/MobileVideoSensorService";

        private ControllerClient.ControllerClient client = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode == false)
            {
                client = new ControllerClient.ControllerClient(webServiceBaseUrl);
                if (client.CanConnect() == false)
                {
                    MessageBox.Show(this, "Failed to connect to web service", "Error");
                }
                else
                {
                    logMessage("Connected to " + webServiceBaseUrl);
                }
                resizeColumns();
                timer.Enabled = true;
                populateResolutionDropDown();
                updateUI();
            }
        }

        private void populateResolutionDropDown()
        {
            this.comboBoxResolution.Items.Add("Medium");
            this.comboBoxResolution.Items.Add("Low");
            this.comboBoxResolution.SelectedItem = "Medium";
        }

        private void updateUI()
        {
            SensorInfo selectedSensor = getSelectedSensor();
            this.groupBoxCurrentSensor.Enabled = (selectedSensor != null);
        }

        private SensorInfo getSelectedSensor()
        {
            if ( this.listViewSensors.SelectedItems.Count == 0)
            {
                return null;
            }
            return this.listViewSensors.SelectedItems[0].Tag as SensorInfo;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            SensorInfo sensor = getSelectedSensor();
            if ( sensor == null )
            {
                return;
            }

            string message;
            if (client.SetSensorCommand(getSensorUploadCommandForResolution(), sensor.SensorID, out message) == false)
            {
                logMessage( "Failed to start sensor streaming. " + message );
            }
            else
            {
                logMessage("Sensor stream start command sent");
            }
            populateSensorList();
            updateUI();
        }

        private SensorCommand getSensorUploadCommandForResolution()
        {
            string selectedResolution = this.comboBoxResolution.SelectedItem.ToString();
            if ( selectedResolution == "Medium" )
            {
                return SensorCommand.StartUpload_MediumRes;
            }
            else
            {
                return SensorCommand.StartUpload_LowRes;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            SensorInfo sensor = getSelectedSensor();
            if (sensor == null)
            {
                return;
            }

            string message;
            if (client.SetSensorCommand(SensorCommand.StopUpload, sensor.SensorID, out message) == false)
            {
                logMessage("Failed to stop sensor streaming. " + message);
            }
            else
            {
                logMessage("Sensor stream stop command sent");
            }
            populateSensorList();
            updateUI();
        }

        private void buttonTools_Click(object sender, EventArgs e)
        {
            string directory = AppDataManager.ToolDirectory;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = directory;
            dialog.ShowDialog();
        }

        private void buttonShowAllSensors_Click(object sender, EventArgs e)
        {
            string directory = AppDataManager.SensorDataDirectory;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = directory;
            dialog.ShowDialog();
        }

        private void buttonLogs_Click(object sender, EventArgs e)
        {
            string directory = AppDataManager.LogDirectory;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = directory;
            dialog.ShowDialog();
        }

        private void buttonShowStreamsInProgress_Click(object sender, EventArgs e)
        {
            SensorInfo sensor = getSelectedSensor();
            if (sensor == null)
            {
                return;
            }

            string directory = AppDataManager.GetStreamPartDirectory(sensor.SensorID);
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = directory;
            dialog.ShowDialog();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            SensorInfo sensor = getSelectedSensor();
            if (sensor == null)
            {
                return;
            }

            string directory = AppDataManager.GetStreamRecordingDirectory(sensor.SensorID);
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = directory;
            dialog.ShowDialog();
        }

        private void listViewSensors_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateUI();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            resizeColumns();
        }

        private void resizeColumns()
        {
            this.columnHeaderSensor.Width = Convert.ToInt32(this.listViewSensors.Width * .6);
            this.columnHeaderIsConnected.Width = Convert.ToInt32(this.listViewSensors.Width * .2);
            this.columnHeaderPendingCommand.Width = Convert.ToInt32(this.listViewSensors.Width * .2) -5;

            this.columnHeaderMessage.Width = this.listViewMessages.Width - 5;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            populateSensorList();
        }

        private delegate void logMessageInvoker(string message);
        private void logMessage( string message )
        {
            if ( this.InvokeRequired == true )
            {
                this.Invoke(new logMessageInvoker(logMessage), new object[] { message });
                return;
            }

            DateTime time = DateTime.Now;
            string text = string.Format("{0}:{1}:{2}.{3} - {4}", time.Hour, time.Minute.ToString().PadLeft(2, '0'), time.Second.ToString().PadLeft(2,'0'), time.Millisecond, message.Trim());
            this.listViewMessages.Items.Add( text );
        }

        private void populateSensorList()
        {
            List<SensorInfo> newSensors = client.GetSensors();
            if ( newSensors == null )
            {
                logMessage("Web Service API call to GetSensors() returned null");
                return;
            }
            
            // compare to existing list to see if we need to repopulate
            bool listsDiffer = false;
            if (newSensors.Count == sensors.Count)
            {
                for (int i = 0; i < sensors.Count; i++)
                {
                    SensorInfo x = sensors[i];
                    SensorInfo y = newSensors[i];
                    if ( (x.SensorID != y.SensorID) ||
                         (x.IsConnected != y.IsConnected) ||
                         (x.PendingCommandAsInt != y.PendingCommandAsInt))
                    {
                        listsDiffer = true;
                        break;
                    }
                }
            }
            else
            {
                listsDiffer = true;
            }

            if ( listsDiffer == false )
            {
                return;
            }

            // remember which one was selecte d(if any)
            SensorInfo selectedSensor = getSelectedSensor();

            this.listViewSensors.Items.Clear();
            sensors = newSensors;
            foreach ( SensorInfo sensor in sensors )
            {
                if ( sensor.IsConnected == true )
                {
                    ListViewItem item = new ListViewItem(sensor.DisplayName);
                    item.SubItems.Add(sensor.IsConnected ? "Y" : "N");
                    item.SubItems.Add(sensor.GetPendingCommand().ToString());
                    item.Tag = sensor;
                    if ( selectedSensor != null && selectedSensor.SensorID == sensor.SensorID )
                    {
                        item.Selected = true;
                    }
                    this.listViewSensors.Items.Add(item);
                }
            }
        }




    }
}
