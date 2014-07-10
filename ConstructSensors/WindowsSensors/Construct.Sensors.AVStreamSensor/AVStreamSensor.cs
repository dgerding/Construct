using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.Live;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Profiles;
using Construct.Media.MediaStream;
using Construct.MessageBrokering;

namespace Construct.Sensors.AVStreamSensor
{
    public class AVStreamSensor : Sensor
    {
        private List<EncoderDevice> videoDevList = new List<EncoderDevice>();
        private List<EncoderDevice> audioDevList = new List<EncoderDevice>();
        private LiveJob job;
        private int port = 8860;
        private int chunkSize = 100000;

        private System.Timers.Timer chunkTimer;

        private event MediaCaptureCompleteEventHandler mediaCaptureComplete;
        private delegate void MediaCaptureCompleteEventHandler(object sender, MediaCaptureEventArgs e);

        private MediaStreamSender mediaStreamSender;

        public AVStreamSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("D044CF68-2F0E-466D-B89B-C60F1D4029BA"), new Dictionary<string, Guid>{ {"AVStream", Guid.Parse("EE8A2FD5-AD74-4098-BD23-59D8786B34B9") } } )
        {
            ConfigureLiveJobs();
            chunkTimer = new System.Timers.Timer(5000);
            chunkTimer.AutoReset = false;
            chunkTimer.Elapsed += new System.Timers.ElapsedEventHandler(HandleVideoChunk);

            mediaStreamSender = new MediaStreamSender(port);
            mediaCaptureComplete += HandleMediaCapture;

            Dictionary<string, string> streamArgs = new Dictionary<string, string>();
            streamArgs.Add("HostName", System.Net.Dns.GetHostName());
            streamArgs.Add("Port", port.ToString());
            streamArgs.Add("ChunkSize", chunkSize.ToString());
            streamArgs.Add("StreamName", String.Format("{0}_AVStream", System.Net.Dns.GetHostName()));

            Telemetry announceStream = new Telemetry("AnnounceStream", streamArgs);
            broker.Publish(announceStream);
        }

        private void HandleVideoChunk(object sender, System.Timers.ElapsedEventArgs args)
        {
            lock (job)
            {
                job.StopEncoding();
                mediaCaptureComplete(this, new MediaCaptureEventArgs() { File = ((FileArchivePublishFormat)job.PublishFormats.Where(p => p is FileArchivePublishFormat).Single()).OutputFileName });
                string newOutputFile = Path.Combine(Directory.GetCurrentDirectory(), String.Format("testVideo_{0}.wmv", DateTime.Now.ToFileTime()));
                ((FileArchivePublishFormat)job.PublishFormats.Where(p => p is FileArchivePublishFormat).Single()).OutputFileName = newOutputFile;
                job.StartEncoding();
                chunkTimer.Start();
            }
        }

        private void HandleMediaCapture(object sender, MediaCaptureEventArgs e)
        {
            byte[] bytes = File.ReadAllBytes(e.File);
            try
            {
                mediaStreamSender.WriteFileBytes(bytes, bytes.Length);
                File.Delete(e.File);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        private void ConfigureLiveJobs()
        {
            job = new LiveJob();

            GatherStreamDevices();

            LiveDeviceSource archiveDeviceSource = job.AddDeviceSource(videoDevList.Count > 0 ? videoDevList[0] : null, audioDevList.Count > 0 ? audioDevList[0] : null);
            job.ActivateSource(archiveDeviceSource);

            job.OutputFormat = new WindowsMediaOutputFormat()
            {
                VideoProfile = new SimpleVC1VideoProfile(),
                AudioProfile = new WmaAudioProfile()
            };

            FileArchivePublishFormat archive = new FileArchivePublishFormat() { OutputFileName = Path.Combine(Directory.GetCurrentDirectory(), String.Format("testVideo_{0}.wmv", DateTime.Now.ToFileTime()))};
            job.PublishFormats.Add(archive);
        }
  
        private void GatherStreamDevices()
        {
            foreach (EncoderDevice vidDev in EncoderDevices.FindDevices(EncoderDeviceType.Video))
            {
                videoDevList.Add(vidDev);
            }
            foreach (EncoderDevice AudioDev in EncoderDevices.FindDevices(EncoderDeviceType.Audio))
            {
                audioDevList.Add(AudioDev);
            }
        }

        protected override string Start()
        {
            string ret = base.Start();
            lock (job)
            {
                job.StartEncoding();
            }
            chunkTimer.Start();
            return ret;
        }

        protected string Stop()
        {
            string ret = base.Stop();
            lock (job)
            {
                job.StopEncoding();
                mediaCaptureComplete(this, new MediaCaptureEventArgs() { File = ((FileArchivePublishFormat)job.PublishFormats.Where(p => p is FileArchivePublishFormat).Single()).OutputFileName });
            }
            chunkTimer.Stop();
            return ret;
        }
    }
}
