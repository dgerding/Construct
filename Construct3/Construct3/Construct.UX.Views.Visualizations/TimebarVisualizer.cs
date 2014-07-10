using System;
using System.Linq;
using System.Windows.Threading;
using System.ServiceModel.Channels;
using Telerik.Windows.Controls.Sparklines;

namespace Construct.UX.Views.Visualizations
{
    public class TimebarVisualizer<T, ServiceContract>: Visualizer<T, ServiceContract>
    {
        protected RadSparklineBase sparkline = null;
        public RadSparklineBase Sparkline
        {
            get { return sparkline; }
            set
            {
                sparkline = value;
                NotifyPropertyChanged("Sparkline");
            }
        }

        public TimebarVisualizer(Dispatcher dispatcher, Uri propertyServiceUri, Binding binding)
            : base(dispatcher, propertyServiceUri, binding)
        {
        }
    }
}
