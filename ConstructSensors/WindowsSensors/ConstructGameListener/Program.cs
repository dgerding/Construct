using System;
using System.Linq;
using NLog;

namespace ConstructGameListener
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Log.Trace("in Main");
            if (true)
            {
                ListenerSocket sts = new ListenerSocket();
                Console.WriteLine("Starting Construct Game Tcp Listener");
                sts.StartServer();
            }
        }
    }
}