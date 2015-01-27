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

		public IVisualizer PreviewVisualization
		{
			get { return PreviewContainer.Children.Count > 0 ? PreviewContainer.Children[0] as IVisualizer : null; }
		}

		public IVisualizer DetailsVisualization
		{
			get { return DetailsContainer.Children.Count > 0 ? DetailsContainer.Children[0] as IVisualizer : null; }
		}

		public Guid ID { get; set; }

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

		public void SetSubscriptions(IEnumerable<DataSubscription> newSubscriptions)
		{
			foreach (var model in SelectedProperties)
			{
				if (PreviewVisualization != null)
					PreviewVisualization.RequestRemoveVisualization((DataSubscription)model.Reference);
				if (DetailsVisualization != null)
					DetailsVisualization.RequestRemoveVisualization((DataSubscription)model.Reference);
			}

			SelectedProperties.Clear();

			foreach (var subscription in newSubscriptions)
			{
				if (PreviewVisualization != null)
					PreviewVisualization.RequestAddVisualization(subscription);
				if (DetailsVisualization != null)
					DetailsVisualization.RequestAddVisualization(subscription);

				if (PreviewVisualization is PropertyVisualization)
					SelectedProperties.Add(GenerateAvailableProperties().Single(p => p.Reference.Equals(subscription)));
				if (PreviewVisualization is AggregateVisualization)
					SelectedProperties.Add(GenerateAvailableSources().Single(s => s.Reference.Equals(subscription)));
			}
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

			//	Ugh, this type-based procedural code needs to be redone
			if (PreviewVisualization is PropertyVisualization)
				RunPropertySelection(GenerateAvailableProperties());
			if (PreviewVisualization is AggregateVisualization)
				RunSourceSelection(GenerateAvailableSources());
		}

		IEnumerable<DataPropertyModel> GenerateAvailableProperties()
		{
			var allProperties = new List<DataPropertyModel>();
			var visualizableTypes = (PreviewVisualization as PropertyVisualization).VisualizableTypes;
			foreach (var possibleSubscription in subscriptionTranslator.AllTranslations(visualizableTypes))
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

			return allProperties;
		}

		IEnumerable<DataPropertyModel> GenerateAvailableSources()
		{
			var allSources = new List<DataPropertyModel>();
			var visualizableTypes = (PreviewVisualization as AggregateVisualization).VisualizableDataTypes;

			foreach (var possibleSubscription in subscriptionTranslator.AllKnownSubscriptions.Where(ds => visualizableTypes.Contains(ds.AggregateDataTypeId)))
			{
				var translation = subscriptionTranslator.GetTranslation(possibleSubscription);

				allSources.Add(new DataPropertyModel()
				{
					DataTypeName = translation.DataTypeName,
					SensorHostName = translation.SourceName,
					SensorTypeName = translation.SourceTypeName,
					Reference = new DataSubscription()
					{
						AggregateDataTypeId = possibleSubscription.AggregateDataTypeId,
						SourceId = possibleSubscription.SourceId
					}
				});
			}

			return allSources.Distinct();
		}

		void RunPropertySelection(IEnumerable<DataPropertyModel> allProperties)
		{
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
					if (PreviewVisualization != null)
						PreviewVisualization.RequestAddVisualization(propertyDescriptor);
					if (DetailsVisualization != null)
						DetailsVisualization.RequestAddVisualization(propertyDescriptor);
				}

				selectedProperties = newSelectedProperties;
			}
		}

		void RunSourceSelection(IEnumerable<DataPropertyModel> allSources)
		{
			var propertyDialog = new SourceSelectionDialog(allSources, selectedProperties);

			if (propertyDialog.ShowDialog().GetValueOrDefault(false))
			{
				var newSelectedSources = propertyDialog.SelectedSources.ToList();

				//	Remove route listeners for removed properties
				var removedSources = selectedProperties.Where((model) => !newSelectedSources.Contains(model));
				foreach (var removedSource in removedSources)
				{
					var propertyDescriptor = (DataSubscription)removedSource.Reference;
					if (PreviewVisualization != null)
						PreviewVisualization.RequestRemoveVisualization(propertyDescriptor);
					if (DetailsVisualization != null)
						DetailsVisualization.RequestRemoveVisualization(propertyDescriptor);
				}

				//	Add route listeners for new properties
				var uniqueNewProperties = newSelectedSources.Where((model) => !selectedProperties.Contains(model));
				foreach (var newProperty in uniqueNewProperties)
				{
					var propertyDescriptor = (DataSubscription)newProperty.Reference;
					if (PreviewVisualization != null)
						PreviewVisualization.RequestAddVisualization(propertyDescriptor);
					if (DetailsVisualization != null)
						DetailsVisualization.RequestAddVisualization(propertyDescriptor);
				}

				selectedProperties = newSelectedSources;
			}
		}
	}
}
