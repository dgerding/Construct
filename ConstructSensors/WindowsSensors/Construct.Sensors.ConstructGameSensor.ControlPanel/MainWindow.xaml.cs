using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Columbia.CCF.UI.ControlPanel
{
    public partial class MainWindow : Window
    {
        private static string _conStr = System.Configuration.ConfigurationManager.ConnectionStrings["Columbia.CCF.UI.Properties.Settings.CCFConnectionString"].ConnectionString;
        TcpController oTcp;

        public MainWindow()
        {
            InitializeComponent();
        }

        private static string GetPseudonymFromComboBox(ComboBox cbo)
        {
            if (cbo.Items.Count > 0 && cbo.SelectedItem != null)
                return ((def_Object)(cbo.SelectedItem)).Pseudonim.Replace(" ", "");
            else
                return null;
        }

        /// <summary>
        /// This returns the selected string from a combobox item. 
        /// This method is necessary because some of the items are strings and some are comboboxitems.
        /// </summary>
        /// <param name="cbo"></param>
        /// <returns></returns>
        private static string GetSelectedItemStringFromComboBox(ComboBox cbo)
        {
            if (cbo.SelectedItem == null)
            {
                return null;
            }
            string strValue;
            strValue = cbo.SelectedItem as string;

            if (strValue == null)
                strValue = ((ComboBoxItem)(cbo.SelectedItem)).Content.ToString();
            return strValue;
        }

        private static string GetSelectedItemString(ComboBox cbo)
        {
            if (cbo.SelectedItem == null)
            {
                return null;
            }
            string strValue;
            strValue = cbo.SelectedItem as string;

            if (strValue == null)
                strValue = ((ComboBoxItem)(cbo.SelectedItem)).Content.ToString();
            return strValue;
        }

        /// <summary>
        /// This method connects to the clients and loads the comboboxes' data.
        /// </summary>
        void ConnectToTcpPeersAndLoadPlayerPseudonymData()
        {
            new TcpController();
            try
            {
                if (oTcp == null)
                {
                    oTcp = new TcpController();
                    oTcp.LoadPlayers();
                }
            }
            catch (Exception)
            {
                throw (new Exception("Starting TcpController failed."));
            }

            if (oTcp != null)
            {
                
                cboPlayer1Name.ItemsSource = oTcp.PlayerNames;
                cboPlayer1Name.Visibility = Visibility.Visible;

                cboPlayer2Name.ItemsSource = oTcp.PlayerNames;
                cboPlayer2Name.Visibility = Visibility.Visible;

                cboPlayer3Name.ItemsSource = oTcp.PlayerNames;
                cboPlayer3Name.Visibility = Visibility.Visible;

                cboPlayer4Name.ItemsSource = oTcp.PlayerNames;
                cboPlayer4Name.Visibility = Visibility.Visible;

                MessageBox.Show("Available Players Loaded.");
            }
            else
                MessageBox.Show("Unable to load availble players.  Ask for help.");
        }

        private void AddVideoStreamersReportedAvailableByTcp()
        {
            int iVideoCounter = 0;
            foreach (string item in oTcp.VideoSockets.Keys)
            {
                ComboBoxItem oSelectedItem = new ComboBoxItem();
                oSelectedItem.Content = item;
                oSelectedItem.IsSelected = true;

                if (iVideoCounter == 0)
                    cboPlayer1Video.Items.Add(oSelectedItem);
                else
                    cboPlayer1Video.Items.Add(item);

                if (iVideoCounter == 1)
                    cboPlayer2Video.Items.Add(oSelectedItem);
                else
                    cboPlayer2Video.Items.Add(item);

                if (iVideoCounter == 2)
                    cboPlayer3Video.Items.Add(oSelectedItem);
                else
                    cboPlayer3Video.Items.Add(item);

                if (iVideoCounter == 3)
                    cboPlayer4Video.Items.Add(oSelectedItem);
                else
                    cboPlayer4Video.Items.Add(item);

                if (iVideoCounter == 5)
                    cboGameServerName.Items.Add(oSelectedItem);
                else
                    cboGameServerName.Items.Add(item);

                iVideoCounter++;
            }
        }
  
        private void AddAudioStreamerSocketsReportedAvailableByTcp()
        {
            int iAudioCounter = 0;

            foreach (string item in oTcp.AudioSockets.Keys)
            {
                ComboBoxItem oSelectedItem = new ComboBoxItem();
                oSelectedItem.Content = item;
                oSelectedItem.IsSelected = true;

                if (iAudioCounter == 0)
                    cboPlayer1Audio.Items.Add(oSelectedItem);
                else
                    cboPlayer1Audio.Items.Add(item);

                if (iAudioCounter == 1)
                    cboPlayer2Audio.Items.Add(oSelectedItem);
                else
                    cboPlayer2Audio.Items.Add(item);

                if (iAudioCounter == 2)
                    cboPlayer3Audio.Items.Add(oSelectedItem);
                else
                    cboPlayer3Audio.Items.Add(item);

                if (iAudioCounter == 3)
                    cboPlayer4Audio.Items.Add(oSelectedItem);
                else
                    cboPlayer4Audio.Items.Add(item);

                iAudioCounter++;
            }
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectToTcpPeersAndLoadPlayerPseudonymData();          
            btnConnectLab.Visibility = Visibility.Hidden;

            ValidatePlayerRosterButton.Visibility = Visibility.Visible;
            
            btnStartServer.IsEnabled = true;
            cboGameType.Visibility = Visibility.Visible;
        }

        private void SendStopServiceMessage(ComboBox cbo, string PlayerDescription, bool isAudio)
        {
            if (isAudio)
                oTcp.SendAudioMessage(GetSelectedItemStringFromComboBox(cbo), "StopApp|VoxApp");
            else
                oTcp.SendVideoMessage(GetSelectedItemStringFromComboBox(cbo), "StopApp|GameApp");
        }

        private void StartAudio_Click(object sender, RoutedEventArgs e)
        {
            string strServer = GetGameServerItem();

            bool blSuccess = true;
            blSuccess = SendAudioMessage(cboPlayer1Name, "Player1", strServer, GetSelectedItemStringFromComboBox(cboPlayer1Audio));
            blSuccess = SendAudioMessage(cboPlayer2Name, "Player2", strServer, GetSelectedItemStringFromComboBox(cboPlayer2Audio));
            blSuccess = SendAudioMessage(cboPlayer3Name, "Player3", strServer, GetSelectedItemStringFromComboBox(cboPlayer3Audio));
            blSuccess = SendAudioMessage(cboPlayer4Name, "Player4", strServer, GetSelectedItemStringFromComboBox(cboPlayer4Audio));
         
            if (blSuccess)
                MessageBox.Show("Audio Successfully Started");
        }

        private void StartVideo_Click(object sender, RoutedEventArgs e)
        {
            string strServer = GetGameServerItem();

            bool blSuccess = true;
            blSuccess = SendVideoMessage(cboPlayer1Name, "Player1", strServer, GetSelectedItemStringFromComboBox(cboPlayer1Video));
            blSuccess = SendVideoMessage(cboPlayer2Name, "Player2", strServer, GetSelectedItemStringFromComboBox(cboPlayer2Video));
            blSuccess = SendVideoMessage(cboPlayer3Name, "Player3", strServer, GetSelectedItemStringFromComboBox(cboPlayer3Video));
            blSuccess = SendVideoMessage(cboPlayer4Name, "Player4", strServer, GetSelectedItemStringFromComboBox(cboPlayer4Video));
         
            if (blSuccess)
                MessageBox.Show("Game Client Successfully Started");
        }

        private bool SendVideoMessage(ComboBox cbo, string strPlayerDescription, string strGameServer, string strGameClientComputer)
        {
            if (string.IsNullOrEmpty(strGameClientComputer))
            {
                MessageBox.Show(string.Format("Please Select a Value for {0}", strPlayerDescription));
                return false;
            }
           
            List<string> lstArguments = new List<string>();
            lstArguments.Add("StartApp"); // command name
            lstArguments.Add("GameApp"); // app name
            string strName = GetPseudonymFromComboBox(cbo);
            lstArguments.Add(strName); // player alias
            lstArguments.Add(string.Format("-playerName {0} -teamNumber 1 -connect {1}", strName, strGameServer)); //command line arguments
            oTcp.SendVideoMessage(strGameClientComputer, lstArguments);
            return true;
        }

        private bool SendAudioMessage(ComboBox cbo, string strPlayerDescription, string strGameServer, string strAudioClientComputer)
        {
            if (string.IsNullOrEmpty(strAudioClientComputer))
            {
                MessageBox.Show(string.Format("Please Select a Value for {0}", strPlayerDescription));
                return false;
            }
            
            List<string> lstArguments = new List<string>();
            lstArguments.Add("StartApp"); // command name
            lstArguments.Add("VoxApp"); // app name
            string strAlias = GetPseudonymFromComboBox(cbo);
            string strName = GetObjectNameFromComboBox(cbo);
            lstArguments.Add(strName); // player alias
            lstArguments.Add(string.Format("-name \"{0}\" -alias \"{1}\"", strName, strAlias)); //command line arguments
            oTcp.SendAudioMessage(strAudioClientComputer, lstArguments);
            return true;
        }

        /// <summary>
        /// Gets the object name from the def_Object for a selected combobox
        /// </summary>
        /// <param name="cbo"></param>
        /// <returns></returns>
        private string GetObjectNameFromComboBox(ComboBox cbo)
        {
            if (cbo.Items.Count > 0 && cbo.SelectedItem != null)
                return ((def_Object)(cbo.SelectedItem)).ObjectName;
            else
                return null;
        }

        /// <summary>
        /// This returns the selected server. 
        /// </summary>
        /// <returns></returns>
        private string GetGameServerItem()
        {
            if (cboGameServerName.SelectedItem == null)
            {
                btnStartServer.IsEnabled = false;

                return null;
            }

            string strValue;
            strValue = cboGameServerName.SelectedItem as string;
            if (strValue == null)
                strValue = ((ComboBoxItem)(cboGameServerName.SelectedItem)).Content.ToString();

            btnStartServer.IsEnabled = true;
            return strValue;
        }

        private void ConnectToGameServerHost_Click(object sender, RoutedEventArgs e)
        {
            List<string> lstArguments = new List<string>();
            lstArguments.Add("StartApp"); // command name
            lstArguments.Add("GameApp"); // app name
            lstArguments.Add("Server"); // player alias
            lstArguments.Add(string.Format("-playerName Server -teamNumber 0 -mission {0}", GetSelectedItemStringFromComboBox(cboGameType))); //command line arguments

            string strValue = GetGameServerItem();

            if (string.IsNullOrEmpty(strValue))
            {
                MessageBox.Show("Please Select a Server");
                return;
            }
            else
                oTcp.SendVideoMessage(strValue, lstArguments);
                
            System.Threading.Thread.Sleep(12000);

            oTcp.ConnectToGameServer(strValue);

            using (SqlConnection cn = new SqlConnection(_conStr))
            {
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.CommandText = "dbo.usp_GameControlPanel_Insert";
                    cn.Open();

                    cm.Parameters.AddWithValue("@GameId", oTcp.CurrentGameId);
                    cm.Parameters.AddWithValue("@PlayerName", GetObjectNameFromComboBox(cboPlayer1Name));
                    cm.Parameters.AddWithValue("@PlayerPseudonim", GetPseudonymFromComboBox(cboPlayer1Name));
                    cm.Parameters.AddWithValue("@AudioClientComputer", GetSelectedItemStringFromComboBox(cboPlayer1Audio));
                    cm.Parameters.AddWithValue("@GameClientComputer", GetSelectedItemStringFromComboBox(cboPlayer1Video));
                    cm.ExecuteNonQuery();

                    cm.Parameters["@PlayerName"].Value = GetObjectNameFromComboBox(cboPlayer2Name);
                    cm.Parameters["@PlayerPseudonim"].Value = GetPseudonymFromComboBox(cboPlayer2Name);
                    cm.Parameters["@AudioClientComputer"].Value = GetSelectedItemStringFromComboBox(cboPlayer2Audio);
                    cm.Parameters["@GameClientComputer"].Value = GetSelectedItemStringFromComboBox(cboPlayer2Video);
                    cm.ExecuteNonQuery();

                    cm.Parameters["@PlayerName"].Value = GetObjectNameFromComboBox(cboPlayer3Name);
                    cm.Parameters["@PlayerPseudonim"].Value = GetPseudonymFromComboBox(cboPlayer3Name);
                    cm.Parameters["@AudioClientComputer"].Value = GetSelectedItemStringFromComboBox(cboPlayer3Audio);
                    cm.Parameters["@GameClientComputer"].Value = GetSelectedItemStringFromComboBox(cboPlayer3Video);
                    cm.ExecuteNonQuery();

                    cm.Parameters["@PlayerName"].Value = GetObjectNameFromComboBox(cboPlayer4Name);
                    cm.Parameters["@PlayerPseudonim"].Value = GetPseudonymFromComboBox(cboPlayer4Name);
                    cm.Parameters["@AudioClientComputer"].Value = GetSelectedItemStringFromComboBox(cboPlayer4Audio);
                    cm.Parameters["@GameClientComputer"].Value = GetSelectedItemStringFromComboBox(cboPlayer4Video);
                    cm.ExecuteNonQuery();
                }
            }

            btnStartLab.IsEnabled = true;
            btnStartVOX.IsEnabled = true;
            btnStopLab.IsEnabled = true;
            btnStopVOX.IsEnabled = true;

            MessageBox.Show("Server Successfully Started");
        }
        
        private void StopAudio_Click(object sender, RoutedEventArgs e)
        {
            SendStopServiceMessage(cboPlayer1Audio, "Player 1", true);
            SendStopServiceMessage(cboPlayer2Audio, "Player 2", true);
            SendStopServiceMessage(cboPlayer3Audio, "Player 3", true);
            SendStopServiceMessage(cboPlayer4Audio, "Player 4", true);
            
            MessageBox.Show("VOX Successfully Shutdown");
        }

        private void StopVideo_Click(object sender, RoutedEventArgs e)
        {
            SendStopServiceMessage(cboPlayer1Video, "Player 1", false);
            SendStopServiceMessage(cboPlayer2Video, "Player 2", false);
            SendStopServiceMessage(cboPlayer3Video, "Player 3", false);
            SendStopServiceMessage(cboPlayer4Video, "Player 4", false);
            
            SendStopServiceMessage(cboGameServerName, "the Server", false);

            MessageBox.Show("Game Clients Successfully Shutdown");
        }

        private string GetStatus(string strMessage)
        {
            if (strMessage.StartsWith("Status"))
                return strMessage.Substring(6);
            else
                return strMessage;
        }

        private void GetMediaStreamerStatus_Click(object sender, RoutedEventArgs e)
        {
            const string strGetStatus = "GetStatus|{0}";
            try
            {
                lblMainServerStatus.Content = GetStatus(oTcp.SendReceiveMainServerMessage(strGetStatus));

                lblAudio1Status.Text = GetStatus(oTcp.SendReceiveAudioMessage(GetSelectedItemStringFromComboBox(cboPlayer1Audio), strGetStatus));
                lblAudio2Status.Text = GetStatus(oTcp.SendReceiveAudioMessage(GetSelectedItemStringFromComboBox(cboPlayer2Audio), strGetStatus));
                lblAudio3Status.Text = GetStatus(oTcp.SendReceiveAudioMessage(GetSelectedItemStringFromComboBox(cboPlayer3Audio), strGetStatus));
                lblAudio4Status.Text = GetStatus(oTcp.SendReceiveAudioMessage(GetSelectedItemStringFromComboBox(cboPlayer4Audio), strGetStatus));
             
                lblVideo1Status.Text = GetStatus(oTcp.SendReceiveVideoMessage(GetSelectedItemStringFromComboBox(cboPlayer1Video), strGetStatus));
                lblVideo2Status.Text = GetStatus(oTcp.SendReceiveVideoMessage(GetSelectedItemStringFromComboBox(cboPlayer2Video), strGetStatus));
                lblVideo3Status.Text = GetStatus(oTcp.SendReceiveVideoMessage(GetSelectedItemStringFromComboBox(cboPlayer3Video), strGetStatus));
                lblVideo4Status.Text = GetStatus(oTcp.SendReceiveVideoMessage(GetSelectedItemStringFromComboBox(cboPlayer4Video), strGetStatus));
             
                lblVideoServerStatus.Text = GetStatus(oTcp.SendReceiveVideoMessage(GetSelectedItemStringFromComboBox(cboGameServerName), strGetStatus));
            }
            catch
            {
                throw (new Exception("GetMediaStreamStatus failed."));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (oTcp != null)
            {
                oTcp.Dispose();
                oTcp = null;
            }
        }

        private void cboGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ValidatePlayerRoster_Click(object sender, RoutedEventArgs e)
        {
            if (((ControlPanel.def_Object)cboPlayer1Name.SelectedItem).playerNumber == 1)
            {
                if (((ControlPanel.def_Object)cboPlayer2Name.SelectedItem).playerNumber == 2)
                {
                    if (((ControlPanel.def_Object)cboPlayer3Name.SelectedItem).playerNumber == 3)
                    {
                        if (((ControlPanel.def_Object)cboPlayer4Name.SelectedItem).playerNumber == 4)
                        {
                            MessageBox.Show("Player assignments seem okay.  Do session numbers match?");                
                            return;
                        }
                    }
                }
            }
            
            MessageBox.Show("Player assignments seem funky");
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            List<string> lstArguments = new List<string>();
            lstArguments.Add("StartApp"); // command name
            lstArguments.Add("GameApp"); // app name
            lstArguments.Add("Server"); // player alias
            lstArguments.Add(string.Format("-playerName Server -teamNumber 0 -mission {0}", GetSelectedItemString(cboGameType))); //command line arguments

            string strValue = GetGameServerItem();

            if (string.IsNullOrEmpty(strValue))
            {
                MessageBox.Show("Please Select a Server");
                return;
            }
            else
                oTcp.SendVideoMessage(strValue, lstArguments);
            System.Threading.Thread.Sleep(12000);

            oTcp.ConnectToGameServer(strValue);

            using (SqlConnection cn = new SqlConnection(_conStr))
            {
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.CommandText = "dbo.usp_GameControlPanel_Insert";
                    cn.Open();

                    cm.Parameters.AddWithValue("@GameId", oTcp.CurrentGameId);
                    cm.Parameters.AddWithValue("@PlayerName", GetObjectNameFromComboBox(cboPlayer1Name));
                    cm.Parameters.AddWithValue("@PlayerPseudonim", GetPseudonymFromComboBox(cboPlayer1Name));
                    cm.Parameters.AddWithValue("@AudioClientComputer", GetSelectedItemString(cboPlayer1Audio));
                    cm.Parameters.AddWithValue("@GameClientComputer", GetSelectedItemString(cboPlayer1Video));
                    cm.ExecuteNonQuery();

                    cm.Parameters["@PlayerName"].Value = GetObjectNameFromComboBox(cboPlayer2Name);
                    cm.Parameters["@PlayerPseudonim"].Value = GetPseudonymFromComboBox(cboPlayer2Name);
                    cm.Parameters["@AudioClientComputer"].Value = GetSelectedItemString(cboPlayer2Audio);
                    cm.Parameters["@GameClientComputer"].Value = GetSelectedItemString(cboPlayer2Video);
                    cm.ExecuteNonQuery();

                    cm.Parameters["@PlayerName"].Value = GetObjectNameFromComboBox(cboPlayer3Name);
                    cm.Parameters["@PlayerPseudonim"].Value = GetPseudonymFromComboBox(cboPlayer3Name);
                    cm.Parameters["@AudioClientComputer"].Value = GetSelectedItemString(cboPlayer3Audio);
                    cm.Parameters["@GameClientComputer"].Value = GetSelectedItemString(cboPlayer3Video);
                    cm.ExecuteNonQuery();

                    cm.Parameters["@PlayerName"].Value = GetObjectNameFromComboBox(cboPlayer4Name);
                    cm.Parameters["@PlayerPseudonim"].Value = GetPseudonymFromComboBox(cboPlayer4Name);
                    cm.Parameters["@AudioClientComputer"].Value = GetSelectedItemString(cboPlayer4Audio);
                    cm.Parameters["@GameClientComputer"].Value = GetSelectedItemString(cboPlayer4Video);
                    cm.ExecuteNonQuery();
                }
            }

            btnStartLab.IsEnabled = true;
            btnStartVOX.IsEnabled = true;
            btnStopLab.IsEnabled = true;
            btnStopVOX.IsEnabled = true;

            MessageBox.Show("Server Successfully Started");
        }

        private void GetAllAvailableAppHostNames_Click(object sender, RoutedEventArgs e)
        {
            AddAudioStreamerSocketsReportedAvailableByTcp();
            AddVideoStreamersReportedAvailableByTcp();

        }
    }
}