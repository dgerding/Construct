using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Construct.UX.ViewModels;
using NLog;
using Telerik.Windows.Controls;

namespace Construct.UX.WindowsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Telerik.Windows.Controls.RadRibbonWindow
    {
        private const int port = 8000;
        private const string hostname = "localhost";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            logger.Trace("in Construct.UX.ConstructWindows ctor...");
            CheckRuntimePrerequisites();

            InitializeComponent();
        }

        #region Application Startup and Initialization Members
        private void CheckRuntimePrerequisites()
        {
            /*  TODO: Reenable this when media player is actuall required
            // Check prerequisities
            if (!this.isWindowsMediaPlayerComponentInstalled())
            {
            MessageBox.Show(@"To use Contruct application you need to have Windows Media Player 10 or later.");   
            ShutdownApplication();
            }
            */
        }

        #endregion

        #region Debugging and Development Members

        private void WipeDatabaseAndRestoreDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Pressing YES will DELETE all data in the Server and reset it to its starting values.", "Construct Server Unavailable", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                credentialsViewModel.WipeDatabase(sessionInfo.ConnectionString);
                credentialsViewModel.DatabaseReset += () =>
                {
                    //DeleteAllUiRegions();
                    InvalidateUserIdentityAndSessionInfo();
                    RevertUiToNoUserOrServer();
                    MessageBox.Show("The working directory and database have been reset to starting and default values.", "Construct Server Reset", MessageBoxButton.OK);
                };
            }
        }

        private void LoadTestItemDataSet_Click(object sender, RoutedEventArgs e)
        {
            credentialsViewModel.LoadTestItems(sessionInfo.ConnectionString);
        }

        #endregion

        #region UI Initializations Members
        private void RadRibbonWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //sessionInfo.PropertyChanged += sessionInfo_PropertyChanged;
            transitionWin.Children.Clear();
            transitionWin.Children.Add(noConnectionPanel);

            if (!this.isApplicationCloseRequested)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                ShutdownApplication();
            }

            PrepareUiForInitialContextAndMakeLoginReady();

            sessionInfo.WorkingDirectoryPath = WorkingDirectoryPathTextBox.Text;

			//	!!!
			//LoginToServer_Click(null, null);
        }

        private void PrepareUiForInitialContextAndMakeLoginReady()
        {
            foreach (RadRibbonTab theTab in ShellRibbonBar1.Items)
            {
                theTab.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        #endregion

        #region Application Settings Members
        private void AppMenuSettingsAndPreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowUiRegionWhenRegionNotOnTab("Settings and Preferences");
        }

        private void AppMenuAboutButton_Click(object sender, RoutedEventArgs e)
        {
            //RibbonBar1.IsEnabled = Fa
            ShowUiRegionWhenRegionNotOnTab("About");
        }

        #endregion

        #region RibbonBar and Tabs Runtime-Related Members

        private List<TabPanel> tabPanels = new List<TabPanel>();

        private void BuildTabsFromTabPanels()
        {
            Cursor = Cursors.Wait;

            tabPanels.Sort(new TabPanelComparer());

            foreach (TabPanel tabPanel in tabPanels)
            {
                if (tabPanel.TabOrder == 0)
                {
                    //TODO: Add application menu items here dynamically?
                }
                else
                {
                    RadRibbonTab newTab = new RadRibbonTab();
                    newTab.Name = tabPanel.TabName;
                    newTab.Header = tabPanel.TabName;
                    newTab.TabIndex = tabPanel.TabOrder;

                    ShellRibbonBar1.Items.Add(newTab);
                }
            }

            Cursor = Cursors.Arrow;
        }

        private UserControl noConnectionPanel = new ShellLoginPanel();

        private int highestTabItemNumberForUndefinedRegions = 10;

        private void SetVisualStyleDefaults()
        {
            StyleManager.ApplicationTheme = new ModernTheme();
        }
        
        private int lastTab;
        private void ShowUiRegionWhenRegionNotOnTab(string regionName)
        {
            if (this.sessionInfo.areAllValuesReady())
            {
                TabPanel tabPanel = tabPanels.Find(r => r.TabName == regionName);
                transitionWin.Children.Clear();
                transitionWin.Children.Add(tabPanel.RegionControl);
                lastTab = -1;
            }
        }

        private void ShellRibbonBar1_CollapsedChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
        }

        private void ShowUiRegionBasedOnSelectedTab()
        {
            Cursor = Cursors.Wait;

            if (ShellRibbonBar1.SelectedTab == null)
            {
                transitionWin.Children.Clear();
                transitionWin.Children.Add(noConnectionPanel);
            }
            else
            {
                string regionTabName = ShellRibbonBar1.SelectedTab.Header.ToString();
                if (this.sessionInfo.areAllValuesReady())
                {
                    TabPanel theRegionPanel = tabPanels.Find(r => r.TabName == regionTabName);

                    if (theRegionPanel.RegionControl == null)
                    {
                        //theRegionPanel.RegionControl = CreateUiRegionByName(theRegionPanel.RegionName);
                    }

                    transitionWin.Children.Clear();
                    transitionWin.Children.Add(theRegionPanel.RegionControl);
                }
                else
                {
                    transitionWin.Children.Clear();
                    transitionWin.Children.Add(noConnectionPanel);
                }
            }
            Cursor = Cursors.Arrow;
        }

        private void ShellRibbonBar1_SelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            Cursor = Cursors.Wait;
            SetSelectedTabMovingDirectionTransition();

            ShowUiRegionBasedOnSelectedTab();

            Cursor = Cursors.Arrow;
        }

        private void SetSelectedTabMovingDirectionTransition()
        {
            /* TODO: See if the panning screens can return...
            if (this.lastTab < 0)
            {
            // unchanged 
            //TODO:Restore if can... transitionWin.Transition = new Telerik.Windows.Controls.TransitionEffects.FlipWarpTransition();
            return;
            }
            if (this.ShellRibbonBar1.SelectedTab.TabIndex == lastTab)
            {
            // unchanged 
            //TODO:Restore if can... transitionWin.Transition = new Telerik.Windows.Controls.TransitionEffects.MotionBlurredZoomTransition();
            return;
            }
            if (this.ShellRibbonBar1.SelectedTab.TabIndex > lastTab)
            {
            // moving right
            Telerik.Windows.Controls.TransitionEffects.SlideAndZoomTransition rm = new Telerik.Windows.Controls.TransitionEffects.SlideAndZoomTransition();
            rm.SlideDirection = FlowDirection.RightToLeft;
            //TODO:Restore if can... transitionWin.Transition = rm;
            return;
            }
            if (this.ShellRibbonBar1.SelectedTab.TabIndex < lastTab)
            {
            // moving left
            Telerik.Windows.Controls.TransitionEffects.SlideAndZoomTransition lm = new Telerik.Windows.Controls.TransitionEffects.SlideAndZoomTransition();
            lm.SlideDirection = FlowDirection.LeftToRight;
            //TODO:Restore if can... transitionWin.Transition = lm;
            }
            */
        }

        private void ShowAllUiTabs()
        {
            foreach (RadRibbonTab theTab in ShellRibbonBar1.Items)
            {
                theTab.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion

        #region Application Session, Credentials and Login Members
        private ApplicationSessionInfo sessionInfo = new ApplicationSessionInfo();
        private Construct.UX.ViewModels.Credentials.ViewModel credentialsViewModel = new ViewModels.Credentials.ViewModel();
        private bool isConstructServerAttached;

        public bool IsConstructServerAttached
        {
            get
            {
                return isConstructServerAttached;
            }
        }
        
        private void LoginToServer_Click(object sender, RoutedEventArgs e)
        {
            IsBusy = true;
            BusyContent = "Attempting to login...";
            ShellRibbonBar1.IsBackstageOpen = false;
            AttemptLogin(txtServer.Text, txtLogin.Text, txtPassword.Text);
        }

        private void AttemptLogin(string constructServerName, string userName, string password)
        {
            if (IsUserConnectedAndChoosingNewConnection())
            {
                //TODO: disconnect views and hide the tabs?
            }
            InvalidateUserIdentityAndSessionInfo();
            SetSuucessAndFailureHandlers(constructServerName, userName, password);
            credentialsViewModel.Login(constructServerName, userName, password);
        }
  
        private void SetSuucessAndFailureHandlers(string constructServerName, string userName, string password)
        {
            credentialsViewModel.LoginSuccess += (connectionString) =>
            {
	            try
	            {
		            SetSessionInfoForAuthenticatedUser(constructServerName, userName, password, connectionString,
			            WorkingDirectoryPathTextBox.Text);
		            BuildUiForAuthenticatedUser();
		            LogToList("User " + userName + " logged in.");
					WorkingDirectoryPathTextBox.IsEnabled = false;
					isConstructServerAttached = true;
	            }
	            catch (Exception e)
	            {
		            MessageBox.Show("An error occurred while logging in.\n\n" + e.Message);
		            if (Debugger.IsAttached)
			            throw;

					InvalidateUserIdentityAndSessionInfo();
	            }
            };
            credentialsViewModel.LoginError += AlertUserThatValidationServerIsUnavailable;
        }

        private bool IsUserConnectedAndChoosingNewConnection()
        {
            if (isConstructServerAttached)
            {
                if (MessageBox.Show("Are you sure you want to change your connected Construct Server? Doing so will close all existing work.", "ConstructServer Context Change Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private void InvalidateUserIdentityAndSessionInfo()
        {
            sessionInfo.IsDefaultPrincipalSet = false;
            //SessionInfo.Identity = null;
            sessionInfo.UserId = 0;
            sessionInfo.IsAuthenticated = false;
            sessionInfo.Password = "";
        }

        private void SetSessionInfoForAuthenticatedUser(string constructServerName, string userName, string password, string connectionString, string workingDirectory)
        {
            sessionInfo.IsAuthenticated = true;
            sessionInfo.Password = password;
            sessionInfo.ConnectionString = connectionString;
            sessionInfo.UserName = userName;
            sessionInfo.WorkingDirectoryPath = workingDirectory;
            sessionInfo.Port = port;
            sessionInfo.HostName = hostname;
        }

        private void BuildUiForAuthenticatedUser()
        {
            //TODO Need these?
            //BuildRegionPanelsCollectionFromDiscoveredAndUserRequestedDisplayRegions();
            //AddNativeRegionPanelsToCollection();
            CreateTabPanels();
            BuildTabsFromTabPanels();
            ShowAllUiTabs();

			LoginToServerButton.Visibility = Visibility.Hidden;
			DisconnectFromServerButton.Visibility = Visibility.Visible;
			WipeDatabaseAndRestoreDefaultsButton.Visibility = Visibility.Visible;
			LoadTestItemDataSet.Visibility = Visibility.Visible;

			WorkingDirectoryPathTextBox.IsEnabled = false;
			WorkingDirectoryPathTextBox.IsReadOnly = true;
        }

        private void CreateTabPanels()
        {
            CreateAndAddDataTabPanel();
            CreateAndAddSourcesTabPanel();
            CreateAndAddMeaningTabPanel();
            CreateAndAddQuestionsTabPanel();
            CreateAndAddVisualizationsTabPanel();
            CreateAndAddSessionsTabPanel();
            CreateAndAddLearningTabPanel();
        }

        private UserControl dataView;
        private void CreateAndAddDataTabPanel()
        {
            if (dataView != null)
            {
            }
            try
            {
                dataView = new Construct.UX.Views.Data.View(sessionInfo);
            }
            catch (Exception e)
            {
                logger.ErrorException("DataView construction exception.", e);
                throw;
            }
            TabPanel dataPanel = new TabPanel(dataView, "Data", "Data", "Data", 1, 0, "Data");
            tabPanels.Add(dataPanel);
        }

        private UserControl sourcesView;
        private void CreateAndAddSourcesTabPanel()
        {
            if (sourcesView != null)
            {
            }
            try
            {
                sourcesView = new Construct.UX.Views.Sources.View(sessionInfo);
            }
            catch (Exception e)
            {
                logger.ErrorException("SourcesView construction exception.", e);
                throw;
            }
            TabPanel sourcesPanel = new TabPanel(sourcesView, "Sources", "Sources", "Sources", 2, 0, "Sources");
            tabPanels.Add(sourcesPanel);
        }

        private UserControl meaningView;
        private void CreateAndAddMeaningTabPanel()
        {
            if (meaningView != null)
            {
            }
            try
            {
                meaningView = new Construct.UX.Views.Meaning.View(sessionInfo);
            }
            catch (Exception e)
            {
                logger.ErrorException("Meaning construction exception.", e);
                throw;
            }
            TabPanel meaningPanel = new TabPanel(meaningView, "Meaning", "Meaning", "Meaning", 3, 0, "Meaning");
            tabPanels.Add(meaningPanel);
        }

        private UserControl questionsView;
        private void CreateAndAddQuestionsTabPanel()
        {
            if (questionsView != null)
            {
            }
            try
            {
                questionsView = new Construct.UX.Views.Questions.View(sessionInfo);
            }
            catch (Exception e)
            {
                logger.ErrorException("Questions construction exception.", e);
                throw;
            }
            TabPanel questionsPanel = new TabPanel(questionsView, "Questions", "Questions", "Questions", 4, 0, "Questions");
            tabPanels.Add(questionsPanel);
        }

        private UserControl sessionsView;
        private void CreateAndAddSessionsTabPanel()
        {
            if (sessionsView != null)
            {
            }
            try
            {
                sessionsView = new Construct.UX.Views.Sessions.View(sessionInfo);
            }
            catch (Exception e)
            {
                logger.ErrorException("SessionsView construction exception.", e);
                throw;
            }
            TabPanel sessionsPanel = new TabPanel(sessionsView, "Sessions", "Sessions", "Sessions", 5, 0, "Sessions");
            tabPanels.Add(sessionsPanel);
        }

        private UserControl learningView;
        private void CreateAndAddLearningTabPanel()
        {
            if (learningView != null)
            {
            }
            try
            {
                learningView = new Construct.UX.Views.Learning.View(sessionInfo);
            }
            catch (Exception e)
            {
                logger.ErrorException("LearningView construction exception.", e);
                throw;
            }
            TabPanel learningPanel = new TabPanel(learningView, "Learning", "Learning", "Learning", 6, 0, "Learning");
            tabPanels.Add(learningPanel);
        }        
        private UserControl visualizationsView;
        private void CreateAndAddVisualizationsTabPanel()
        {
            if (visualizationsView != null)
            {
            }
            try
            {
                visualizationsView = new Construct.UX.Views.Visualizations.View(sessionInfo);
            }
            catch (Exception e)
            {
                logger.ErrorException("VisualizationsView construction exception.", e);
                throw;
            }
            TabPanel visualizationsPanel = new TabPanel(visualizationsView, "Visualizations", "Visualizations", "Visualizations", 7, 0, "Visualizations");
            tabPanels.Add(visualizationsPanel);
        }

        private void AlertUserThatValidationServerIsUnavailable()
        {
            MessageBox.Show("We couldn't connect to the Credential and Validation Server.", "Credential Server Unavailable", MessageBoxButton.OK);
        }

        private void AlertUserThatCredentialsAreBad()
        {
            MessageBox.Show("The credentials you provided are not authorized or are incorrect.", "Bad Username or Password", MessageBoxButton.OK);
        }

        private void AlertUserThatConstructServerIsUnavailable()
        {
            MessageBox.Show("You are authorized to connect to that Construct Server but we couldn't connect to it.", "Construct Server Unavailable", MessageBoxButton.OK);
        }

        private void DisconnectFromServer_Click(object sender, RoutedEventArgs e)
        {
            InvalidateUserIdentityAndSessionInfo();

            RevertUiToNoUserOrServer();
        }

        private void RevertUiToNoUserOrServer()
        {
            LoginToServerButton.Visibility = Visibility.Visible;
            DisconnectFromServerButton.Visibility = Visibility.Hidden;
            WipeDatabaseAndRestoreDefaultsButton.Visibility = Visibility.Hidden;
            LoadTestItemDataSet.Visibility = Visibility.Hidden;

            WorkingDirectoryPathTextBox.IsEnabled = true;
            WorkingDirectoryPathTextBox.IsReadOnly = false;
            //TODO Okay?
            //DeleteAllUiRegions();
        }

        #endregion

        #region Runtime Logging and SaaS Metrics

        private void LogToList(string theMessage)
        {
            LastLogMessageLabel.Content = theMessage;
            ListBoxItem theMessageItem = new ListBoxItem();
            theMessageItem.Content = DateTime.Now.ToString() + ":" + theMessage;
            LogListBox.Items.Add(theMessageItem);
        }

        #endregion

        #region Busy Indicator Handling
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    //AppWideBusyIndictor.IsBusy = value;
                    //OnPropertyChanged(() => this.IsBusy);

                    /*
                    if (isBusy)
                    {
                        var backgroundWorker = new BackgroundWorker();
                        backgroundWorker.DoWork += this.OnBackgroundWorkerDoWork;
                        backgroundWorker.RunWorkerCompleted += OnBackgroundWorkerRunWorkerCompleted;
                        backgroundWorker.RunWorkerAsync();
                    }
                     */
                }
            }
        }

        private string busyContent;
        public string BusyContent
        {
            get { return this.busyContent; }
            set
            {
                if (this.busyContent != value)
                {
                    this.busyContent = value;
                    //this.OnPropertyChanged(() => this.BusyContent);
                }
            }
        }

        private void OnBackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;
            backgroundWorker.DoWork -= this.OnBackgroundWorkerDoWork;
            backgroundWorker.RunWorkerCompleted -= OnBackgroundWorkerRunWorkerCompleted;

            /*
            InvokeOnUIThread(() =>
            {
                
                //this.Appointments = new ObservableCollection<Appointment>((IEnumerable<Appointment>)e.Result);
            });
            */
            this.IsBusy = false;
        }

        private void OnBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int x = 3;
            //e.Result = this.GenerateRandomAppointments().ToList();
        }


        #endregion 

        #region Application Shutdown

        private bool isApplicationCloseRequested = false;

        private void RadRibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isApplicationCloseRequested = true;

            //TODO: Handle shutdown denial, user overrides, and cleanup in ShutDownApplication
            ShutdownApplication();
        }

        private static void ShutdownApplication()
        {
            App.Current.Shutdown(1);
        }
        #endregion
    }
}