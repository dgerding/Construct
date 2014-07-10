using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using System.Collections;
using sm.eod;
using sm.eod.io;
using sm.eod.utils;
using Newtonsoft.Json;
using Construct.Sensors;
using Construct.MessageBrokering;


namespace Construct.Sensors.FaceLabSensor
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]    
    class FaceLabSensor : Sensor
    {
        public string dirpath = String.Empty;
        float xTransform = 0;
        float zTransform = 0;

        private void Transform(ref float x, ref float z)
        {
            x = (x * (float)Math.Cos(xTransform)) - (x * (float)Math.Sin(xTransform));
            z = (z * (float)Math.Sin(zTransform)) + (z * (float)Math.Cos(zTransform));
        }
        private IEnumerable Transform(IEnumerable coordinates)
        {
            float[] transformedCoordinates = coordinates.Cast<float>().ToArray();
            Transform(ref transformedCoordinates[0], ref transformedCoordinates[2]);
            return transformedCoordinates;
        }
        private IEnumerator Transform(IEnumerator coordinates)
        {
            float x = (float)coordinates.Current;
            coordinates.MoveNext();
            coordinates.MoveNext();
            float z = (float)coordinates.Current;
            IEnumerable collection = new float[]{ x, z };
            return Transform(collection).GetEnumerator();
        }

        static void Main(string[] args) //Currently, the command line arguments provide the instance ID - nothing more.
        {
            FaceLabSensor sensor = new FaceLabSensor(args); //Start instance of self in static entry point to begin execution of program.
            try
            {
                VectorUInt8 buffer = new VectorUInt8();
                InetAddress from = new InetAddress();
                DatagramSocket input_socket = new DatagramSocket(2001);
                //sensor.dirpath = args[1];

                while (true)
                {
                    buffer.Clear();
                    input_socket.ReceiveDatagram(buffer, from);
                    sensor.HandleBufferData(buffer, from);
                }
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine("{0}", e.Message);
            }

            Console.ReadLine(); //Keep the program running.
        }

        /// <summary>
        /// This constructor for the FaceLabsensor object provides
        /// an instance ID of the sensor to report to the Construct
        /// server.
        /// </summary>
        public FaceLabSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("9713e778-8fd2-44ae-b915-4ea991ae7633"), new Dictionary<string, Guid> { { "", Guid.Empty } })
        {
            broker.OnCommandReceived += new Action<object, string>(broker_OnCommandReceived);
        }

        void broker_OnCommandReceived(object arg1, string arg2)
        {
            Command command = JsonConvert.DeserializeObject<Command>(arg2);

            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                case "GlobalCoordinateTransformation":
                    xTransform = float.Parse(command.Args["xCoordinate"]);
                    zTransform = float.Parse(command.Args["zCoordinate"]);

                    break;
            }
        }


        /// <summary>
        /// Starts the sensor
        /// </summary>
        /// <returns>Returns a string identifying execution of the Start method call.</returns>
        protected override string Start()
        {
            return base.Start();
        }

        /// <summary>
        /// Stops the sensor from emitting data.
        /// </summary>
        /// <returns>Returns a string identifying execution of the Stop method call.</returns>
        protected override string Stop()
        {
            return base.Stop();
        }

        /// <summary>
        /// Allow for the sensor to be stopped.
        /// </summary>
        protected override void Exit()
        {
            base.Exit();
        }

        public void HandleBufferData(VectorUInt8 buffer, InetAddress from)
        {
            for (int i = 0; i < buffer.Count;) // Extract all the objects from the buffer
            {
                using (Serializable serializable = SerializableFactory.NewObject(buffer, ref i))
                {
                    if (serializable != null)
                        handleEngineOutputData(serializable, from);
                    else
                        System.Console.WriteLine("\nUnrecognised packet received, header id: {0}\n", (int)buffer[0]);
                }
            }
        }

        void handleEngineOutputData(Serializable outputData, InetAddress from)
        {
            this.SendItem(GetItems(), "FaceLabItem", DateTime.Now);
        }

        public IEnumerable<ConstructDataType> GetItems()
        {
            VectorInt8 objectsToRead = new VectorInt8();
            objectsToRead.Add((sbyte)TimingOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)HeadOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)EyeOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)WorldOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)FeatureSetsByCamera.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)CustomEventOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)GPSOutputDataV2.THIS_OBJECT_ID);

            using (EngineOutputDataFileReader reader = new EngineOutputDataFileReader())
            {
                
                reader.OpenFiles(objectsToRead, dirpath);
                VectorUInt8 buffer = new VectorUInt8();
                while (!reader.EndOfData())
                {
                    EngineOutputData data = reader.ReadData();
                    yield return GetTransformedConstructDataType(data);
                }
            }
        }
        private ConstructDataType GetTransformedConstructDataType(EngineOutputData data)
        {
            return new ConstructDataType()
            {
                EngineStateOutputData_ObjectID = data.EngineStateOutputData().ObjectID(),
                EngineStateOutputData_State = data.EngineStateOutputData().State(),
                EyeOutputData_EyeClosureOutputData_AverageBlinkDuration = data.EyeOutputData().EyeClosureOutputData().AverageBlinkDuration(),
                EyeOutputData_EyeClosureOutputData_BlinkFrequency = data.EyeOutputData().EyeClosureOutputData().BlinkFrequency(),
                EyeOutputData_EyeClosureOutputData_Blinking = data.EyeOutputData().EyeClosureOutputData().Blinking(),
                //EyeOutputData_EyeClosureOutputData_EyeClosure = data.EyeOutputData().EyeClosureOutputData().EyeClosure(),
                EyeOutputData_EyeClosureOutputData_EyeClosureCalibStatus = data.EyeOutputData().EyeClosureOutputData().EyeClosureCalibStatus(),
                //EyeOutputData_EyeClosureOutputData_EyeClosureConfidence = data.EyeOutputData().EyeClosureOutputData().EyeClosureConfidence(),
                EyeOutputData_EyeClosureOutputData_ObjectID = data.EyeOutputData().EyeClosureOutputData().ObjectID(),
                EyeOutputData_EyeClosureOutputData_Perclos = data.EyeOutputData().EyeClosureOutputData().Perclos(),
                //EyeOutputData_GazeOutputData_EyeballCentre_GetEnumerator = data.EyeOutputData().GazeOutputData().EyeballCentre().GetEnumerator(),
                EyeOutputData_GazeOutputData_GazeCalibrated = data.EyeOutputData().GazeOutputData().GazeCalibrated(),
                //EyeOutputData_GazeOutputData_GazeQualityLevel = data.EyeOutputData().GazeOutputData().GazeQualityLevel(),
                //EyeOutputData_GazeOutputData_GazeRotation_GetEnumerator = data.EyeOutputData().GazeOutputData().GazeRotation().GetEnumerator(),
                EyeOutputData_GazeOutputData_ObjectID = data.EyeOutputData().GazeOutputData().ObjectID(),
                EyeOutputData_GazeOutputData_Saccade = data.EyeOutputData().GazeOutputData().Saccade(),
                EyeOutputData_ObjectID = data.EyeOutputData().ObjectID(),
                EyeOutputData_PupilOutputData_ObjectID = data.EyeOutputData().PupilOutputData().ObjectID(),
                //EyeOutputData_PupilOutputData_PupilDiameter = data.EyeOutputData().PupilOutputData().PupilDiameter(),
                //EyeOutputData_PupilOutputData_PupilPosition_GetEnumerator = data.EyeOutputData().PupilOutputData().PupilPosition().GetEnumerator(),
                //FaceOutputData_FaceLandmarks_Get_fc_x = data.FaceOutputData().FaceLandmarks().Get().fc.x,
                //FaceOutputData_FaceLandmarks_Get_fc_y = data.FaceOutputData().FaceLandmarks().Get().fc.y,
                //FaceOutputData_FaceLandmarks_Get_fc_z = data.FaceOutputData().FaceLandmarks().Get().fc.z,
                //FaceOutputData_FaceLandmarks_Get_ftc_u = data.FaceOutputData().FaceLandmarks().Get().ftc.u,
                //FaceOutputData_FaceLandmarks_Get_ftc_v = data.FaceOutputData().FaceLandmarks().Get().ftc.v,
                //FaceOutputData_FaceLandmarks_Get_id = data.FaceOutputData().FaceLandmarks().Get().id,
                FaceOutputData_FaceLandmarks_Size = data.FaceOutputData().FaceLandmarks().Size(),
                //FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_x = data.FaceOutputData().FaceTexture().FaceMaskLandmarks().Get().fc.x,
                //FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_y = data.FaceOutputData().FaceTexture().FaceMaskLandmarks().Get().fc.y,
                //FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_z = data.FaceOutputData().FaceTexture().FaceMaskLandmarks().Get().fc.z,
                //FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_ftc_u = data.FaceOutputData().FaceTexture().FaceMaskLandmarks().Get().ftc.u,
                //FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_ftc_v = data.FaceOutputData().FaceTexture().FaceMaskLandmarks().Get().ftc.v,
                //FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_id = data.FaceOutputData().FaceTexture().FaceMaskLandmarks().Get().id,
                FaceOutputData_FaceTexture_FaceMaskLandmarks_Size = data.FaceOutputData().FaceTexture().FaceMaskLandmarks().Size(),
                FaceOutputData_FaceTexture_FaceTexture_BytesPerLine = data.FaceOutputData().FaceTexture().FaceTexture().BytesPerLine(),
                FaceOutputData_FaceTexture_FaceTexture_Height = data.FaceOutputData().FaceTexture().FaceTexture().Height(),
                FaceOutputData_FaceTexture_FaceTexture_ImageFormat = data.FaceOutputData().FaceTexture().FaceTexture().ImageFormat(),
                FaceOutputData_FaceTexture_FaceTexture_ImageIdentifier = data.FaceOutputData().FaceTexture().FaceTexture().ImageIdentifier(),
                FaceOutputData_FaceTexture_FaceTexture_IsSharingMemory = data.FaceOutputData().FaceTexture().FaceTexture().IsSharingMemory(),
                FaceOutputData_FaceTexture_FaceTexture_NumDataBytes = data.FaceOutputData().FaceTexture().FaceTexture().NumDataBytes(),
                FaceOutputData_FaceTexture_FaceTexture_ObjectID = data.FaceOutputData().FaceTexture().FaceTexture().ObjectID(),
                FaceOutputData_FaceTexture_FaceTexture_RawData = data.FaceOutputData().FaceTexture().FaceTexture().RawData(),
                FaceOutputData_FaceTexture_FaceTexture_Width = data.FaceOutputData().FaceTexture().FaceTexture().Width(),
                //FaceOutputData_FaceTexture_GetFaceTextureType = data.FaceOutputData().FaceTexture().GetFaceTextureType(),
                FaceOutputData_FaceTexture_ObjectID = data.FaceOutputData().FaceTexture().ObjectID(),
                FaceOutputData_ObjectID = data.FaceOutputData().ObjectID(),
                FaceOutputDataV2_FaceCount = data.FaceOutputData().FaceCount(),
                //FaceOutputDataV2_WearingGlasses = data.FaceOutputData().WearingGlasses(),
                GPSOutputData_GmtTime = data.GpsOutputData().GmtTime(),
                GPSOutputData_IsDataValid = data.GpsOutputData().IsDataValid(),
                GPSOutputData_Latitude_degrees = data.GpsOutputData().Latitude().degrees,
                GPSOutputData_Latitude_hemisphere = data.GpsOutputData().Latitude().hemisphere,
                GPSOutputData_Latitude_minutes = data.GpsOutputData().Latitude().minutes,
                GPSOutputData_Latitude_seconds = data.GpsOutputData().Latitude().seconds,
                GPSOutputData_Longitude_degrees = data.GpsOutputData().Longitude().degrees,
                GPSOutputData_Longitude_hemisphere = data.GpsOutputData().Longitude().hemisphere,
                GPSOutputData_Longitude_minutes = data.GpsOutputData().Longitude().minutes,
                GPSOutputData_Longitude_seconds = data.GpsOutputData().Longitude().seconds,
                GPSOutputData_ObjectID = data.GpsOutputData().ObjectID(),
                GPSOutputData_Speed = data.GpsOutputData().Speed(),
                GPSOutputDataV2_Course = data.GpsOutputDataV2().Course(),
                GPSOutputDataV2_GmtTime = data.GpsOutputDataV2().GmtTime(),
                GPSOutputDataV2_IsDataValid = data.GpsOutputDataV2().IsDataValid(),
                GPSOutputDataV2_Latitude_degrees = data.GpsOutputDataV2().Latitude().degrees,
                GPSOutputDataV2_Latitude_hemisphere = data.GpsOutputDataV2().Latitude().hemisphere,
                GPSOutputDataV2_Latitude_minutes = data.GpsOutputDataV2().Latitude().minutes,
                GPSOutputDataV2_Latitude_seconds = data.GpsOutputDataV2().Latitude().seconds,
                GPSOutputDataV2_Longitude_degrees = data.GpsOutputDataV2().Longitude().degrees,
                GPSOutputDataV2_Longitude_hemisphere = data.GpsOutputDataV2().Longitude().hemisphere,
                GPSOutputDataV2_Longitude_minutes = data.GpsOutputDataV2().Longitude().minutes,
                GPSOutputDataV2_Longitude_seconds = data.GpsOutputDataV2().Longitude().seconds,
                GPSOutputDataV2_ObjectID = data.GpsOutputDataV2().ObjectID(),
                GPSOutputDataV2_Speed = data.GpsOutputDataV2().Speed(),
                //HeadOutputData_HeadEyeBallPos_GetEnumerator = data.HeadOutputData().HeadEyeBallPos().GetEnumerator(),
                HeadOutputData_HeadPosition_GetEnumerator = Transform(data.HeadOutputData().HeadPosition().GetEnumerator()),
                //HeadOutputData_HeadPositionConfidence = data.HeadOutputData().HeadPositionConfidence(),
                HeadOutputData_HeadRotation_GetEnumerator = data.HeadOutputData().HeadRotation().GetEnumerator(),
                HeadOutputData_ModelQualityLevel = data.HeadOutputData().ModelQualityLevel(),
                HeadOutputData_ObjectID = data.HeadOutputData().ObjectID(),
                HeadOutputDataV2_HeadRotationQuaternion_GetEnumerator = data.HeadOutputData().HeadRotationQuaternion().GetEnumerator(),
                //ImageCollectionOutputData_Image_BytesPerLine = data.ImagesOutputData().Image().BytesPerLine(),
                //ImageCollectionOutputData_Image_Height = data.ImagesOutputData().Image().Height(),
                //ImageCollectionOutputData_Image_ImageFormat = data.ImagesOutputData().Image().ImageFormat(),
                //ImageCollectionOutputData_Image_ImageIdentifier = data.ImagesOutputData().Image().ImageIdentifier(),
                //ImageCollectionOutputData_Image_IsSharingMemory = data.ImagesOutputData().Image().IsSharingMemory(),
                //ImageCollectionOutputData_Image_NumDataBytes = data.ImagesOutputData().Image().NumDataBytes(),
                //ImageCollectionOutputData_Image_ObjectID = data.ImagesOutputData().Image().ObjectID(),
                //ImageCollectionOutputData_Image_RawData = data.ImagesOutputData().Image().RawData(),
                //ImageCollectionOutputData_Image_Width = data.ImagesOutputData().Image().Width(),
                ImageCollectionOutputData_NumImages = data.ImagesOutputData().NumImages(),
                ImageCollectionOutputData_ObjectID = data.ImagesOutputData().ObjectID(),
                InertiaOutputData_AccelX = data.InertiaOutputData().AccelX(),
                InertiaOutputData_AccelY = data.InertiaOutputData().AccelY(),
                InertiaOutputData_AccelZ = data.InertiaOutputData().AccelZ(),
                InertiaOutputData_AngularRateX = data.InertiaOutputData().AngularRateX(),
                InertiaOutputData_AngularRateY = data.InertiaOutputData().AngularRateY(),
                InertiaOutputData_AngularRateZ = data.InertiaOutputData().AngularRateZ(),
                InertiaOutputData_ObjectID = data.InertiaOutputData().ObjectID(),
                TimingOutputData_AnnotationLabelID = data.TimingOutputData().AnnotationLabelID(),
                TimingOutputData_ApproxDelay = data.TimingOutputData().ApproxDelay(),
                TimingOutputData_ExperimentTime = data.TimingOutputData().ExperimentTime(),
                TimingOutputData_FrameTimeMSecs = data.TimingOutputData().FrameTimeMSecs(),
                TimingOutputData_FrameTimeSecs = data.TimingOutputData().FrameTimeSecs(),
                TimingOutputData_ObjectID = data.TimingOutputData().ObjectID(),
                //VergencePoint_X = data.EyeOutputData().PupilOutputData().
                WorldOutputData_HeadIntersectionOutputData_ObjectID = data.WorldOutputData().HeadIntersectionOutputData().ObjectID(),
                WorldOutputData_ObjectID = data.WorldOutputData().ObjectID()
            };

        }
    }
}
