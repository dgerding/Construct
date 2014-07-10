using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Construct.Base.Wcf;
using System.Diagnostics;
using System.ServiceModel.Channels;
using Construct.Sensors.DragonTranscriptionSensor.AggregatorService;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    public abstract class Sensor : ISensor
    {
        private SelfServiceHostConfigurator
            selfServiceHost;
        private AggregatorService.AggregatorClient
            theAggregator;

        protected Guid 
            sourceID,
            typeSourceID;
        protected string
            name = typeof(Sensor).ToString(), 
            startMessage = "started", 
            stopMessage = "stopped", 
            exitMessage = "is now exiting",
            version = "1000";

        public enum BindingTypes
        {
            HTTP, NetNamedPipe, TCP
        }

        protected Sensor(string[] args, BindingTypes type, string version)
        {
            theAggregator = null;
            sourceID = Guid.Parse(args[0]);
            typeSourceID = Guid.Parse(args[2]);
            this.version = version;

            try
            {
                ConfigureSelfHost();
                InitializeAggregator(args[1], type);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendItem(Object itemPayload)
        {
            //theAggregator.AddItem
            //(
            //    sourceID, DateTime.UtcNow, new AggregatorService.SqlGeography(), itemPayload
            //);
        }

        public virtual string Start()
        {
            return name + ": " + startMessage;
        }
        public virtual string Stop()
        {
            return name + ": " + stopMessage;
        }
        public virtual string Exit()
        {
            return name + ": " + exitMessage;
        }

        private void ConfigureSelfHost()
        {
            selfServiceHost = new SelfServiceHostConfigurator(this, typeof(ISensor), sourceID);
            selfServiceHost.AddDefaultTcpBinding();
            selfServiceHost.AddDefaultNetNamedPipeBinding();
            selfServiceHost.AddDefaultHttpBinding();
            selfServiceHost.Open();
        }
        private void InitializeAggregator(string constructServerUri, BindingTypes type)
        {
            EndpointAddress
                address = new EndpointAddress(constructServerUri);
            Binding
                binding = null;

            switch (type)
            {
                case BindingTypes.HTTP:
                    binding = new BasicHttpBinding();
                    break;
                case BindingTypes.NetNamedPipe:
                    binding = new NetNamedPipeBinding();
                    break;
                case BindingTypes.TCP:
                    binding = new NetTcpBinding();
                    break;
                default:
                    throw new ProtocolException();
            }

            theAggregator = new AggregatorClient(binding, address);
        }
    }
}

