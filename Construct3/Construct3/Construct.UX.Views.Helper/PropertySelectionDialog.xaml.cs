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
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Construct.UX.Views.Helper
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class PropertySelectionDialog : RadRibbonWindow
	{
		private ObservableCollection<DataPropertyViewModel> availableProperties;

		public PropertySelectionDialog(IEnumerable<DataPropertyModel> availableProperties, IEnumerable<DataPropertyModel> selectedProperties = null)
		{
			InitializeComponent();

			this.availableProperties = new ObservableCollection<DataPropertyViewModel>();

			foreach (var model in availableProperties)
			{
				var newViewModel = new DataPropertyViewModel(model);
				if (selectedProperties != null && selectedProperties.Any((selectedModel) => selectedModel.Equals(model)))
					newViewModel.IsSelected = true;
				this.availableProperties.Add(newViewModel);
			}

			this.PropertiesView.ItemsSource = this.availableProperties;
			
			this.Closed += PropertySelectionDialog_Closed;
		}

		void PropertySelectionDialog_Closed(object sender, EventArgs e)
		{
			if (!DialogResult.GetValueOrDefault(false))
				return;

			List<DataPropertyModel> result = (
				from propertyViewModel in availableProperties
				where propertyViewModel.IsSelected
				select propertyViewModel.Model
				).ToList();

			SelectedProperties = result;
		}

		public IEnumerable<DataPropertyModel> SelectedProperties { get; private set; }

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
