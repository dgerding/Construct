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
using Telerik.Windows.Controls.Legend;

namespace Construct.UX.Views.Visualizations
{
	/// <summary>
	/// Interaction logic for DataLegend.xaml
	/// </summary>
	public partial class DataLegend : UserControl
	{
		struct LegendEntry
		{
			public Color Color;
			public String Name;

			public DataSubscription SourceSubscription;

			public LegendItem WpfLegendItem;
		}

		private List<LegendEntry> LegendEntries = new List<LegendEntry>();
		public DataLegend()
		{
			InitializeComponent();

			Legend.Items = new LegendItemCollection();
		}

		public bool HasMaxEntries
		{
			get { return LegendEntries.Count == MaxEntries; }
		}

		public int MaxEntries
		{
			get { return LegendColors.Colors.Length; }
		}
		public Color ? AddLegendItem(DataSubscription subscription, SubscriptionTranslator translator)
		{
			if (HasMaxEntries)
				return null;

			var translation = translator.GetTranslation(subscription);
			LegendEntry newEntry = new LegendEntry()
			{
				Color = LegendColors.GetUnusedColor(LegendEntries.Select((entry) => entry.Color)),
				SourceSubscription = subscription,
				Name = String.Format("{0} {1} - {2}", translation.SourceName, translation.SourceTypeName, translation.PropertyName)
			};

			LegendItem newLegendItem = new LegendItem();
			newLegendItem.MarkerFill = new SolidColorBrush(newEntry.Color);
			newLegendItem.Title = newEntry.Name;

			newEntry.WpfLegendItem = newLegendItem;

			LegendEntries.Add(newEntry);
			Legend.Items.Add(newLegendItem);

			return newEntry.Color;
		}

		public void RemoveLegendItem(DataSubscription subscription)
		{
			LegendEntry entryForSubscription = LegendEntries.Single((entry) => entry.SourceSubscription == subscription);
			Legend.Items.Remove(entryForSubscription.WpfLegendItem);
			LegendEntries.Remove(entryForSubscription);
		}
	}
}
