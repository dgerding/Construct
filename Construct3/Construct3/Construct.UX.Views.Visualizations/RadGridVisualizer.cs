using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ComponentModel;
using System.ServiceModel.Channels;
using Telerik.Windows.Controls.Sparklines;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace Construct.UX.Views.Visualizations
{
    public class RadGridVisualizer<T, ServiceContract>: INotifyPropertyChanged
    {
        private ServiceHost host = null;
        private DispatcherTimer timer = null;
        private ObservableCollection<T> collection = new ObservableCollection<T>();
        private Random random = new Random();
        private dynamic service = default(ServiceContract);
        private Dispatcher dispatcher = null;
        
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


        public RadGridVisualizer(Dispatcher dispatcher, Uri propertyServiceUri, Binding binding)
        {
            this.dispatcher = dispatcher;

            ChannelFactory<ServiceContract> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(propertyServiceUri);
            channelFactory = new ChannelFactory<ServiceContract>(binding, myEndpoint);

            service = channelFactory.CreateChannel();

            timer = new DispatcherTimer(DispatcherPriority.Background, dispatcher);
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (collection.Count() == 0)
            {

                IList<dynamic> temp = new List<dynamic>();
                if (latestRecordedTime.HasValue)
                {
                    foreach (var item in service.GetAfter(latestRecordedTime))
                    {
                        temp.Add((T)item);
                    }
                }
                else
                {
                    foreach (var item in service.GetAll())
                    {
                        temp.Add((T)item);
                    }
                }

                if (temp.Count > 0)
                {
                    DateTime earliest = temp.First().StartTime;
                    DateTime last = earliest;

                    if (EarliestRecordedTime.HasValue == false)
                    {
                        EarliestRecordedTime = earliest;
                    }

                    foreach (var item in temp)
                    {
                        if (item.StartTime < EarliestRecordedTime.Value)
                        {
                            earliest = item.StartTime;
                        }

                        if (item.StartTime > last)
                        {
                            last = item.StartTime;
                        }
                    }

                    if (EarliestRecordedTime.HasValue)
                    {
                        if (earliest < EarliestRecordedTime.Value)
                        {
                            EarliestRecordedTime = earliest;
                        }
                    }
                    else
                    {
                        EarliestRecordedTime = earliest;
                    }

                    if (LatestRecordedTime.HasValue)
                    {
                        if (last > latestRecordedTime)
                        {
                            LatestRecordedTime = last;
                        }
                    }
                    else
                    {
                        LatestRecordedTime = last;
                    }
                }

                UpdateGuiCollection(temp);
            }
            else
            {
                dispatcher.Invoke
                (
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action(delegate()
                    {
                        IList<dynamic> temp = new List<dynamic>();
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
                        UpdateGuiCollection(temp);
                    })
                );
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

            dispatcher.Invoke
            (
                System.Windows.Threading.DispatcherPriority.DataBind,
                new Action
                (
                    delegate()
                    {
                        if (propertiesToAdd.Count() > 0)
                        {
                            foreach (T property in propertiesToAdd)
                            {
                                if (collection.Contains(property) == false)
                                {
                                    collection.Add(property);
                                }
                            }
                        }
                    }
                )
            );
        }

    }
}
