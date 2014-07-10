using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConstructTcpServer
{
    public class ProducerConsumerQueue<T> : Queue<T>
    {
        readonly object SyncRoot = new object();

        public ProducerConsumerQueue() : base()
        {
            InitSyncEvents();
        }

        public ProducerConsumerQueue(int capacity) : base(capacity)
        {
            InitSyncEvents();
        }

        public EventWaitHandle ExitThreadEvent { get; private set; }
        public EventWaitHandle NewItemEvent { get; private set; }
        public WaitHandle[] EventArray { get; private set; }

        /// <summary>
        /// Thread-safe method to add an object to the end of the queue.
        /// </summary>
        /// <param name="item">The object to add to the queue. The value can be null for reference types.</param>
        public void SyncEnqueue(T item)
        {
            lock (SyncRoot)
            {
                Enqueue(item);
                NewItemEvent.Set();
            }
        }

        /// <summary>
        /// Thread-safe method to remove and return the object at the beginning of the queue
        /// </summary>
        /// <returns>The object that is removed from the beginning of the Queue<(Of <(T>)>).</returns>
        /// <exception cref="System.InvalidOperationException">The Queue<(Of <(T>)>) is empty.</exception>
        public T SyncDequeue()
        {
            lock (SyncRoot)
            {
                if (Count > 0)
                {
                    return Dequeue();
                }
                else
                {
                    return default(T);
                }
            }
        }

        private void InitSyncEvents()
        {
            NewItemEvent = new AutoResetEvent(false);
            ExitThreadEvent = new ManualResetEvent(false);
            EventArray = new WaitHandle[2];
            EventArray[0] = ExitThreadEvent; // exit event has priority over new item event
            EventArray[1] = NewItemEvent;
        }
    }
    //}
    //        producerThread.Join();
    //        consumerThread.Join();
    //    }
    //        Console.WriteLine("Signaling threads to terminate...");
    //        syncEvents.ExitThreadEvent.Set();
    //        for (int i = 0; i < 4; i++)
    //        {
    //            Thread.Sleep(2500);
    //            ShowQueueContents(queue);
    //        }
    //        Console.WriteLine("Launching producer and consumer threads...");
    //        producerThread.Start();
    //        consumerThread.Start();
    //        Console.WriteLine("Configuring worker threads...");
    //        Producer producer = new Producer(queue, syncEvents);
    //        Consumer consumer = new Consumer(queue, syncEvents);
    //        Thread producerThread = new Thread(producer.ThreadRun);
    //        Thread consumerThread = new Thread(consumer.ThreadRun);
    //    static void Main()
    //    {
    //        Queue<int> queue = new Queue<int>();
    //        SyncEvents syncEvents = new SyncEvents();
    //public class ThreadSyncSample
    //{
    //    private static void ShowQueueContents(Queue<int> q)
    //    {
    //        lock (((ICollection)q).SyncRoot)
    //        {
    //            foreach (int item in q)
    //            {
    //                Console.Write("{0} ", item);
    //            }
    //        }
    //        Console.WriteLine();
    //    }
    //public class Consumer
    //{
    //    public Consumer(Queue<int> q, SyncEvents e)
    //    {
    //        _queue = q;
    //        _syncEvents = e;
    //    }
    //    // Consumer.ThreadRun
    //    public void ThreadRun()
    //    {
    //        int count = 0;
    //        while (WaitHandle.WaitAny(_syncEvents.EventArray) != 1)
    //        {
    //            lock (((ICollection)_queue).SyncRoot)
    //            {
    //                int item = _queue.Dequeue();
    //            }
    //            count++;
    //        }
    //        Console.WriteLine("Consumer Thread: consumed {0} items", count);
    //    }
    //    private Queue<int> _queue;
    //    private SyncEvents _syncEvents;
    //}
    //public class Producer
    //{
    //    public Producer(Queue<int> q, SyncEvents e)
    //    {
    //        _queue = q;
    //        _syncEvents = e;
    //    }
    //    // Producer.ThreadRun
    //    public void ThreadRun()
    //    {
    //        int count = 0;
    //        Random r = new Random();
    //        while (!_syncEvents.ExitThreadEvent.WaitOne(0, false))
    //        {
    //            lock (((ICollection)_queue).SyncRoot)
    //            {
    //                while (_queue.Count < 20)
    //                {
    //                    _queue.Enqueue(r.Next(0, 100));
    //                    _syncEvents.NewItemEvent.Set();
    //                    count++;
    //                }
    //            }
    //        }
    //        Console.WriteLine("Producer thread: produced {0} items", count);
    //    }
    //    private Queue<int> _queue;
    //    private SyncEvents _syncEvents;
    //}
}