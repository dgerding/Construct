using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.MessageBrokering
{
    public static class GlobalRuntimeSettings
    {
        private static Guid telemetry_Guid = Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3");
        public static Guid TELEMETRY_GUID
        {
            get
            {
                return telemetry_Guid;
            }
        }

        private static Guid command_Guid  = Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6");
        public static Guid COMMAND_GUID
        {
            get
            {
                return command_Guid;
            }
        }
    }
}
