using System;
using sm.eod;
using sm.eod.io;

namespace SMVisualization
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		[STAThread]
        static void Main(string[] args)
        {
            using (SMVisualization game = new SMVisualization())
            {
                game.Run();
            }
        }
    }
#endif
}

