using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Models.Data.PropertyValues.Services;
using System.Windows.Threading;
using System.ServiceModel.Channels;

namespace Construct.UX.Views.Visualizations
{
    public class IntVisualizer : Visualizer<IntPropertyValue, IIntPropertyValueService>
    {
        public IntVisualizer(Dispatcher dispatcher, Uri propertyServiceUri, Binding binding)
            : base(dispatcher, propertyServiceUri, binding)
        {
        }
    }
}
