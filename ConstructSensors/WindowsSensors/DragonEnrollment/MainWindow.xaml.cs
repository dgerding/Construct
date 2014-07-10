using System;
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
using System.IO;
using System.Diagnostics;

namespace DragonEnrollment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DragonEnrollmentSensor 
            dragonEnrollmentSensor;
        private List<List<string>>
            textToReadByLine;
        private string
            preparedText ="";
        private bool
            isNotStarted;
        private int
            paragraphIndex = 0;
        private string
            machineName;

        public MainWindow()
        {
            InitializeComponent();

            machineName = Environment.MachineName;
            //string[] commandLineArgs = Environment.GetCommandLineArgs();
            //List<string> sensorArgs = new List<string>();

            //for (int i = 0; i < commandLineArgs.Count(); i++)
            //{
            //    if (i >= 1)
            //    {
            //        sensorArgs.Add(commandLineArgs[i]);
            //    }
            //}
            //string[] preparedSensorArgs = sensorArgs.ToArray<string>();
            List<string> sensorArgs = new List<string>();
            sensorArgs.Add(getProcessID());
            sensorArgs.Add(getShellUri());
            sensorArgs.Add(getSensorHostUri());
            string[] preparedSensorArgs = sensorArgs.ToArray<string>();
            
            dragonEnrollmentSensor = new DragonEnrollmentSensor(preparedSensorArgs);
            dragonEnrollmentSensor.onSensorStarted += new EventHandler(dragonEnrollmentSensor_onSensorStarted);
            textToReadByLine = new List<List<string>>();
            textToReadByLine.Add(new List<string>());
            ImportTextFile();
            SetTbReadableTextText();
            isNotStarted = false;
            btStart.IsEnabled = true;
            tbReadableText.Visibility = Visibility.Visible;
        }

        private string getShellUri()
        {
            return "http://Johnny5/F721F879-9F84-412F-AE00-632CFEA5A1F3/2339bd10-a933-41cc-b684-34b15d7516e6";
        }

        private string getSensorHostUri()
        {
            switch (machineName)
            {
                case "T3500-SALT":
                    return "http://t3500-salt/f721f879-9f84-412f-ae00-632cfea5a1f3/2e1f2ff3-6efa-4bb6-ad8f-f7bd5638383a";
                    break;
                case "T3500-CILANTRO":
                    return "http://t3500-cilantro/f721f879-9f84-412f-ae00-632cfea5a1f3/05e322c1-f972-457d-a018-8413434521e3";
                    break;
                case "T3500-SASSAFRAS":
                    return "http://t3500-sassafras/f721f879-9f84-412f-ae00-632cfea5a1f3/1d6694c2-a6f9-48ed-b900-0c26c590a21d";
                    break;
                case "T3500-ALLSPICE":
                    return "http://t3500-allspice/f721f879-9f84-412f-ae00-632cfea5a1f3/125083b8-0105-471a-ab09-b19537789649";
                    break;
                default:
                    return "This should not happen";
                    break;
            }
        }

        private string getProcessID()
        {
            switch (machineName)
            {
                case "T3500-SALT":
                    return "593A17F4-2780-4BB4-8466-F578F77693C8";
                    break;
                case "T3500-CILANTRO":
                    return "7CA451C5-7E6A-4715-944D-2EC95136166A";
                    break;
                case "T3500-SASSAFRAS":
                    return "C27DFD6F-3C24-4F7A-82FC-ED5B338F2BD5";
                    break;
                case "T3500-ALLSPICE":
                    return "2C784A13-3855-46B3-A9DD-3901C8B697EB";
                    break;
                default:
                    return "This should not happen";
                    break;
            }
        }

        private void dragonEnrollmentSensor_onSensorStarted(object sender, EventArgs e)
        {
            btStart.IsEnabled = true;
            tbReadableText.Visibility = Visibility.Visible;
        }

        private void ImportTextFile()
        {
            int index = 0;

            try
            {
                using (StreamReader sr = new StreamReader("TextToRead.txt"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("^"))
                        {
                            index++;
                            textToReadByLine.Add(new List<string>());
                        }
                        else
                        {
                            textToReadByLine[index].Add(line);
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        private void SetTbReadableTextText()
        {
            preparedText = "";
            dragonEnrollmentSensor.convertedEnrolmentTextFile = "";
            if (textToReadByLine.Count > paragraphIndex)
            {
                foreach (string line in textToReadByLine[paragraphIndex])
                {
                    preparedText += line + Environment.NewLine;
                    dragonEnrollmentSensor.convertedEnrolmentTextFile += (line + " ");
                }
                tbReadableText.Clear();
                tbReadableText.Text = preparedText;
            }
            else
            {
                tbReadableText.Clear();
                tbReadableText.Text = "You are now finished with the enrollment portion of our setup process.";
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            dragonEnrollmentSensor.BeginRecording();
            btNext.IsEnabled = true;
            btStart.Visibility = Visibility.Hidden;
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
        {
            dragonEnrollmentSensor.PauseRecording();
            dragonEnrollmentSensor.BeginRecording();
            paragraphIndex++;
            SetTbReadableTextText();
            

        }
    }
}
