using System;
using System.Linq;
using System.ServiceModel.Channels;
using Construct.Server.Models.Data.PropertyValues.Services;
using Construct.Server.Models.Data.PropertyValue;
using System.Windows.Threading;

namespace Construct.UX.Views.Visualizations
{
    public class BooleanVisualizer : Visualizer<BooleanPropertyValue, IBooleanPropertyValueService>
    {
        public BooleanVisualizer(Dispatcher dispatcher, Uri propertyServiceUri, Binding binding)
            : base(dispatcher, propertyServiceUri, binding)
        {
        }
    }
}
