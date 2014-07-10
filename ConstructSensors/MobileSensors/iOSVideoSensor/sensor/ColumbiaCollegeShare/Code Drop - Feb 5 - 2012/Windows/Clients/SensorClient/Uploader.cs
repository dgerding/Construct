using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using RestSharp;
using SensorSharedTypes;

namespace SensorClient
{
    public class Uploader
    {
        private Uploader() { }

        public Uploader( SensorClient client, uint maxPartsPerStream ) 
        {
            this._client = client;
            this._maxPartsPerStream = maxPartsPerStream;
        }

        private SensorClient _client = null;
        private ConcurrentQueue<StreamPart> _parts = new ConcurrentQueue<StreamPart>();
        private uint _maxPartsPerStream;
        private uint _sequenceNumber = 0;
        private bool _isRunning = false;
        private uint _maxBufferedParts = 50;
        private string _currentStreamID = Guid.NewGuid().ToString();

        private Thread _uploaderThread = null;
        private ManualResetEvent _uploaderThreadShutdownEvent = null;

        #region events
        public EventHandler<StreamPartUploadedEventArgs> StreamPartUploaded;
        private void onStreamPartUploaded( StreamPart part )
        {
            if ( StreamPartUploaded != null )
            {
                StreamPartUploadedEventArgs args = new StreamPartUploadedEventArgs();
                args.Part = part;
                StreamPartUploaded(this, args);
            }
        }

        public EventHandler<StreamPartUploadFailedEventArgs> StreamPartUploadFailed;
        private void onStreamPartUploadFailed(StreamPart part, string message)
        {
            if (StreamPartUploadFailed != null)
            {
                StreamPartUploadFailedEventArgs args = new StreamPartUploadFailedEventArgs();
                args.Part = part;
                args.Message = message;
                StreamPartUploadFailed(this, args);
            }
        }

        public EventHandler<StreamPartDroppedEventArgs> StreamPartUploadDropped;
        private void onStreamPartUploadDropped(StreamPart part)
        {
            if (StreamPartUploadDropped != null)
            {
                StreamPartDroppedEventArgs args = new StreamPartDroppedEventArgs();
                args.Part = part;
                StreamPartUploadDropped(this, args);
            }
        }
        #endregion

        public void AddPart(StreamPart part)
        {
            bool insert = true;
            lock( this )
            {
                if ( _parts.Count > _maxBufferedParts )
                {
                    insert = false;
                }
            }
            if (insert == true)
            {
                _parts.Enqueue(part);
            }
            else
            {
                onStreamPartUploadDropped(part); ;
            }
        }

        public void Start()
        {
            lock (this)
            {
                if ( _isRunning == true )
                {
                    return;
                }
                _isRunning = true;

                startUploaderThread();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if ( _isRunning == false )
                {
                    return;
                }
                _isRunning = false;

                stopUploaderThread();
            }
        }

        private void stopUploaderThread()
        {
            if ( _uploaderThread == null || _uploaderThreadShutdownEvent == null)
            {
                return;
            }

            _uploaderThreadShutdownEvent.Set();

            // wait for it to be manually reset on thread func completion
            while (_uploaderThreadShutdownEvent.WaitOne(0) == true)
            {
                Thread.Sleep(10);
            }

            _uploaderThread = null;
            _uploaderThreadShutdownEvent = null;
        }


        private void startUploaderThread()
        {
            _uploaderThreadShutdownEvent = new ManualResetEvent(false);
            _uploaderThread = new Thread(new ThreadStart(uploadParts));
            _uploaderThread.IsBackground = true;
            _uploaderThread.Name = "Stream Part Uploader";
            _uploaderThread.Start();
        }

        private void uploadParts()
        {
            string message;
            _client.ConnectSensor(out message);

            while ( true )
            {
                if ( _uploaderThreadShutdownEvent.WaitOne(0) == true )
                {
                    try
                    {
                        // clear the existing queue and send the next item (if any) as IsLast
                        StreamPart lastPart = null;
                        if (_parts.TryDequeue(out lastPart) == true) 
                        {
                            lastPart.SequenceNumber = _sequenceNumber++;
                            lastPart.StreamID = _currentStreamID;
                            lastPart.IsLastPart = true;
                            uploadPart(lastPart);
                        }
                        _sequenceNumber = 0;
                        _parts = new ConcurrentQueue<StreamPart>();
                        _currentStreamID = Guid.NewGuid().ToString();
                    }
                    catch
                    {
                    }
                    break;
                }

                // dequeue stream parts while available and upload them
                StreamPart part;
                if (_parts.TryDequeue(out part) == true)
                {
                    lock (this)
                    {
                        // overwrite the stream ID and sequence number
                        part.SequenceNumber = _sequenceNumber++;
                        part.StreamID = _currentStreamID;

                        // modify the stream ID, sequence number and part count if we have reached the size limit
                        // for this stream
                        if ( _sequenceNumber >= _maxPartsPerStream )
                        {
                            part.IsLastPart = true;
                            
                            // reset the stream fields
                            _currentStreamID = Guid.NewGuid().ToString();
                            _sequenceNumber = 0;
                        }
                    }
                    uploadPart(part);
                }
                else
                {
                    Thread.Sleep(0);
                }
            }

            _uploaderThreadShutdownEvent.Reset();
        }

        private void uploadPart(StreamPart part)
        {
            string message;
            if (_client.UploadStreamPart(part, out message) == false)
            {
                if ( message == "NOT_CONNECTED" )
                {
                    // try to connect
                    if (_client.ConnectSensor(out message) == false)
                    {
                        onStreamPartUploadFailed(part, message);
                    }
                    else
                    {
                        onStreamPartUploaded(part);
                    }
                }
            }
            else
            {
                onStreamPartUploaded(part);
            }
        }
    }

    public class StreamPartUploadedEventArgs : EventArgs
    {
        public StreamPart Part;
    }

    public class StreamPartDroppedEventArgs : EventArgs
    {
        public StreamPart Part;
    }

    public class StreamPartUploadFailedEventArgs : EventArgs
    {
        public StreamPart Part;
        public string Message;
    }



}
