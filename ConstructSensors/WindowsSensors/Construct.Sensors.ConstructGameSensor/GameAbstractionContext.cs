using System;
using System.Threading;

namespace ConstructTcpServer
{
    /// <summary>
    /// Holds all information needed to associate a thread
    /// with a particular game session.
    /// </summary>}
    public class GameAbstractionContext
    {
        public int GameId;
        public int GameIntervalDefId;
        public int AbstractionIntervalIndex;
        public ManualResetEvent CurrentResetEvent;
        public ManualResetEvent LastResetEvent;

        public GameAbstractionContext(int iGameId, int iGameIntervalDefId, int iAbstractionIntervalIndex, ManualResetEvent oCurrentResetEvent, ManualResetEvent oLastResetEvent)
        {
            GameId = iGameId;
            GameIntervalDefId = iGameIntervalDefId;
            AbstractionIntervalIndex = iAbstractionIntervalIndex;
            CurrentResetEvent = oCurrentResetEvent;
            LastResetEvent = oLastResetEvent;
        }
    }
    /// <summary>
    /// Holds all information needed to associate a thread
    /// with a particular TcpServer, such as the Tcp Server used to manage audio services
    /// </summary>
}