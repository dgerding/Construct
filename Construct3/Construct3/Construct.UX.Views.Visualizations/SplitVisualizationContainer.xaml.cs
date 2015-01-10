using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Construct.UX.Views.Helper;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
	/// <summary>
	/// Interaction logic for SplitVisualizationContainer.xaml
	/// </summary>
	public partial class SplitVisualizationContainer : UserControl
	{
		public bool EnablePropertySelectionDialog { get; protected set; }

		double splitterPosition;
		public double SplitterPosition
		{
			get { return splitterPosition; }
			set
			{
				if (Math.Abs(splitterPosition - value) < 0.0001)
					return;

				splitterPosition = value;
				PreviewColumn.Width = new GridLength(splitterPosition, GridUnitType.Star);
				DetailsColumn.Width = new GridLength(1.0 - splitterPosition, GridUnitType.Star);
			}
		}

		public event Action<object, double> SplitPositionChanged;

		private List<DataPropertyModel> selectedProperties = new List<DataPropertyModel>();
		public List<DataPropertyModel> SelectedProperties { get { return selectedProperties; } } 

		private SubscriptionTranslator subscriptionTranslator;

		public IEnumerable<Type> VisualizableTypes
		{
			get { return PreviewVisualization.VisualizableTypes; }
		}

		public PropertyVisualization PreviewVisualization
		{
			get { return PreviewContainer.Children[0] as PropertyVisualization; }
		}

		public PropertyVisualization DetailsVisualization
		{
			get { return DetailsContainer.Children[0] as PropertyVisualization; }
		}

		public String VisualizationName { get; protected set; }

		public SplitVisualizationContainer(SubscriptionTranslator translator)
		{
			InitializeComponent();
			
			Splitter.DragCompleted += Splitter_DragCompleted;
			SplitterPosition = 0.5;

			this.EnablePropertySelectionDialog = true;

			this.ContextMenu = new ContextMenu();
			var showPropertyMenuItem = new MenuItem();
			showPropertyMenuItem.Header = "_Show Property Selection";
			showPropertyMenuItem.Click += showPropertyMenuItem_Click;
			ContextMenu.Items.Add(showPropertyMenuItem);

			this.subscriptionTranslator = translator;

			
			VisualizationName = "Data Visualization";
		}

		void Splitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			splitterPosition = PreviewColumn.Width.Value / (PreviewColumn.Width.Value + DetailsColumn.Width.Value);
			if (SplitPositionChanged != null)
				SplitPositionChanged(this, splitterPosition);
		}

		

		void showPropertyMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!EnablePropertySelectionDialog)
				return;

			//	Build list of available properties
			var allProperties = new List<DataPropertyModel>();
			foreach (var possibleSubscription in subscriptionTranslator.AllTranslations(VisualizableTypes))
			{
				allProperties.Add(new DataPropertyModel()
				{
					DataTypeName = possibleSubscription.Value.DataTypeName,
					PropertyName = possibleSubscription.Value.PropertyName,
					SensorHostName = possibleSubscription.Value.SourceName,
					SensorTypeName = possibleSubscription.Value.SourceTypeName,
					Reference = possibleSubscription.Key
				});
			}

			var propertyDialog = new PropertySelectionDialog(allProperties, selectedProperties);

			if (propertyDialog.ShowDialog().GetValueOrDefault(false))
			{
				var newSelectedProperties = propertyDialog.SelectedProperties.ToList();

				//	Remove route listeners for removed properties
				var removedProperties = selectedProperties.Where((model) => !newSelectedProperties.Contains(model));
				foreach (var removedProperty in removedProperties)
				{
					var propertyDescriptor = (DataSubscription)removedProperty.Reference;
					PreviewVisualization.RequestRemoveVisualization(propertyDescriptor);
					DetailsVisualization.RequestRemoveVisualization(propertyDescriptor);
				}

				//	Add route listeners for new properties
				var uniqueNewProperties = newSelectedProperties.Where((model) => !selectedProperties.Contains(model));
				foreach (var newProperty in uniqueNewProperties)
				{
					var propertyDescriptor = (DataSubscription)newProperty.Reference;
					PreviewVisualization.RequestAddVisualization(propertyDescriptor);
					DetailsVisualization.RequestAddVisualization(propertyDescriptor);
				}

				selectedProperties = newSelectedProperties;
			}
		}
	}
}
