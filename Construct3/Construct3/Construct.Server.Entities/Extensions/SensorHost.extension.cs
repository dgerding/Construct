using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Construct.MessageBrokering;

namespace Construct.Server.Entities
{
    public partial class SensorHost
    {
        public Protocol GetProtocol()
        {
            if (HostUri != null)
            {
                string[] split = HostUri.Split(':');
                if (split.Length > 0)
                {
                    switch (split[0].ToLower())
                    {
                        case "http":
                            return Protocol.HTTP;
                        case "net.pipe":
                            return Protocol.NetNamedPipes;
                        case "net.tcp":
                            return Protocol.TCP;
                        default:
                            return Protocol.UNKNOWN;
                    }
                }
                else
                {
                    return Protocol.UNKNOWN;
                }
            }
            else
            {
                return Protocol.UNKNOWN;
            }
        }

        public Rendezvous<Command> CreateCommandRendezvous()
        {
            string[] hostUriParts = HostUri.Split('/');
            hostUriParts[3] = "AE9E3C31-09C8-4835-8E2D-286922ADB3F6";
            for (int i = 0; i < hostUriParts.Length - 1; ++i)
            {
                StringBuilder sb = new StringBuilder(hostUriParts[i]);
                sb.Append('/');
                hostUriParts[i] = sb.ToString();
            }

            Rendezvous<Command> ret = new Rendezvous<Command>(String.Concat(hostUriParts));
            return ret;
        }
    }
}
