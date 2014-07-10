using System;
using System.Linq;

namespace Construct.MessageBrokering
{
    public enum Protocol
    {
        HTTP,
        NetNamedPipes,
        TCP,
        UNKNOWN = Int32.MaxValue
    }
}