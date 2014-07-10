using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Construct.Media.MediaStream;
using Newtonsoft.Json;
using Construct.MessageBrokering;

namespace Construct.Sensors.FaceDetectorSensor
{
    public class FaceDetectorSensor : Sensor
    {
        private readonly Dictionary<string, string> availableCommands;

        private Capture capture;
        private HaarCascade haar;
        private MediaStreamReceiver mediaStreamReceiver;
        private readonly int FRAME_SIZE = 921600;
        private readonly int FRAME_WIDTH = 640;
        private readonly int FRAME_HEIGHT= 480;
        private readonly int FRAME_STRIDE = 1920;

        public FaceDetectorSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("F7C94746-397E-4F26-ACA3-20DB45E82248"), new Dictionary<string, Guid> { { "", Guid.Empty } })
        {
            broker.OnCommandReceived += broker_OnCommandReceived;

            availableCommands = new Dictionary<string, string>();
            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();

            // passing 0 gets zeroth webcam
            capture = new Capture(0);
            haar = new HaarCascade(Path.Combine(Directory.GetCurrentDirectory(), "haarcascade_frontalface_alt2.xml"));

            //debug line, REMOVE FROM CONSTRUCTOR WHEN DONE
            ConnectToRemoteStream("localhost", 8080, 1000000, "TestFaceDetectorSensorStream");
        }

        private void SendAvailableCommandsTelemetry()
        {
            broker.Publish(new Telemetry("AvailableSensorCommands", availableCommands));
        }

        private void GatherAvailableCommands()
        {
            availableCommands.Add("ConnectToRemoteStream", "HostName");
        }

        public void DetectFaceInCameraCapture()
        {
            while (IsStarted)
            {
                using (Image<Bgr, byte> nextFrame = capture.QueryFrame())
                {
                    FindFaces(nextFrame);
                }
            }
        }

        private void DetectFaceInStream()
        {
            //MemoryStream ms = mediaSocket.MemStream;
            //byte[] rawBytes = new byte[FRAME_SIZE];
            //int offset = 0;
            //do
            //{
            //    offset += ms.Read(rawBytes, 0, FRAME_SIZE);

            //    GCHandle pinnedHandle = GCHandle.Alloc(rawBytes, GCHandleType.Pinned);
            //    IntPtr rawBytePtr = pinnedHandle.AddrOfPinnedObject();
            //    Bitmap bmp = new Bitmap(FRAME_WIDTH, FRAME_HEIGHT, FRAME_STRIDE, System.Drawing.Imaging.PixelFormat.Format24bppRgb, rawBytePtr);

            //    using (Image<Bgr, byte> nextFrame = new Image<Bgr, byte>(bmp))
            //    {
            //        FindFaces(nextFrame);
            //    }
            //    pinnedHandle.Free();
            //}
            //while(true);
        }

        private void FindFaces(Image<Bgr, byte> nextFrame)
        {
            if (nextFrame != null)
            {
                // there's only one channel (greyscale), hence the zero index
                //var faces = nextFrame.DetectHaarCascade(haar)[0];
                Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();

            //    var faces = haar.Detect(grayframe);

            //    foreach (var face in faces)
            //    {
            //        SendItem(new FacePayloadData()
            //                {
            //                    X = face.rect.X,
            //                    Y = face.rect.Y,
            //                    Height = face.rect.Height,
            //                    Width = face.rect.Width,
            //                    Neighbors = face.neighbors
            //                },
            //                 "FaceDetectorItem");
            //    }
            }
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                case "ConnectToRemoteStream":
                    ConnectToRemoteStream(command.Args["Hostname"], Int32.Parse(command.Args["Port"]), Int32.Parse(command.Args["ChunkSize"]), command.Args["StreamName"]);
                    break;
            }
        }

        private void ConnectToRemoteStream(string hostname, int port, int chunkSize, string streamName)
        {
            mediaStreamReceiver = new MediaStreamReceiver(hostname, port, chunkSize, streamName);
            mediaStreamReceiver.ReadStreamData();
        }

        protected override string Start()
        {
            string ret = base.Start();
            if (mediaStreamReceiver == null)
            {
                DetectFaceInCameraCapture();
            }
            else
            {
                DetectFaceInStream();
            }
            return ret;
        }

        protected override string Stop()
        {
            return base.Stop();
        }
    }

    internal struct FacePayloadData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Neighbors { get; set; }
    }
}
