using System;
using System.Linq;

namespace Construct.Server.Models
{
    public interface IServer
    {
        bool IsRunning { get; }
        MessageBrokering.Broker Broker { get; }

        void Start();
        void Stop();

        void Start(Models.IModel model, Type contractType, bool isWSDualHTTPRequired = false);
        void Stop(Models.IModel model);
    }
}
