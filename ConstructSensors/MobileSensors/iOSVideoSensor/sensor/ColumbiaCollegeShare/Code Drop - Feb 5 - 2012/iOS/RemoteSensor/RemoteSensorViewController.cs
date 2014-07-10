using System;
using System.Text;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using SensorClient;
using SensorSharedTypes;

namespace RemoteSensor
{
	public partial class RemoteSensorViewController : UIViewController
	{
		private UIBarButtonItem buttonSettings;
		private UIBarButtonItem buttonStartStop;

		// settings dialog members
		private DialogViewController settingsDialogController = null;
		private EntryElement urlElement = null;
		private BooleanElement debugEnabledElement = null;
		private StringElement cameraSelectorElement = null;
		
		private UITextView textView = null;
		private CaptureManager captureManager = null;
		private bool isMonitoringCommands = false;
		private SensorClient.SensorClient client = null;
		
		private ConcurrentQueue<string> messages = new ConcurrentQueue<string>();
		
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public RemoteSensorViewController ()
			: base (UserInterfaceIdiomIsPhone ? "RemoteSensorViewController_iPhone" : "RemoteSensorViewController_iPad", null)
		{
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			setupToolBar();
			setupTextView();
			
			// subscribe to the popped controller event
			((RootViewController)NavigationController).ViewControllerPopped += handleViewControllerPopped;
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			textView.Frame = getTextViewRectForOrientation();
			scrollMessageViewToEnd();
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. this.myOutlet = null;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			if (UserInterfaceIdiomIsPhone) 
			{
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} 
			else 
			{
				return true;
			}
		}
		
		private bool capturing
		{
			get
			{
				return (captureManager != null && captureManager.IsCapturing);
			}
		}
		
		void handleViewControllerPopped (object sender, RootViewController.ViewControllerPoppedEventArgs e)
		{
			if ( e.Controller == settingsDialogController )
			{
				harvestDialogChoices();
			}
		}
		
		private void setupToolBar()
		{
			this.NavigationController.ToolbarHidden = false;
			this.NavigationController.Toolbar.BarStyle = UIBarStyle.Black;
			
			createSettingsBarButtonItem();
			createStartStopBarButtonItem();
			
			// add some space between buttons
			UIBarButtonItem fixedSpace = new UIBarButtonItem( UIBarButtonSystemItem.FixedSpace, null);
			fixedSpace.Width = 25;
			
			this.ToolbarItems = new[] {	buttonSettings, fixedSpace, buttonStartStop };
		}
		
		private void setupTextView()
		{
			if ( textView == null )
			{
				textView = new UITextView ( getTextViewRectForOrientation() );
				textView.Font = UIFont.FromName("Courier", 16);
				textView.ScrollEnabled = true;
				textView.Editable = false;
				textView.BackgroundColor = UIColor.FromRGB(240,240,240);
				this.View.AddSubview (textView);				
			}
		}
		
		private RectangleF getTextViewRectForOrientation()
		{
			int width = (int)(this.View.Frame.Width * .95);
			int height = (int)(this.View.Frame.Height * .95);
			int xOffset = (int)((this.View.Frame.Width - width) / 2);
			int yOffset = (int)((this.View.Frame.Height - height) / 2);
			
			return new RectangleF( xOffset, yOffset, width, height );
		}
		
		private void editSettings()
		{
			// create elements that we can reference later when harvesting results
			urlElement = new EntryElement("Service URL", Settings.WebServiceUrl, Settings.WebServiceUrl );
			debugEnabledElement = new BooleanElement("Debug Enabled", Settings.DebugEnabled );
			cameraSelectorElement = new StringElement("Camera", () => 
			{
				Settings.ToggleCamera();
				repopulateDialog();
			});
			cameraSelectorElement.Value = getCameraElementText();
			
			var menu = new RootElement ("Settings")
			{
				new Section ("Connection")
				{
					urlElement,
				},
				new Section ("Application")
				{
					debugEnabledElement,
					cameraSelectorElement,
					new StringElement( "Reset Settings", ()=>
					{
						Settings.Reset();
						repopulateDialog();
					})
				},
			};
			
			// show the settings UI
			settingsDialogController = new DialogViewController (menu, true);
			NavigationController.PushViewController (settingsDialogController, true);				
		}
		
		private string getCameraElementText()
		{
			if ( Settings.Camera == Settings.CameraType.FrontFacing )
			{
				return "Front (press to change)";
			}
			else
			{
				return "Rear (press to change)";
			}
		}
		
		private void repopulateDialog()
		{
			urlElement.Value = Settings.WebServiceUrl;
			debugEnabledElement.Value = Settings.DebugEnabled;
			cameraSelectorElement.Value = getCameraElementText();

			settingsDialogController.ReloadData();
		}
		
		private void harvestDialogChoices()
		{
			// harvest the dialog results
			Settings.DebugEnabled = debugEnabledElement.Value;
			Settings.WebServiceUrl = urlElement.Value;
			
			Settings.Save();
		}
		
		private void createSettingsBarButtonItem()
		{
			UIButton button = new UIButton();
			UIImage imageForButton = UIImage.FromFile("./Images/Gear.png");
			button.SetImage( imageForButton, UIControlState.Normal );
			button.SetImage( imageForButton, UIControlState.Selected );
			button.SetImage( imageForButton, UIControlState.Highlighted );
			button.Frame = new RectangleF( 0, 0, imageForButton.Size.Width, imageForButton.Size.Height );
			button.Enabled = true;
			buttonSettings = new UIBarButtonItem( button );
			button.TouchUpInside += (s,e) =>
			{
				editSettings();
			};
			buttonSettings.CustomView = button;
			buttonSettings.Enabled = true;
		}
		
		private void createStartStopBarButtonItem()
		{
			UIButton button = new UIButton();
			UIImage imageForButton = UIImage.FromFile("./Images/Start.png");
			button.SetImage( imageForButton, UIControlState.Normal );
			button.SetImage( imageForButton, UIControlState.Selected );
			button.SetImage( imageForButton, UIControlState.Highlighted );
			button.Frame = new RectangleF( 0, 0, imageForButton.Size.Width, imageForButton.Size.Height );
			button.Enabled = true;
			buttonStartStop = new UIBarButtonItem( button );
			button.TouchUpInside += (s,e) =>
			{
				startStop();
			};
			buttonStartStop.CustomView = button;
			buttonStartStop.Enabled = true;
		}

		private void restyleStartStopButton( string imageFile )
		{
			UIImage imageForButton = UIImage.FromFile( imageFile );
			UIButton button = buttonStartStop.CustomView as UIButton;
			button.SetImage( imageForButton, UIControlState.Normal );
			button.SetImage( imageForButton, UIControlState.Selected );
			button.SetImage( imageForButton, UIControlState.Highlighted );
		}
		
		private bool buttonHandlerInProgress = false;
		private void startStop()
		{
			try
			{
				if ( buttonHandlerInProgress == true )
				{
					return;
				}
				buttonHandlerInProgress = true;
				startStopInternal();
			}
			finally
			{
				buttonHandlerInProgress = false;
			}
		}

		private void startStopInternal()
		{
			// initialize the client to service connection if needed
			if ( client == null )
			{
				logMessage("creating and configuring web service client");
				
				// set max parts per stream to a value which will approximate 1 minute long streams
				uint maxPartsPerStream = (uint)((1000.0 / CaptureManager.FragmentDuration) * 60);
				client = new SensorClient.SensorClient( Settings.WebServiceUrl, AppData.SensorID, AppData.DisplayName, maxPartsPerStream, 5000 );
				
				client.Uploader.StreamPartUploaded += new EventHandler<StreamPartUploadedEventArgs>( handlePartUploaded );
				client.Uploader.StreamPartUploadFailed += new EventHandler<StreamPartUploadFailedEventArgs>( handlePartUploadFailed );
				client.Uploader.StreamPartUploadDropped += new EventHandler<StreamPartDroppedEventArgs>( handlePartUploadDropped );
				client.CommandMonitor.SensorCommandReceived += new EventHandler<SensorCommandReceivedEventArgs>( handleSensorCommandReceived );
				
				client.SensorConnected += new EventHandler( handleSensorConnected );
				client.SensorDisconnected += new EventHandler( handleSensorDisconnected );
				client.SensorConnectFailed += new EventHandler<SensorConnectFailedEventArgs>( handleSensorConnectFailed );
				client.SensorDisconnectFailed += new EventHandler<SensorDisconnectFailedEventArgs>( handleSensorDisconnectFailed );
				
				logMessage("client configured");
			}
			
			// start/stop
			if ( isMonitoringCommands == false)
			{
				restyleStartStopButton( "./Images/Pause.png" );
				isMonitoringCommands = true;

				connect();

				logMessage("starting command listener ...");
				client.CommandMonitor.Start();
				logMessage("command listener started");
				
			}
			else
			{
				restyleStartStopButton( "./Images/Start.png" );
				isMonitoringCommands = false;

				logMessage("Stopping command listener ... ");
				client.CommandMonitor.Stop();
				logMessage("command listener stopped");
				
				disconnect();
				
				stopStreaming();
			}
		}
		
		
		private void connect()
		{
			if ( client == null )
			{
				return;
			}
			string message;
			logMessage("connecting ...");
			client.ConnectSensor( out message );
		}
		
		private void disconnect()
		{
			if ( client == null )
			{
				return;
			}
			logMessage("disconnecting ...");
			string message;
			client.DisconnectSensor( out message );
		}
		
		private void handleSensorCommandReceived( object sender, SensorCommandReceivedEventArgs args )
		{
			logMessage ( "Command Received: " + args.Command );
			
			if ( args.Command == SensorCommand.None )
			{
				return;
			}

			BeginInvokeOnMainThread(delegate{
				if ( args.Command == SensorCommand.StopUpload )
				{
					stopStreaming();	
				}
				else
				{
					stopStreaming();
					stream( args.Command );	
				}
			});
		}

		private static Resolution ResolutionFromCommand(SensorCommand command)
		{
			if ( command == SensorCommand.StartUpload_LowRes )  
			{
				return Resolution.Low;
			}
			else
			{
				return Resolution.Medium;	
			}
		}
		
		private void stream( SensorCommand command )
		{
			if ( command == SensorCommand.StopUpload  )
			{
				return;
			}
			
			if ( captureManager == null )
			{
				logMessage("Creating and initializing capture graph");
				captureManager = new CaptureManager();
				captureManager.MediaFragmentCaptured += new EventHandler<MediaFragmentCapturedEventArgs>( handleMediaFragmentCaptured );
			}
			
			string errorMessage = null;
			logMessage("starting capture ...");
			if ( captureManager.StartCapture( ResolutionFromCommand(command),  out errorMessage ) == false )
			{
				logMessage( errorMessage );
				return;
			}
			logMessage ("capture started");
			
			logMessage("starting uploader...");
			client.Uploader.Start();
			logMessage("Uploader started");
		}

		private void stopStreaming()
		{
			if ( captureManager != null && captureManager.IsCapturing)
			{
				logMessage("stopping uploader...");
				client.Uploader.Stop();
				logMessage("uploader stopped");
				
				logMessage("stopping capture...");
				captureManager.StopCapture();
				logMessage("capture stopped");
			}
		}
		
		private void handleSensorConnected( object sender, EventArgs args )
		{
			logMessage ( "Sensor Connected");
		}
		
		private void handleSensorDisconnected( object sender, EventArgs args )
		{
			logMessage ( "Sensor Disconnected");
		}
		
		private void handleSensorConnectFailed( object sender, SensorConnectFailedEventArgs args )
		{
			logMessage ( "Sensor Connect Failed. " + args.Message);
		}
	
		private void handleSensorDisconnectFailed( object sender, SensorDisconnectFailedEventArgs args )
		{
			logMessage ( "Sensor Disconnect Failed. " + args.Message);
		}
		
		private StreamPart buildStreamPart(  MediaFragmentCapturedEventArgs args)
		{
			StreamPart part = new StreamPart();
			part.SensorID = AppData.SensorID.ToString();
			part.Base64Bytes = Convert.ToBase64String( File.ReadAllBytes( args.File ) );
			part.StartTime = args.StartedAt;
			part.DurationMilliSeconds = args.DurationMilliSeconds;
			part.FileName = Path.GetFileName( args.File );
			return part;
		}
		
		private void handleMediaFragmentCaptured( object sender, MediaFragmentCapturedEventArgs args )
		{
			client.Uploader.AddPart( buildStreamPart(args) );
			
			// delete the local file
			try
			{
				File.Delete( args.File );
			}
			catch
			{
			}
		}
		
		private void handlePartUploaded( object sender, StreamPartUploadedEventArgs args)
		{
			logMessage("Part uploaded.  Sequence Number = " + args.Part.SequenceNumber);
		}
		
		private void handlePartUploadFailed( object sender, StreamPartUploadFailedEventArgs args)
		{
			logMessage("stream part upload failed");
		}
		
		private void handlePartUploadDropped( object sender, StreamPartDroppedEventArgs args)
		{
			logMessage("Stream part dropped");
		}
	
		private void logMessage( string message )
		{
			if ( Settings.DebugEnabled == true )
			{
				DateTime time = DateTime.Now;
				string timeText = string.Format ("{0}:{1}:{2}.{3}", time.Hour, time.Minute.ToString().PadLeft(2,'0'), time.Second.ToString().PadLeft(2,'0'), time.Millisecond);
				string timeStampedMessage = string.Format ("{0}: {1}\r\n", timeText , message.TrimEnd());
				messages.Enqueue( timeStampedMessage );
				
				StringBuilder sb = new StringBuilder();
				string [] messageArray = messages.ToArray();
				foreach ( string m in messageArray )
				{
					sb.Append(m);
				}
				string text = sb.ToString();

				BeginInvokeOnMainThread(delegate{
					textView.Text = text;
					scrollMessageViewToEnd();
				});
				
			}
			
		}
		
		private void scrollMessageViewToEnd()
		{
			try
			{
				// find the number of characters between the end of the text and the previous newline
				string text = textView.Text.TrimEnd();
				int index = text.Length - 1;
				while (true)
				{
					char c = text[index];
					if ( c == '\r' || c == '\n' )
					{
						break;
					}
					index--;
				}
				textView.ScrollRangeToVisible( new NSRange( index + 1, 1 ) );
			}
			catch
			{
			}
		}
		
	}
}
