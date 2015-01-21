using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace Construct.UX.Views.Helper
{
	/// <summary>
	/// Interaction logic for SourceSelectionDialog.xaml
	/// </summary>
	public partial class SourceSelectionDialog : RadRibbonWindow
	{
		private ObservableCollection<DataPropertyViewModel> availableSources;

		public SourceSelectionDialog(IEnumerable<DataPropertyModel> availableSources, IEnumerable<DataPropertyModel> selectedSources = null)
		{
			InitializeComponent();

			this.availableSources = new ObservableCollection<DataPropertyViewModel>();

			foreach (var model in availableSources)
			{
				var newViewModel = new DataPropertyViewModel(model);
				if (selectedSources != null && selectedSources.Any((selectedModel) => selectedModel.Equals(model)))
					newViewModel.IsSelected = true;
				this.availableSources.Add(newViewModel);
			}

			this.PropertiesView.ItemsSource = this.availableSources;
			
			this.Closed += PropertySelectionDialog_Closed;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				var selectedItems = PropertiesView.SelectedItems.Cast<DataPropertyViewModel>().ToList();
				bool currentIsEnabled = selectedItems.All((model) => model.IsSelected);
				for (int i = 0; i < selectedItems.Count; i++)
					selectedItems[i].IsSelected = !currentIsEnabled; // Toggle the values
			}

			base.OnKeyDown(e);
		}

		void PropertySelectionDialog_Closed(object sender, EventArgs e)
		{
			if (!DialogResult.GetValueOrDefault(false))
				return;

			List<DataPropertyModel> result = (
				from propertyViewModel in availableSources
				where propertyViewModel.IsSelected
				select propertyViewModel.Model
				).ToList();

			SelectedSources = result;
		}

		public IEnumerable<DataPropertyModel> SelectedSources { get; private set; }

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}
	}
}
