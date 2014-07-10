using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Construct.MessageBrokering;

namespace Construct.Server.Entities.Adapters
{
    public class SensorHost
    {
        private Guid _sensorHostTypeID;
        public virtual Guid SensorHostTypeID
        {
            get
            {
                return this._sensorHostTypeID;
            }
            set
            {
                this._sensorHostTypeID = value;
            }
        }

        private bool _isHealthy;
        public virtual bool IsHealthy
        {
            get
            {
                return this._isHealthy;
            }
            set
            {
                this._isHealthy = value;
            }
        }

        private Guid _iD;
        public virtual Guid ID
        {
            get
            {
                return this._iD;
            }
            set
            {
                this._iD = value;
            }
        }

        private string _hostUri;
        public virtual string HostUri
        {
            get
            {
                return this._hostUri;
            }
            set
            {
                this._hostUri = value;
            }
        }

        private string _hostName;
        public virtual string HostName
        {
            get
            {
                return this._hostName;
            }
            set
            {
                this._hostName = value;
            }
        }

        public static implicit operator Construct.Server.Entities.SensorHost(Construct.Server.Entities.Adapters.SensorHost adapter)
        {
            var ret = new Construct.Server.Entities.SensorHost()
            {
                ID = adapter._iD,
                SensorHostTypeID = adapter._sensorHostTypeID,
                HostName = adapter._hostName,
                HostUri = adapter._hostUri,
                IsHealthy = adapter._isHealthy
            };

            return ret;
        }

        public static implicit operator Construct.Server.Entities.Adapters.SensorHost(Construct.Server.Entities.SensorHost adapter)
        {
            var ret = new Construct.Server.Entities.Adapters.SensorHost()
            {
                ID = adapter.ID,
                SensorHostTypeID = adapter.SensorHostTypeID,
                HostName = adapter.HostName,
                HostUri = adapter.HostUri,
                IsHealthy = adapter.IsHealthy
            };

            return ret;
        }

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
                        case "tcp":
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