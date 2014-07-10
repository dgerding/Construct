using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Construct.Base.Wcf;
using System.IO;
using Microsoft.SqlServer.Types;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    public abstract class Aggregator : IAggregator
    {
        public SelfServiceHostConfigurator
            selfService;

        private SensorHostClient theHostClient

        public string 
            SourceTypeName { get; set; }

        public Guid 
            ID { get; private set; }

        public Type 
            AggregatorType = typeof(object);

        private int 
            iteration = 0;

        // NEED a method that can replace the word/reference to object above with a reference
        // to, for example, "RandomNumber" as a runtime type available in the generated Construct assembly

        public Aggregator(string[] args)
        {
            this.ID = Guid.Parse(AssemblyLoadEventArgs[0]);
            this.SourceTypeName = "DragonTranscriptionSensor";
            
            //Resolve blob to type conversion and Entities Collection members

            this.SetupWcfSelfHostService(sourceID);
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

        public void AddItem(Guid sourceID, DateTime createdTime, SqlGeography createdLocation, Object blob)
        {
            CastBlobToUtteranceSensorType(blob);
        }

        private void CastBlobToUtteranceSensorType(Object blob)
        {
            byte[] blobarray;
            blobarray = (byte[])blob;
            string fileName = "Utterance";

            try
            {
                FileStream filestream = new FileStream(fileName + iteration, FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter binarywriter = new BinaryWriter(filestream);
                binarywriter.Write(blobarray);
                binarywriter.Close();
            }
            catch (Exception ex)
            {
                //TODO Add exception  handling.
            }
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
            return this.ID;
        }
    }
}
