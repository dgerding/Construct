using Microsoft.Xna.Framework;
using sm.eod;
using sm.eod.io;
using sm.eod.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SMFramework
{
	public class FaceLabConnection : IDisposable
	{
		public FaceData CurrentData
		{
			get;
			private set;
		}

		public FaceLabConnection(FaceLabSignalConfiguration config)
		{
			Connect(config.Port, config.Label);
			CurrentData = new FaceData();
			CurrentData.SignalLabel = config.Label;
		}

#if DEBUG
		public bool IsValid
		{
			get { return true; }
		}

		public FaceLabConnection(String signalLabel)
		{
			CurrentData = new FaceData();
			CurrentData.SignalLabel = signalLabel;
		}

		private void Connect(int udpPort, String signalLabel)
		{
			//	Do nothing in Debug, the SeeingMachines DLLs don't run properly in Debug
		}
#else
		VectorUInt8 m_Buffer;
		InetAddress m_SourceIP;
		DatagramSocket m_DataSocket;
		int m_Port;

		public FaceLabConnection(int udpPort, String signalLabel)
		{
			Connect(udpPort, signalLabel);
		}

		public bool IsValid
		{
			get
			{
				return m_DataSocket != null /* && m_DataSocket.Connected() */;
			}
		}


		private void Connect(int udpPort, String signalLabel)
		{
			m_Buffer = new VectorUInt8();
			m_SourceIP = new InetAddress();
			CurrentData = new FaceData();
			CurrentData.SignalLabel = signalLabel;

			try
			{
				m_Port = udpPort;
				m_DataSocket = new DatagramSocket(m_Port);
			}
			catch (Exception e)
			{
				DebugOutputStream.SlowInstance.WriteLine("Unable to listen on port " + udpPort + "\nException: " + e.Message);
			}
		}
#endif

		public void Dispose()
		{
#if !DEBUG
			m_DataSocket = null;
			GC.Collect();
#endif
		}

		public void Disconnect()
		{
#if !DEBUG
			m_DataSocket.DisconnectSocket();
			m_DataSocket = null;
#endif
		}

		private Vector3 SeeingMachinesVectorToXNA(VectorDouble vec)
		{
			return new Vector3((float)vec[0], (float)vec[1], (float)vec[2]);
		}

		private Vector3 SeeingMachinesVectorToXNA3(VectorFloat vec)
		{
			return new Vector3(vec[0], vec[1], vec[2]);
		}

		private Vector2 SeeingMachinesVectorToXNA2(VectorFloat vec)
		{
			return new Vector2(vec[0], vec[1]);
		}

		private Vector3 FaceCoordinateToXNA3(FaceCoordinate coord)
		{
			return new Vector3(coord.x, coord.y, coord.z);
		}

		private bool LandmarkMapHasLeftEyebrow(FaceLandmarkMap map)
		{
			if (map == null)
				return false;

			return
				map.HasKey(400) &&
				map.HasKey(401) &&
				map.HasKey(402);
		}

		private bool LandmarkMapHasRightEyebrow(FaceLandmarkMap map)
		{
			if (map == null)
				return false;

			return
				map.HasKey(300) &&
				map.HasKey(301) &&
				map.HasKey(302);
		}

		private bool LandmarkMapHasFaceOutline(FaceLandmarkMap map)
		{
			if (map == null)
				return false;

			//	If we have at least a couple points, we assume we have an outline
			return map.HasKey(800) && map.HasKey(801);
		}

		private bool LandmarkMapHasMouthOutline(FaceLandmarkMap map)
		{
			if (map == null)
				return false;

			return
				map.HasKey(4) && map.HasKey(5) && // Reference points (far mouth corners)
				Enumerable.Range(100, 6).All((index) => map.HasKey(index)) && // Outer lip contour
				Enumerable.Range(200, 8).All((index) => map.HasKey(index)); // Inner lip contour
		}

		public bool RetrieveNewData()
		{
#if !DEBUG

			EngineOutputData data = new EngineOutputData();
			VectorUInt8 buffer = new VectorUInt8();
			data.Serialize(buffer);

			if (m_DataSocket.DataReady())
			{
				m_DataSocket.ReceiveDatagram(m_Buffer, m_SourceIP);
				int pos = 0;

				while (pos < m_Buffer.Count)
				{
					using (Serializable serializable = SerializableFactory.NewObject(m_Buffer, ref pos))
					{
						EngineOutputData eod = EngineOutputData.Downcast(serializable);

						if (eod != null)
						{
							CurrentData = new FaceData();

							EngineOutputData outputData = eod;

							FaceOutputDataV2 faceOutput = outputData.FaceOutputData();
							if (faceOutput != null)
							{
								CurrentData.WearingGlasses = faceOutput.WearingGlasses() == WearingGlassesType.WEARING_GLASSES_YES;
								CurrentData.LandmarkCount = faceOutput.FaceLandmarks().Size();

								FaceTextureOutputData faceTexture = faceOutput.FaceTexture();
								if (faceTexture != null)
								{
									CurrentData.FaceTextureData = faceTexture.FaceTexture();
								}

								FaceLandmarkMap landmarkMap = faceOutput.FaceLandmarks();

								//	See: Face Landmark Standard in FaceAPI docs for the magic landmark numbers

// 								#region Eye Outline Vertices
// 								//	Reference points (0-99) and Right Eye/Left Eye points (600-699/700-799)
// 
// 								#endregion

								#region Face Outline Vertices
								//	Mask points (800-899)
								if (LandmarkMapHasFaceOutline(landmarkMap))
								{
									List<Vector3> faceOutlineVertices = new List<Vector3>();
									List<Vector2> faceOutlineUVs = new List<Vector2>();
									//	Try to get as many outline marks as we can
									for (int i = 0; i < 99; i++)
									{
										FaceLandmark currentOutlineLandmark = landmarkMap.Get(800 + i);
										if (currentOutlineLandmark == null)
											break;

										faceOutlineVertices.Add(FaceCoordinateToXNA3(currentOutlineLandmark.fc));
										faceOutlineUVs.Add(new Vector2(currentOutlineLandmark.ftc.u, currentOutlineLandmark.ftc.v));
									}

									CurrentData.FaceBoundsVertices = faceOutlineVertices;
									CurrentData.FaceBoundsUVs = faceOutlineUVs;
								}
								else
								{
									CurrentData.FaceBoundsVertices = null;
									CurrentData.FaceBoundsUVs = null;
								}
								#endregion

								#region Mouth Outline Vertices
								//	Reference points, and inner/outer lip contours

								if (LandmarkMapHasMouthOutline(landmarkMap))
								{
									//	Points added from top-to-bottom, left-to-right
									CurrentData.MouthOuterUpperLipVertices = new List<Vector3>();
									CurrentData.MouthOuterUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(4).fc));
									CurrentData.MouthOuterUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(100).fc));
									CurrentData.MouthOuterUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(101).fc));
									CurrentData.MouthOuterUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(102).fc));
									CurrentData.MouthOuterUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(5).fc));

									CurrentData.MouthInnerUpperLipVertices = new List<Vector3>();
									CurrentData.MouthInnerUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(200).fc));
									CurrentData.MouthInnerUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(201).fc));
									CurrentData.MouthInnerUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(202).fc));
									CurrentData.MouthInnerUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(203).fc));
									CurrentData.MouthInnerUpperLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(204).fc));

									CurrentData.MouthInnerLowerLipVertices = new List<Vector3>();
									CurrentData.MouthInnerLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(200).fc));
									CurrentData.MouthInnerLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(207).fc));
									CurrentData.MouthInnerLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(206).fc));
									CurrentData.MouthInnerLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(205).fc));
									CurrentData.MouthInnerLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(204).fc));

									CurrentData.MouthOuterLowerLipVertices = new List<Vector3>();
									CurrentData.MouthOuterLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(4).fc));
									CurrentData.MouthOuterLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(105).fc));
									CurrentData.MouthOuterLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(104).fc));
									CurrentData.MouthOuterLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(103).fc));
									CurrentData.MouthOuterLowerLipVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(5).fc));
								}
								else
								{
									CurrentData.MouthOuterUpperLipVertices = null;
									CurrentData.MouthInnerUpperLipVertices = null;
									CurrentData.MouthInnerLowerLipVertices = null;
									CurrentData.MouthOuterLowerLipVertices = null;
								}

								#endregion

								#region Eyebrow Outline Vertices

								if (LandmarkMapHasLeftEyebrow(landmarkMap))
								{
									CurrentData.LeftEyebrowVertices = new List<Vector3>();
									CurrentData.LeftEyebrowVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(400).fc));
									CurrentData.LeftEyebrowVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(401).fc));
									CurrentData.LeftEyebrowVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(402).fc));
								}
								else
								{
									CurrentData.LeftEyebrowVertices = null;
								}

								if (LandmarkMapHasRightEyebrow(landmarkMap))
								{
									CurrentData.RightEyebrowVertices = new List<Vector3>();
									CurrentData.RightEyebrowVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(300).fc));
									CurrentData.RightEyebrowVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(301).fc));
									CurrentData.RightEyebrowVertices.Add(FaceCoordinateToXNA3(landmarkMap.Get(302).fc));
								}
								else
								{
									CurrentData.RightEyebrowVertices = null;
								}

								#endregion
							}

							TimingOutputData timeOutput = outputData.TimingOutputData();

							HeadOutputDataV2 headOutput = outputData.HeadOutputData();
							if (headOutput != null)
							{
								CurrentData.HeadPosition = SeeingMachinesVectorToXNA(headOutput.HeadPosition());
								CurrentData.RightEyePosition = SeeingMachinesVectorToXNA(headOutput.HeadEyeBallPos(EyeId.RIGHT_EYE));
								CurrentData.LeftEyePosition = SeeingMachinesVectorToXNA(headOutput.HeadEyeBallPos(EyeId.LEFT_EYE));

								CurrentData.HeadRotation = SeeingMachinesVectorToXNA(headOutput.HeadRotation());

								CurrentData.HeadPositionConfidence = (float)headOutput.HeadPositionConfidence();
							}

							EyeOutputData eyeOutput = outputData.EyeOutputData();
							if (eyeOutput != null)
							{
								PupilOutputData pupilOutput = outputData.EyeOutputData().PupilOutputData();
								if (pupilOutput != null)
								{
									CurrentData.RightPupilDiameter = pupilOutput.PupilDiameter(0);
									CurrentData.LeftPupilDiameter = pupilOutput.PupilDiameter(1);

									CurrentData.RightPupilPosition = SeeingMachinesVectorToXNA3(pupilOutput.PupilPosition(0));
									CurrentData.LeftPupilPosition = SeeingMachinesVectorToXNA3(pupilOutput.PupilPosition(1));
								}

								EyeClosureOutputData closureOutput = eyeOutput.EyeClosureOutputData();
								if (closureOutput != null)
								{
									CurrentData.IsBlinking = closureOutput.Blinking();
									CurrentData.RightEyeClosure = closureOutput.EyeClosure((int)EyeId.RIGHT_EYE);
									CurrentData.LeftEyeClosure = closureOutput.EyeClosure((int)EyeId.LEFT_EYE);
									CurrentData.RightEyeClosureConfidence = closureOutput.EyeClosureConfidence((int)EyeId.RIGHT_EYE);
									CurrentData.LeftEyeClosureConfidence = closureOutput.EyeClosureConfidence((int)EyeId.LEFT_EYE);
								}

								GazeOutputData gazeOutput = eyeOutput.GazeOutputData();
								if (gazeOutput != null)
								{
									CurrentData.RightEyeGazeRotation = SeeingMachinesVectorToXNA2(gazeOutput.GazeRotation((int)EyeId.RIGHT_EYE));
									CurrentData.LeftEyeGazeRotation = SeeingMachinesVectorToXNA2(gazeOutput.GazeRotation((int)EyeId.LEFT_EYE));
									CurrentData.RightEyeGazeQualityLevel = (int)gazeOutput.GazeQualityLevel((int)EyeId.RIGHT_EYE);
									CurrentData.LeftEyeGazeQualityLevel = (int)gazeOutput.GazeQualityLevel((int)EyeId.LEFT_EYE);
								}
							}


							CurrentData.CoordinateSystem = FaceData.CoordinateSystemType.Local;
							CurrentData.SnapshotTimestamp = DateTime.UtcNow;
							CurrentData.FaceLabFrameIndex = outputData.FrameNum();


							/* WARNING: This is in a try/catch since GazeVergencePoint fails whenever there isn't
							 * enough data for calculation (which is quite often.) Even though we catch the
							 * statement, if the program is being ran through VisualStudio the exception will
							 * still be reported and output to the Debugger console. This will cause noticeable
							 * lag if you have more than 1 connection, easily dropping the FPS below 60. This
							 * bug doesn't occur when the application is ran standalone.
							 */
							try
							{
								CurrentData.VergencePosition = SeeingMachinesVectorToXNA3(eodutils.GazeVergencePoint(outputData));
							}
							catch
							{
								CurrentData.VergencePosition = Vector3.Zero;
							}
						}
						else
						{
							DebugOutputStream.SlowInstance.WriteLine("Unrecognised packet received, header id: {0}\n" + (int)m_Buffer[0]);
						}
					}
				}
			
				m_Buffer.Clear();
				return true;
			}
			else
			{
				m_Buffer.Clear();
				return false;
			}

#else
			var newData = new FaceData();

			//	Fill fake geometry
			newData.SignalLabel = CurrentData.SignalLabel;
			newData.CoordinateSystem = FaceData.CoordinateSystemType.Local;
			newData.HeadPosition = new Vector3(0.1f, 0.1f, 0.1f);

			const float featuresPlaneZ = -Units.Meter * 0.075f;

			newData.LeftEyebrowVertices = new List<Vector3>();
			newData.LeftEyebrowVertices.Add(new Vector3(-Units.Centimeter * 6, Units.Centimeter * 2, featuresPlaneZ));
			newData.LeftEyebrowVertices.Add(new Vector3(-Units.Centimeter * 4, Units.Centimeter * 2.5f, featuresPlaneZ));
			newData.LeftEyebrowVertices.Add(new Vector3(-Units.Centimeter * 2, Units.Centimeter * 2, featuresPlaneZ));

			newData.RightEyebrowVertices = new List<Vector3>();
			newData.RightEyebrowVertices.Add(new Vector3(Units.Centimeter * 2, Units.Centimeter * 2, featuresPlaneZ));
			newData.RightEyebrowVertices.Add(new Vector3(Units.Centimeter * 4, Units.Centimeter * 2.5f, featuresPlaneZ));
			newData.RightEyebrowVertices.Add(new Vector3(Units.Centimeter * 6, Units.Centimeter * 2, featuresPlaneZ));

			newData.MouthOuterUpperLipVertices = new List<Vector3>();
			newData.MouthOuterUpperLipVertices.Add(new Vector3(Units.Centimeter * 4, -Units.Centimeter * 7, featuresPlaneZ));
			newData.MouthOuterUpperLipVertices.Add(new Vector3(Units.Centimeter * 2, -Units.Centimeter * 6.2f, featuresPlaneZ));
			newData.MouthOuterUpperLipVertices.Add(new Vector3(0, -Units.Centimeter * 6, featuresPlaneZ));
			newData.MouthOuterUpperLipVertices.Add(new Vector3(-Units.Centimeter * 2, -Units.Centimeter * 6.2f, featuresPlaneZ));
			newData.MouthOuterUpperLipVertices.Add(new Vector3(-Units.Centimeter * 4, -Units.Centimeter * 7, featuresPlaneZ));

			newData.MouthInnerUpperLipVertices = new List<Vector3>();
			newData.MouthInnerUpperLipVertices.Add(new Vector3(Units.Centimeter * 3.5f, -Units.Centimeter * 7, featuresPlaneZ));
			newData.MouthInnerUpperLipVertices.Add(new Vector3(Units.Centimeter * 1.67f, -Units.Centimeter * 6.5f, featuresPlaneZ));
			newData.MouthInnerUpperLipVertices.Add(new Vector3(0, -Units.Centimeter * 6.3f, featuresPlaneZ));
			newData.MouthInnerUpperLipVertices.Add(new Vector3(-Units.Centimeter * 1.67f, -Units.Centimeter * 6.5f, featuresPlaneZ));
			newData.MouthInnerUpperLipVertices.Add(new Vector3(-Units.Centimeter * 3.5f, -Units.Centimeter * 7, featuresPlaneZ));

			newData.MouthInnerLowerLipVertices = new List<Vector3>();
			newData.MouthInnerLowerLipVertices.Add(new Vector3(Units.Centimeter * 3.5f, -Units.Centimeter * 7, featuresPlaneZ));
			newData.MouthInnerLowerLipVertices.Add(new Vector3(Units.Centimeter * 1.67f, -Units.Centimeter * 7.5f, featuresPlaneZ));
			newData.MouthInnerLowerLipVertices.Add(new Vector3(0, -Units.Centimeter * 7.7f, featuresPlaneZ));
			newData.MouthInnerLowerLipVertices.Add(new Vector3(-Units.Centimeter * 1.67f, -Units.Centimeter * 7.5f, featuresPlaneZ));
			newData.MouthInnerLowerLipVertices.Add(new Vector3(-Units.Centimeter * 3.5f, -Units.Centimeter * 7, featuresPlaneZ));

			newData.MouthOuterLowerLipVertices = new List<Vector3>();
			newData.MouthOuterLowerLipVertices.Add(new Vector3(Units.Centimeter * 4, -Units.Centimeter * 7, featuresPlaneZ));
			newData.MouthOuterLowerLipVertices.Add(new Vector3(Units.Centimeter * 2, -Units.Centimeter * 7.8f, featuresPlaneZ));
			newData.MouthOuterLowerLipVertices.Add(new Vector3(0, -Units.Centimeter * 8, featuresPlaneZ));
			newData.MouthOuterLowerLipVertices.Add(new Vector3(-Units.Centimeter * 2, -Units.Centimeter * 7.8f, featuresPlaneZ));
			newData.MouthOuterLowerLipVertices.Add(new Vector3(-Units.Centimeter * 4, -Units.Centimeter * 7, featuresPlaneZ));

			CurrentData = newData;

			return true;
#endif
		}
	}
}
