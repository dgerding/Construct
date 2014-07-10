using System;
using Construct.Dataflow.Brokering;
using Construct.Base.Wcf;

namespace Construct.Sensors.Tests
{
    public class ConcreteSensor : Sensor
    {
        private static string MachineName = System.Environment.MachineName;
        private const string Port = "8086";
        private static string bindingType = BindingTypes.HTTP.ToString();
        private static string NEWGUID = Guid.NewGuid().ToString();
        private static string ServerGuid = "1F0AB154-5E32-410A-9305-6A03FFAB6996C";

        private static string Uri = bindingType + "://" + MachineName + ":" + Port + "/" + "Construct" + "/" + NEWGUID;
        private static string constructUri = bindingType + "://" + "192.168.82.250" + ":" + Port + "/" + "Construct" + "/" + ServerGuid;

        private static Uri uri = new Uri(Uri);

        public ConcreteSensor()
            : base(Protocol.HTTP, new string[] { NEWGUID, constructUri })
        { }

    }
}
