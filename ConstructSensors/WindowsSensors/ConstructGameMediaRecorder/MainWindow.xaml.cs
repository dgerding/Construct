using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Alvas.Audio;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Devices;
using System.IO;
using WebcamControl;

namespace ConstructGameMediaRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Shared runtime and Window Members
        private Dictionary<string, List<string>> RecordingDevices = new Dictionary<string, List<string>>();
        string mediaPath = @"C:\ConstructGameMedia\";

        private Webcam screenRecordingControl;

        public MainWindow()
        {
            InitializeComponent();

            webCamCtrl = new Webcam();
            screenRecordingControl = new Webcam();

            Binding bndg_1 = new Binding("SelectedValue");
            bndg_1.Source = videoDevicesListBox;
            webCamCtrl.SetBinding(Webcam.VideoDeviceProperty, bndg_1);

            Binding bndg_2 = new Binding("SelectedValue");
            bndg_2.Source = audioDevicesListBox;
            webCamCtrl.SetBinding(Webcam.AudioDeviceProperty, bndg_2);

            Binding bndg_3 = new Binding("SelectedValue");
            bndg_3.Source = videoDevicesListBox;
            screenRecordingControl.SetBinding(Webcam.VideoDeviceProperty, bndg_3);

            Binding bndg_4 = new Binding("SelectedValue");
            bndg_4.Source = audioDevicesListBox;
            screenRecordingControl.SetBinding(Webcam.AudioDeviceProperty, bndg_4);


            hostNameLabel.Content=ConfigurationManager.AppSettings["HostName"];
            playerColorLabel.Content = ConfigurationManager.AppSettings["PlayerColor"];
            playerNameLabel.Content = ConfigurationManager.AppSettings["PlayerColor"];

            if (Directory.Exists(mediaPath) == false)
            {
                Directory.CreateDirectory(mediaPath);
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FindDevices();
            headsetAudioRecorder = new Alvas.Audio.Recorder();

            // Set some properties of the Webcam control
            webCamCtrl.VideoDirectory = mediaPath;
            webCamCtrl.VidFormat = VideoFormat.mp4;
            

            // Set the Webcam control as the ContentControl's content.
            WebCamContent.Content = webCamCtrl;

            // Set some properties of the Webcam control
            screenRecordingControl.VideoDirectory = mediaPath;
            screenRecordingControl.VidFormat = VideoFormat.mp4;

            // Set the Webcam control as the ContentControl's content.
            WebCamContent.Content = webCamCtrl;
        }
        private void FindDevices()
        {
            var vidDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            var audDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);

            var screenRecordingDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);

            foreach (EncoderDevice dvc in vidDevices)
            {
                videoDevicesListBox.Items.Add(dvc.Name);
            }

            foreach (EncoderDevice dvc in audDevices)
            {
                audioDevicesListBox.Items.Add(dvc.Name);
            }

            foreach (EncoderDevice dvc in screenRecordingDevices)
            {
                screenRecordingDevicesListBox.Items.Add(dvc.Name);
            }

        }

        
        private void recordButton_Click(object sender, RoutedEventArgs e)
        {
            if (headsetAudioRecordingOn || webCamRecordingOn)
            {
                StopAllRecording();
                recordButton.Content = "RECORD";
                recordButton.Foreground = Brushes.Red;

            }
            else
            {
                recordButton.Content = "STOP";
                recordButton.Foreground = Brushes.Black;
                StartAllRecording();
            }

            UpdateRecordingStatusUi();
        }

        private void StartAllRecording()
        {
            StartHeadsetAudioRecording(GameID.Text.Trim(), SessionID.Text.Trim(), playerNameLabel.Content.ToString());
            StartWebCamRecording();
            //StartScreenRecording();
        }

        private void StopAllRecording()
        {
            StopHeadsetAudioRecording();
            StopWebCamRecording();
            //StopScreenRecording();
        }

        private void UpdateRecordingStatusUi()
        {
            if (headsetAudioRecordingOn)
            {
                HeadsetAudioStatus.Text = "RECORDING";
                HeadsetAudioStatus.Foreground = Brushes.Red;
            }
            else
            {
                HeadsetAudioStatus.Text = "stopped recording";
                HeadsetAudioStatus.Foreground = Brushes.Black;
            }

            if (webCamRecordingOn)
            {
                WebCamStatus.Text = "RECORDING";
                WebCamStatus.Foreground = Brushes.Red;
            }
            else
            {
                WebCamStatus.Text = "off";
                WebCamStatus.Foreground = Brushes.Black;
            }
        }

        private void SessionID_TextChanged(object sender, TextChangedEventArgs e)
        {
            SessionID.Text = SessionID.Text.Trim();
            playerNameLabel.Content = ConfigurationManager.AppSettings["PlayerColor"] + SessionID.Text.Trim();
        }

        
        #endregion

        #region Headset Audio Members
        private bool headsetAudioRecordingOn = false;
        private bool headsetAudioRecordingFaulted = false;
        private Alvas.Audio.Player headsetAudioPlayer;
        private Alvas.Audio.Recorder headsetAudioRecorder;
        private string headsetAudioRecordingFile = String.Empty;

        private void StartHeadsetAudioRecording(string gameID, string sessionID, string playerColor)
        {
            headsetAudioRecordingFile = mediaPath + DateTime.UtcNow.ToLongDateString() + gameID + "_" + sessionID + "_" + playerColor + ".wav";

            headsetAudioRecorder.Open();
            Recorder.Channel rc = Recorder.Channel.Mono;
            Recorder.BitsPerSample rbps = Recorder.BitsPerSample.Bps16;
            Recorder.SamplesPerSec rsps;
            
            rsps = Recorder.SamplesPerSec.Sps44100;

            

            headsetAudioRecorder.Configure(rc, rbps, rsps);
            headsetAudioRecorder.Record();

            headsetAudioRecordingOn = true;
        }

        private void StopHeadsetAudioRecording()
        {
            headsetAudioRecorder.Stop();
            headsetAudioRecorder.Save(headsetAudioRecordingFile);
            headsetAudioRecorder.Close();


        }
        #endregion

        #region Webcam Members
        private Webcam webCamCtrl;
        private bool webCamRecordingOn = false;
        private bool webCamRecordingFaulted = false;

        private void StartWebCamRecording()
        {
            webCamCtrl.StartRecording();
        }

        private void StopWebCamRecording()
        {
            webCamCtrl.StopRecording();
        }

        private void StartWebCamCapture()
        {
            try
            {
                // Display webcam video on control.
                webCamCtrl.StartCapture();
            }
            catch (Microsoft.Expression.Encoder.SystemErrorException ex)
            {
                MessageBox.Show("Device is in use by another application");
            }
        }

        private void StopWebCamCapture()
        {
            webCamCtrl.StopCapture();
        }
        private void videoDevicesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            webCamCtrl.StopCapture();
            webCamCtrl.StartCapture();
        }

        #endregion

        #region screen recording

        private bool screenRecordingOn = false;
        private bool screenRecordingFaulted = false;

        private void StartScreenRecording()
        {
            screenRecordingControl.StartRecording();
        }

        private void StopScreenRecording()
        {
            screenRecordingControl.StopRecording();
        }

        private void StartScreenRecordingCapture()
        {
            try
            {
                // Display webcam video on control.
                screenRecordingControl.StartCapture();
            }
            catch (Microsoft.Expression.Encoder.SystemErrorException ex)
            {
                MessageBox.Show("Device is in use by another application");
            }
        }

        private void StartWebCamCapture(object sender, RoutedEventArgs e)
        {
            webCamCtrl.StopCapture();
        }
        
        private void screenRecordingDevicesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            screenRecordingControl.StopCapture();
            screenRecordingControl.StartCapture();
        }

        #endregion

        
        private void audioDevicesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        





    }
}
