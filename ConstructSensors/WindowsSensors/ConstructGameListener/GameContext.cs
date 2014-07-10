using System;
using System.Threading;

namespace ConstructGameListener
{
    /// <summary>
    /// Holds all information needed to associate a thread
    /// with a particular game session.
    /// </summary>
    public class GameContext
    {
        //TODO ? public ProducerConsumerQueue<string[]> MessageQueue;
        public ConstructGameProcessor GameProcessor;
        public Thread QueueThread;
        public int GameId;
        public string CurrentStatus = string.Empty;

        public GameContext(int iGameId, ConstructGameProcessor gameProcessor, Thread queueThread)
        {
            this.GameId = iGameId;
            //todo this.MessageQueue = messageQueue;
            this.GameProcessor = gameProcessor;
            this.QueueThread = queueThread;
            this.PlayerAlias = string.Empty;
        }

        public GameContext(string strPlayerAlias)
        {
            this.GameId = 0;
            //todo this.MessageQueue = null;
            this.GameProcessor = null;
            this.QueueThread = null;
            this.PlayerAlias = strPlayerAlias;
        }

        public string PlayerAlias { get; set; }
    }
}