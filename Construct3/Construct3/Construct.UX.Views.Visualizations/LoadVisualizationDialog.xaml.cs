using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Construct.UX.Views.Helper;
using Telerik.Windows.Controls;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace Construct.UX.Views.Visualizations
{
	/// <summary>
	/// Interaction logic for SourceSelectionDialog.xaml
	/// </summary>
	public partial class LoadVisualizationDialog : RadRibbonWindow
	{
		private ObservableCollection<Visualizer> availableVisualizers;

		public event Action<Visualizer> OnVisualizerDeleted;

		public LoadVisualizationDialog(IEnumerable<Visualizer> availableVisualizers)
		{
			InitializeComponent();

			this.availableVisualizers = new ObservableCollection<Visualizer>(availableVisualizers);

			this.VisualizersList.ItemsSource = this.availableVisualizers;
		}

		public Visualizer SelectedVisualizer { get; private set; }

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			SelectedVisualizer = VisualizersList.SelectedItem as Visualizer;

			this.DialogResult = true;
			this.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void VisualizersList_SelectionChanged(object sender, SelectionChangeEventArgs e)
		{
			OkButton.IsEnabled = VisualizersList.SelectedItem != null;
		}

		private void DeleteVisualizerButton_OnClick(object sender, RoutedEventArgs e)
		{
			var visualizerToDelete = (sender as RadButton).GetVisualParent<RadRowItem>().Item as Visualizer;
			try
			{
				if (OnVisualizerDeleted != null)
					OnVisualizerDeleted(visualizerToDelete);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached)
					throw;
				else
					MessageBox.Show("Unable to delete visualization. " + ex.Message);
				return;
			}

			availableVisualizers.Remove(visualizerToDelete);
		}
	}
}
