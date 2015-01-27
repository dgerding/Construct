using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
	/// <summary>
	/// Interaction logic for SaveVisualizationDialog.xaml
	/// </summary>
	public partial class SaveVisualizationDialog : Window
	{
		private ObservableCollection<Visualizer> allKnownVisualizers;
		private IEnumerable<Visualizer> savedVisualizers;

		public Visualizer Result { get; private set; }

		public SaveVisualizationDialog(IEnumerable<Visualizer> existingVisualizers, Visualizer selectedVisualizer)
		{
			InitializeComponent();

			Result = null;
			savedVisualizers = existingVisualizers;
			allKnownVisualizers = new ObservableCollection<Visualizer>(existingVisualizers);

			Result = new Visualizer()
			{
				Description = selectedVisualizer.Description,
				ID = selectedVisualizer.ID,
				LayoutString = selectedVisualizer.LayoutString,
				Name = selectedVisualizer.Name
			};

			//	If it's a new visualizer that hasn't been saved yet
			if (!existingVisualizers.Any(v => v.ID == selectedVisualizer.ID))
			{
				Result.Name = "My New Visualization ";
				//	Make sure to give it a unique name
				Result.Name +=
					Enumerable.Range(1, 99).First(n => !existingVisualizers.Any(v => v.Name == Result.Name + n));

				Result.Description = "My new visualization description... ";

				allKnownVisualizers.Add(Result);
			}

			VisNameBox.Text = Result.Name;
			VisDescriptionBox.Text = Result.Description;
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			var userName = VisNameBox.Text;
			var userDescription = VisDescriptionBox.Text;
			Visualizer existingVisualizerWithName = savedVisualizers.SingleOrDefault(v => v.Name == userName);
			if (existingVisualizerWithName != null)
			{
				var result = MessageBox.Show(String.Format("Are you sure you want to overwrite the previously-saved visualization named {0}?", userName), "", MessageBoxButton.YesNo);

				if (result == MessageBoxResult.No)
					return;
				else
					Result.ID = existingVisualizerWithName.ID;
			}

			Result.Name = userName;
			Result.Description = userDescription;

			DialogResult = true;
			this.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			this.Close();
		}
	}
}
