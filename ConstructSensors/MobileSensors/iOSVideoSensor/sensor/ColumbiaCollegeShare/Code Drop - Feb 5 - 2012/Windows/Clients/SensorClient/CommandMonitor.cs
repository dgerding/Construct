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
    public class CommandMonitor
    {
        private CommandMonitor() { }

        public CommandMonitor(SensorClient client, uint pollingIntervalMilliSeconds)
        {
            this._client = client;
            this._pollingIntervalMilliSeconds = pollingIntervalMilliSeconds;
        }

        private SensorClient _client = null;
        private uint _pollingIntervalMilliSeconds = 5000;
        private bool _isRunning = false;

        private Thread _monitorThread = null;
        private ManualResetEvent _monitorThreadShutdownEvent = null;

        public EventHandler<SensorCommandReceivedEventArgs> SensorCommandReceived;
        private void onSensorCommandReceived(SensorCommand command)
        {
            if (SensorCommandReceived != null)
            {
                SensorCommandReceivedEventArgs args = new SensorCommandReceivedEventArgs();
                args.Command = command;
                SensorCommandReceived(this, args);
            }
        }

        public void Start()
        {
            lock (this)
            {
                if (_isRunning == true)
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
                if (_isRunning == false)
                {
                    return;
                }
                _isRunning = false;

                stopUploaderThread();
            }
        }

        private void stopUploaderThread()
        {
            if (_monitorThread == null || _monitorThreadShutdownEvent == null)
            {
                return;
            }

            _monitorThreadShutdownEvent.Set();

            // wait for it to be manually reset on thread func completion
            while (_monitorThreadShutdownEvent.WaitOne(0) == true)
            {
                Thread.Sleep(10);
            }

            _monitorThread = null;
            _monitorThreadShutdownEvent = null;
        }


        private void startUploaderThread()
        {
            _monitorThreadShutdownEvent = new ManualResetEvent(false);
            _monitorThread = new Thread(new ThreadStart(monitor));
            _monitorThread.IsBackground = true;
            _monitorThread.Name = "Sensor Command Monitor";
            _monitorThread.Start();
        }

        private void monitor()
        {
            DateTime lastCommandCheckTime = DateTime.MinValue;

            string message;
            _client.ConnectSensor(out message);

            while (true)
            {
                if (_monitorThreadShutdownEvent.WaitOne(0) == true)
                {
                    break;
                }

                if (DateTime.Now.Subtract(lastCommandCheckTime) > TimeSpan.FromMilliseconds(this._pollingIntervalMilliSeconds))
                {
                    try
                    {
                        checkForCommands();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            _monitorThreadShutdownEvent.Reset();
        }

        private void checkForCommands()
        {
            string message;
            SensorCommand command = _client.GetSensorCommand(out message);
            if ( (command == SensorCommand.None) && (message=="NOT_CONNECTED") )
            {
                // try to force connect
                _client.ConnectSensor(out message);

                // try command fetch again
                command = _client.GetSensorCommand(out message);
                if ((command == SensorCommand.None) && (message == "NOT_CONNECTED"))
                {
                    return;
                }
            }

            // the message param is the command
            if ( command != SensorCommand.None )
            {
                onSensorCommandReceived( command );
            }
        }


    }

    public class SensorCommandReceivedEventArgs : EventArgs
    {
        public SensorCommand Command;
    }



}
