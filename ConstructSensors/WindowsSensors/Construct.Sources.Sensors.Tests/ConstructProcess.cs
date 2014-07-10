using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Construct.Dataflow.Brokering;
using Construct.Base.Constants;
using Construct.Base.Wcf;
using Construct.Dataflow.Brokering.Messaging;

namespace Construct.Sensors.Tests
{
    class ConstructProcess
    {
        public Broker broker = new Broker(new Inbox<Telemetry>(new Rendezvous<Telemetry>(Protocol.HTTP, "localhost", GlobalRuntimeSettings.TELEMETRY_GUID, Guid.Parse("77e46375-5494-42ae-88d4-130882224d20"))));   
    }
}
