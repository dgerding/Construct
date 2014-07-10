using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.Timers;

namespace Construct.UX.Views.Visualizations
{
    public class Visualizer<T, ServiceContract> : INotifyPropertyChanged
    {
        private ServiceHost host = null;
        private Timer timer = null;
        private ObservableCollection<T> collection = new ObservableCollection<T>();
        private Random random = new Random();
        private dynamic service = default(ServiceContract);
        internal Dispatcher dispatcher = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<T> Collection
        {
            get { return collection; }
            set
            {
                collection = value;
                NotifyPropertyChanged("Collection");
            }
        }

        private DateTime? earliestRecordedTime = null;
        public DateTime? EarliestRecordedTime
        {
            get { return earliestRecordedTime; }
            set
            {
                earliestRecordedTime = value;
                NotifyPropertyChanged("EarliestRecordedTime");
            }
        }
        private DateTime? latestRecordedTime = null;
        public DateTime? LatestRecordedTime
        {
            get { return latestRecordedTime; }
            set
            {
                latestRecordedTime = value;
                NotifyPropertyChanged("LatestRecordedTime");
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal Binding binding = null;
        internal Uri uri = null;

        public Visualizer(Dispatcher dispatcher, Uri propertyServiceUri, Binding binding)
        {
            this.dispatcher = dispatcher;

            ChannelFactory<ServiceContract> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(propertyServiceUri);
            channelFactory = new ChannelFactory<ServiceContract>(binding, myEndpoint);

            service = channelFactory.CreateChannel();

            timer = new Timer();
            timer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
            timer.Elapsed += TimerTick;
            timer.Start();

            this.binding = binding;
            this.uri = propertyServiceUri;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (collection.Count() == 0)
            {
                IList<dynamic> temp = new List<dynamic>();
                GetLatestItems(temp);

                if (temp.Count > 0)
                {
                    HandleTimes(temp);
                }

                UpdateGuiCollection(temp);
            }
            else
            {
                IList<dynamic> temp = new List<dynamic>();
                GetLatestItems(temp);

                UpdateGuiCollection(temp);
            }
        }

        private void UpdateTimesAsync(DateTime earliest, DateTime last)
        {
            dispatcher.Invoke
            (
                System.Windows.Threading.DispatcherPriority.Background,
                (Action)delegate()
                {
                    if (earliestRecordedTime.HasValue && earliest < earliestRecordedTime.Value || earliestRecordedTime.HasValue == false)
                    {
                        EarliestRecordedTime = earliest;
                    }

                    if (latestRecordedTime.HasValue && last > latestRecordedTime.Value || latestRecordedTime.HasValue == false)
                    {
                        LatestRecordedTime = last;
                    }
                }
            );
        }

        private void HandleTimes(IList<dynamic> temp)
        {
            DateTime? earliest = temp.First().StartTime;
            DateTime? last = earliest;


            foreach (var item in temp)
            {
                if (earliest.HasValue && item.StartTime < earliest.Value || earliest.HasValue == false)
                {
                    earliest = item.StartTime;
                }

                if (last.HasValue && item.StartTime > last.Value || last.HasValue == false)
                {
                    last = item.StartTime;
                }
            }

            UpdateTimesAsync(earliest.Value, last.Value);
        }

        private void GetLatestItems(IList<dynamic> temp)
        {
            if (latestRecordedTime.HasValue)
            {
                foreach (T item in service.GetAfter(latestRecordedTime.Value))
                {
                    temp.Add((T)item);
                }
            }
            else
            {
                foreach (T item in service.GetAll())
                {
                    temp.Add((T)item);
                }
            }
        }


        private void UpdateGuiCollection(IEnumerable<dynamic> temp)
        {
            IList<T> propertiesToAdd = new List<T>();

            foreach (dynamic property in temp)
            {
                if (collection.Contains(property) == false)
                {
                    if (property is T)
                    {
                        propertiesToAdd.Add((T)property);
                    }
                }
            }


            Parallel.ForEach(propertiesToAdd, target =>
                dispatcher.Invoke
                (
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action
                    (
                        delegate()
                        {
                            Collection.Add(target);
                        }
                    )
                )
            );
        }

    }
}
