using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Construct.Base.Wcf;
using System.Diagnostics;
using System.ServiceModel.Channels;
using Construct.Sensors.DragonTranscriptionSensor.AggregatorService;
using System.IO;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    public abstract class AggregatorSensor : Sensor, IAggregator
    {
        public string
            SourceTypeName { get; set; }

        private SelfServiceHostConfigurator
            selfServiceHost,
            selfService;
        private AggregatorService.AggregatorClient
            theAggregator;

        protected Guid 
            sourceID,
            typeSourceID;
        //protected string
        //    name = typeof(Sensor).ToString(), 
        //    startMessage = "started", 
        //    stopMessage = "stopped", 
        //    exitMessage = "is now exiting",
        //    version = "1";

        protected AggregatorSensor(string[] args, BindingTypes type, string version)
            : base(args, type, "v1.1.0")
        {
            theAggregator = null;
            sourceID = Guid.Parse(args[0]);
            typeSourceID = Guid.Parse(args[2]);
            this.version = version;

            try
            {
                //ConfigureSelfHost();
                //InitializeAggregator(args[1], type);
                this.SetupWcfSelfHostService(sourceID);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SetupWcfSelfHostService(Guid sourceID)
        {
            this.selfService = new SelfServiceHostConfigurator(this, typeof(IAggregator), sourceID);

            this.selfService.AddDefaultTcpBinding();
            this.selfService.AddDefaultNetNamedPipeBinding();
            this.selfService.AddDefaultHttpBinding();
        }

        public bool StartAggregator()
        {
            try
            {
                this.selfService.Open();
            }
            catch
            {
                return false;
            }

            return true;
        }







        public virtual void AddItem(Guid sourceID, DateTime createdTime, SqlGeography createdLocation, Object blob)
        {
            
        }

        public void AddTelemetry(Guid sourceID, string theTelemetry)
        {
        }

        public void AddStream(Guid sourceID)
        {
            int x = 3;
        }

        public Guid GetID()
        {
            return sourceID;
        }

        //public void SendItem(Object itemPayload)
        //{
        //    theAggregator.AddItem
        //    (
        //        sourceID, DateTime.UtcNow, new AggregatorService.SqlGeography(), itemPayload
        //    );
        //}

        //public virtual string Start()
        //{
        //    return name + ": " + startMessage;
        //}
        //public virtual string Stop()
        //{
        //    return name + ": " + stopMessage;
        //}
        //public virtual string Exit()
        //{
        //    return name + ": " + exitMessage;
        //}

        //private void ConfigureSelfHost()
        //{
        //    selfServiceHost = new SelfServiceHostConfigurator(this, typeof(ISensor), sourceID);
        //    selfServiceHost.AddDefaultTcpBinding();
        //    selfServiceHost.AddDefaultNetNamedPipeBinding();
        //    selfServiceHost.AddDefaultHttpBinding();
        //    selfServiceHost.Open();
        //}
        //private void InitializeAggregator(string constructServerUri, BindingTypes type)
        //{
        //    EndpointAddress
        //        address = new EndpointAddress(constructServerUri);
        //    Binding
        //        binding = null;

        //    switch (type)
        //    {
        //        case BindingTypes.HTTP:
        //            binding = new BasicHttpBinding();
        //            break;
        //        case BindingTypes.NetNamedPipe:
        //            binding = new NetNamedPipeBinding();
        //            break;
        //        case BindingTypes.TCP:
        //            binding = new NetTcpBinding();
        //            break;
        //        default:
        //            throw new ProtocolException();
        //    }

        //    theAggregator = new AggregatorClient(binding, address);
        //}
    }
}

