using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Models.Data.PropertyValues.Services;
using System.Windows.Threading;
using System.ServiceModel.Channels;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
    public class TimebarBooleanVisualizer : TimebarVisualizer<TimebarBooleanVisualizer.ValueAdapter, IBooleanPropertyValueService>
    {
        public TimebarBooleanVisualizer(Dispatcher dispatcher, Uri propertyServiceUri, Binding binding)
            : base(dispatcher, propertyServiceUri, binding)
        {
            sparkline = new RadWinLossSparkline();
            sparkline.DataContext = this;
            sparkline.SetBinding(RadWinLossSparkline.ItemsSourceProperty, "Collection");
            sparkline.YValuePath = "Value";
            sparkline.XValuePath = "StartTime";
        }

        public TimebarBooleanVisualizer(BooleanVisualizer visualizer)
            : this(visualizer.dispatcher, visualizer.uri, visualizer.binding)
        {
        }

        public class ValueAdapter
        {
            public Guid ItemID
            {
                get;
                set;
            }

            public Guid SourceID
            {
                get;
                set;
            }

            public long? Interval
            {
                get;
                set;
            }

            public DateTime StartTime
            {
                get;
                set;
            }

            public System.Int32 Value
            {
                get;
                set;
            }

            public string Latitude
            {
                get;
                set;
            }

            public string Longitude
            {
                get;
                set;
            }

            public static implicit operator BooleanPropertyValue(ValueAdapter adapter)
            {
                return new BooleanPropertyValue()
                {
                    Interval = adapter.Interval,
                    ItemID = adapter.ItemID,
                    Latitude = adapter.Latitude,
                    Longitude = adapter.Longitude,
                    SourceID = adapter.SourceID,
                    StartTime = adapter.StartTime,
                    Value = adapter.Value > 0
                };
            }

            public static implicit operator ValueAdapter(BooleanPropertyValue value)
            {
                return new ValueAdapter()
                {
                    Interval = value.Interval,
                    ItemID = value.ItemID,
                    Latitude = value.Latitude,
                    Longitude = value.Longitude,
                    SourceID = value.SourceID,
                    StartTime = value.StartTime,
                    Value = value.Value ? 1 : -1
                };
            }
        }
    }
}
