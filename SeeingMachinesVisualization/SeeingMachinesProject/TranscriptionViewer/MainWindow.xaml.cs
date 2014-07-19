using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TranscriptionViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//String constructDatabaseConnectionString = "data source=(local);initial catalog=Construct3;persist security info=True;Integrated Security=true;";
		String constructDatabaseConnectionString = "data source=daisy.colum.edu;initial catalog=Construct3;persist security info=True;user id=Construct;password=ConstructifyConstructifyConstructify";
		DatabaseTranscriptionSource transcriptionSource;

		private TextBlock GetTextBlockForStationName(String stationName)
		{
			switch (stationName)
			{
				case "Station 1":
					return TranscribedOutput1;
				case "Station 2":
					return TranscribedOutput2;
				case "Station 3":
					return TranscribedOutput3;
				case "Station 4":
					return TranscribedOutput4;
				default:
					return null;
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			transcriptionSource = new DatabaseTranscriptionSource(constructDatabaseConnectionString);
			transcriptionSource.NewTranscription += transcriptionSource_NewTranscription;
		}

		void transcriptionSource_NewTranscription(Guid sourceId, DateTime transcriptionTime, string transcription)
		{
			TranscriptionSourceTranslator sourceTranslator = new TranscriptionSourceTranslator();
			String sourceName = sourceTranslator.TranslateSourceToName(sourceId);

			//	Transcription wasn't from any of the 4 stations
			if (sourceName == null)
				return;

			String timeStampString = transcriptionTime.ToString();

			TextBlock outputBlock = GetTextBlockForStationName(sourceName);
			outputBlock.Dispatcher.Invoke(() => outputBlock.Text = timeStampString + ": " + transcription + "\n" + outputBlock.Text);

			TranscribedConversation.Dispatcher.Invoke(() => TranscribedConversation.Text = sourceName + ": " + transcription + "\n" + TranscribedConversation.Text);
		}
	}
}
