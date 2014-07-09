using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SMFramework;
using System;
using System.IO;
using System.Diagnostics;
using SMVisualization.Visualization;

/*
 * Built-in model-based world representation
 * Ability to toggle whether data is stored in local/global space
 * Update header to include SeeingModule-specific information (orientation)
 * Update header to include the coordinate system used (global or camera-space-local)
 * 
 * Tidy up code
 * 
 */

namespace SMVisualization
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class SMVisualization : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		#region Public Properties
		public static Random Random = new Random();

		public static GraphicsDevice Device;

		public static SMVisualization Instance;

		public static float TimeDelta;

		public static Visualization.InputManager Input;

		public static Visualization.FpsTracker FPS = new Visualization.FpsTracker ();

		public static FaceLabConnection[] Signals;
		#endregion

		
		public Visualization.WorldStage World;

		public Texture2D[] SubjectVideoStreamFrames;

		public PlaybackDataStream PlaybackStream;
		public AsyncPairedDatabase SensorStream = new AsyncPairedDatabase(DatabaseFormatMapping.GenerateRecordingFilename());
		public bool IsUsingPlayback = false;

		public SMFramework.SensorClusterConfiguration Sensors = null;

		private Vector2 m_PreviousMousePosition;

		private RenderOptionsForm m_RenderOptionsForm = null;
		private VisualizationRenderOptions m_RenderOptions;

		private PrimitiveBatch m_PrimitiveBatch;

		public void SpawnRenderOptions()
		{
			if (m_RenderOptionsForm != null)
			{
				m_RenderOptionsForm.BringToFront();	//	Bring the window to the foreground
			}
			else
			{
				m_RenderOptionsForm = new RenderOptionsForm(m_RenderOptions);
				m_RenderOptionsForm.FormClosing += m_RenderOptionsForm_FormClosing;
			}
			m_RenderOptionsForm.Show();
		}

		void m_RenderOptionsForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			m_RenderOptionsForm = null;
		}



		#region Window Management
		ScrubberForm scrubber;
		System.Windows.Forms.Form gameWindow;

		public bool HasFocus
		{
			get
			{
				MouseState mouse = Mouse.GetState();
				return
					gameWindow.ContainsFocus &&
					mouse.X > 0 &&
					mouse.Y > 0 &&
					mouse.X < GraphicsDevice.Viewport.Width &&
					mouse.Y < GraphicsDevice.Viewport.Height;
			}
		}

		void gameWindow_GotFocus(object sender, EventArgs e)
		{
			scrubber.TopMost = true;
			gameWindow.TopMost = true;

			scrubber.TopMost = false;
			gameWindow.TopMost = false;

			Input.UpdateInput();
			MouseState mouse = Input.GetMouse(Visualization.InputManager.DefaultAuthKey);
			m_PreviousMousePosition = new Vector2(mouse.X, mouse.Y);
		}

		void gameWindow_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			scrubber.Close();
		}

		void UpdateScrubberWindowLocation()
		{
			scrubber.SetDesktopLocation(gameWindow.DesktopLocation.X, gameWindow.DesktopLocation.Y + gameWindow.Size.Height);
		}

		void gameWindow_Move(object sender, EventArgs e)
		{
			UpdateScrubberWindowLocation();
		}
		#endregion


		public SMVisualization()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 600;

			Content.RootDirectory = "Content";

			System.IO.Directory.CreateDirectory("Data");
		}

		protected override void Initialize()
		{
			base.Initialize();

			scrubber = new ScrubberForm(SensorStream);
			scrubber.Show();

			gameWindow = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
			gameWindow.Move += gameWindow_Move;
			gameWindow.GotFocus += gameWindow_GotFocus;
			gameWindow.FormClosing += gameWindow_FormClosing;

			gameWindow.SetDesktopLocation(gameWindow.DesktopLocation.X, 20);

			UpdateScrubberWindowLocation();
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();

			SensorStream.Stop(false);
		}

		public void ReloadContent()
		{
			Vector3 camPos = Visualization.Camera.Position;
			Vector2 camRot = Visualization.Camera.Rotation;
			LoadContent();
			Visualization.Camera.Position = camPos;
			Visualization.Camera.Rotation = camRot;
		}

		protected override void LoadContent()
		{
			this.IsFixedTimeStep = false;
			Device = GraphicsDevice;
			Instance = this;

			spriteBatch = new SpriteBatch(GraphicsDevice);
			m_PrimitiveBatch = new PrimitiveBatch(GraphicsDevice);

			Visualization.DebugView.LoadContent();
			Visualization.DebugView.Enabled = false;
			Input = new Visualization.InputManager();


			if (File.Exists("Content/defaultcluster.cfg"))
			{
				Sensors = new SensorClusterConfiguration("Content/defaultcluster.cfg", SensorClusterConfiguration.FileLoadResponse.FailIfMissing);
			}
			else
			{
				System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
				dialog.FileName = "Sensor Cluster Configuration";

				System.Windows.Forms.DialogResult result = dialog.ShowDialog();

				if (result != System.Windows.Forms.DialogResult.OK)
				{
					this.Exit();
					return;
				}

				Sensors = new SensorClusterConfiguration(dialog.FileName, SensorClusterConfiguration.FileLoadResponse.DoNothingIfMissing);
			}

			IsMouseVisible = true;

			Signals = new FaceLabConnection[Sensors.SensorConfigurations.Count];
			SubjectVideoStreamFrames = new Texture2D[Sensors.SensorConfigurations.Count];

			for (int i = 0; i < Signals.Length; i++)
			{
#if DEBUG
				Signals[i] = new FaceLabConnection(Sensors.SensorConfigurations[i].Label);
#else
				Signals[i] = new FaceLabConnection(Sensors.SensorConfigurations[i].Port, Sensors.SensorConfigurations[i].Label);
#endif
			}

			if (m_RenderOptions == null)
				m_RenderOptions = new VisualizationRenderOptions(Sensors.SensorConfigurations.Count);

			World = new Visualization.WorldStage(Sensors, m_RenderOptions);

			Color[] fakeVideoFrame = new Color[1280 * 720];
			for (int i = 0; i < fakeVideoFrame.Length; i++)
				fakeVideoFrame[i] = new Color(i % 255, i % 255, i % 255);

			for (int i = 0; i < SubjectVideoStreamFrames.Length; i++)
			{
				SubjectVideoStreamFrames[i] = new Texture2D(GraphicsDevice, 1280, 720);
				SubjectVideoStreamFrames[i].SetData(fakeVideoFrame);
			}
		}

		protected override void Update(GameTime gameTime)
		{
			if (Sensors == null)
				return;

			TimeDelta = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

			if (HasFocus)
				Input.UpdateInput();

			if (Input.GetKeyboard (Visualization.InputManager.DefaultAuthKey).IsKeyDown (Keys.Escape))
				this.Exit();

			base.Update(gameTime);

			#region Camera Management
			MouseState mouse = SMVisualization.Input.GetMouse(Visualization.InputManager.DefaultAuthKey);
			if (mouse.LeftButton == ButtonState.Pressed && SMVisualization.Instance.HasFocus)
			{
				Visualization.Camera.TurnByMouseDelta(mouse.X - m_PreviousMousePosition.X, mouse.Y - m_PreviousMousePosition.Y);
			}

			m_PreviousMousePosition.X = mouse.X;
			m_PreviousMousePosition.Y = mouse.Y;

			KeyboardState keyboard = SMVisualization.Input.GetKeyboard(Visualization.InputManager.DefaultAuthKey);
			if (keyboard.IsKeyDown(Keys.W))
				Visualization.Camera.Move(Visualization.Camera.MoveDirection.Forward);
			if (keyboard.IsKeyDown(Keys.S))
				Visualization.Camera.Move(Visualization.Camera.MoveDirection.Backward);
			if (keyboard.IsKeyDown(Keys.A))
				Visualization.Camera.Move(Visualization.Camera.MoveDirection.Left);
			if (keyboard.IsKeyDown(Keys.D))
				Visualization.Camera.Move(Visualization.Camera.MoveDirection.Right);
			if (keyboard.IsKeyDown(Keys.Z))
				Visualization.Camera.Move(Visualization.Camera.MoveDirection.Down);
			if (keyboard.IsKeyDown(Keys.X))
				Visualization.Camera.Move(Visualization.Camera.MoveDirection.Up);
			#endregion

			if (PlaybackStream == null)
			{
				for (int i = 0; i < Signals.Length; i++)
				{
					Signals[i].RetrieveNewData();
					ImageOutputDataConverter converter = new ImageOutputDataConverter();
					if (Signals[i].CurrentData.FaceTextureData != null)
					{
						SubjectVideoStreamFrames[i].Dispose();
						SubjectVideoStreamFrames[i] = converter.Convert(GraphicsDevice, Signals[i].CurrentData.FaceTextureData);
					}
					World.UpdateModuleToFaceData(i, Signals[i].CurrentData, SensorStream);
				}
			}
			else
			{
				FaceData[] extractedResults = PlaybackStream.CurrentDataInterpretation;
				for (int i = 0; i < Signals.Length; i++)
				{
					if (scrubber.cbGlobalSource.Checked)
						extractedResults[i].CoordinateSystem = FaceData.CoordinateSystemType.Global;
					else
						extractedResults[i].CoordinateSystem = FaceData.CoordinateSystemType.Local;

					World.UpdateModuleToFaceData(i, extractedResults[i], null);
				}
			}

			if (PlaybackStream != null && IsUsingPlayback)
			{
				PlaybackStream.TargetRecordingTime += TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds);
			}

			SensorStream.BeginNextSnapshot();
		}

		protected override void Draw(GameTime gameTime)
		{
			if (Sensors == null)
				return;

			Visualization.DebugView.AddText("Memory usage: " + Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024) + "MB");
            Visualization.DebugView.AddText("Snapshots #: " + SensorStream.NumberOfSnapshots);
			if (PlaybackStream != null)
			{
				Visualization.DebugView.AddText("Playback Frame: " + PlaybackStream.CurrentFrame);
			}

			FPS.MarkNewFrame();

			GraphicsDevice.Clear(Color.CornflowerBlue);

			World.Draw();

			m_PrimitiveBatch.ActiveType = PrimitiveBatch.ListType.Single;
			m_PrimitiveBatch.ActiveColor = Color.Green;

			FacialAttendanceDetector facialAttendanceDetector = new FacialAttendanceDetector();
			for (int i = 0; i < World.LastFaceData.Length; i++)
			{
				if (!m_RenderOptions.SubjectOptions[i].DrawFacialAttendanceInfo)
					continue;

				FaceData currentFace = World.LastFaceData[i];
				if (currentFace == null)
					continue;

				if (currentFace.CoordinateSystem == FaceData.CoordinateSystemType.Local)
					currentFace = SeeingModule.EvaluateCameraData(Sensors.SensorConfigurations[i], currentFace, false);

				for (int j = 0; j < World.LastFaceData.Length; j++)
				{
					if (i == j)
						continue;

					FaceData targetFace = World.LastFaceData[j];
					if (targetFace == null)
						continue;

					if (targetFace.CoordinateSystem == FaceData.CoordinateSystemType.Local)
						targetFace = SeeingModule.EvaluateCameraData(Sensors.SensorConfigurations[j], targetFace, false);

					float thisFrameAttendance = facialAttendanceDetector.GetFacialAttendance(currentFace, targetFace);

					if (thisFrameAttendance < 0.4f)
						continue;

					Vector3 attendanceLineStart = currentFace.HeadPosition;
					Vector3 attendenceLineVector = targetFace.HeadPosition - currentFace.HeadPosition;

					attendanceLineStart += attendenceLineVector / 5;
					Vector3 attendenceLineEnd = attendanceLineStart + attendenceLineVector / 5;

					m_PrimitiveBatch.AddLine(attendanceLineStart, attendenceLineEnd);
				}
			}

			m_PrimitiveBatch.DrawPolygons(Camera.ViewspaceMatrix);

			spriteBatch.Begin();
			Rectangle drawArea = new Rectangle(0, 0, 160, 120);
			for (int i = 0; i < SubjectVideoStreamFrames.Length; i++)
			{
				drawArea.X = i * drawArea.Width + i * 2;
				drawArea.Y = 0;
				if (m_RenderOptions.SubjectOptions[i].DrawCameraStream)
					spriteBatch.Draw(SubjectVideoStreamFrames[i], drawArea, Color.White);
			}
			spriteBatch.End();

			Visualization.DebugView.Draw();

			base.Draw(gameTime);
		}
	}
}
