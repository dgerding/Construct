using System;
using System.Linq;
using NLog;

namespace ConstructTcpServer
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Log.Trace("in Main");
            if (true)
            {
                GameMessageListener sts = new GameMessageListener();
                Console.WriteLine("Starting Tcp Server");
                sts.StartServer();
            }
            //else
            //{
            //    ServiceBase[] ServicesToRun;
            //    ServicesToRun = new ServiceBase[] 
            //    {
            //        new ConstructTcpService() 
            //    };
            //    ServiceBase.Run(ServicesToRun);
            //}
        }
    }
    //[System.ComponentModel.RunInstaller(true)]
    //public class ConstructTcpServerInstaller : DefaultManagementProjectInstaller { }
}