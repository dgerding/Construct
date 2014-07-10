using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.Windows.Threading;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Models.Data.PropertyValues.Services;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
    public class TimebarIntVisualizer : TimebarVisualizer<IntPropertyValue, IIntPropertyValueService>
    {
        public TimebarIntVisualizer(Dispatcher dispatcher, Uri propertyServiceUri, Binding binding) : base(dispatcher, propertyServiceUri, binding)
        {
            sparkline = new RadLinearSparkline();
            sparkline.DataContext = this;
            sparkline.ItemsSource = this.Collection;
            sparkline.SetBinding(RadLinearSparkline.ItemsSourceProperty, "Collection");
            sparkline.YValuePath = "Value";
            sparkline.XValuePath = "StartTime";
        }
        public TimebarIntVisualizer(IntVisualizer visualizer)
            : this(visualizer.dispatcher, visualizer.uri, visualizer.binding)
        {
        }
    }
}
