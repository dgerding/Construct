using System;
using System.Threading;

namespace ConstructTcpServer
{
    /// <summary>
    /// Holds all information needed to associate a thread
    /// with a particular game session.
    /// </summary>
    public class GameContext
    {
        public ProducerConsumerQueue<string[]> MessageQueue;
        public ConstructGameProcessor GameProcessor;
        public Thread QueueThread;
        public int GameId;
        public string CurrentStatus = string.Empty;

        public GameContext(int iGameId, ProducerConsumerQueue<string[]> messageQueue, ConstructGameProcessor gameProcessor, Thread queueThread)
        {
            GameId = iGameId;
            MessageQueue = messageQueue;
            GameProcessor = gameProcessor;
            QueueThread = queueThread;
            PlayerAlias = string.Empty;
        }

        public GameContext(string strPlayerAlias)
        {
            GameId = 0;
            MessageQueue = null;
            GameProcessor = null;
            QueueThread = null;
            PlayerAlias = strPlayerAlias;
        }

        public string PlayerAlias { get; set; }
    }
}