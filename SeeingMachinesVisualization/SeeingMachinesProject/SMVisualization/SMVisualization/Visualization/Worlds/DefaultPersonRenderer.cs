using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMVisualization.Visualization.Worlds
{
	public class DefaultPersonRenderer : PersonRenderer
	{
		PrimitiveBatch m_PrimitiveBatch;

		Dictionary<String, PersonOrientation> m_DisplayCache = new Dictionary<String, PersonOrientation>();

		public DefaultPersonRenderer()
		{
			m_PrimitiveBatch = new PrimitiveBatch(SMVisualization.Instance.GraphicsDevice);
		}

		void UpdateCurrentData(SeeingModule newData)
		{
			if (!m_DisplayCache.ContainsKey(newData.SignalLabel))
				m_DisplayCache[newData.SignalLabel] = new PersonOrientation();

			//	Check to see if values are "filled" before we try to work with them
			Vector3 moduleOrigin = SeeingModule.SensorOrigin(newData.SensorConfiguration);

			PersonOrientation oldPerson = m_DisplayCache[newData.SignalLabel];
			PersonOrientation newPerson = newData.Person;

			if (newPerson.LeftEyePosition == moduleOrigin)
				newPerson.LeftEyePosition = oldPerson.LeftEyePosition;
			if (newPerson.RightEyePosition == moduleOrigin)
				newPerson.RightEyePosition = oldPerson.RightEyePosition;

			if (newPerson.LeftPupilPosition == moduleOrigin)
				newPerson.LeftPupilPosition = oldPerson.LeftPupilPosition;
			if (newPerson.RightPupilPosition == moduleOrigin)
				newPerson.RightPupilPosition = oldPerson.RightPupilPosition;

			if (newPerson.WorldSpaceForeheadPosition == moduleOrigin)
				newPerson.WorldSpaceForeheadPosition = oldPerson.WorldSpaceForeheadPosition;

			m_DisplayCache[newData.SignalLabel] = newPerson;
		}

		public void Draw(SeeingModule data, SeeingModule[] moduleBatch, SubjectRenderOptions renderOptions)
		{
			//	If no render options were specified, use the default options (new instance)
			if (renderOptions == null)
				renderOptions = new SubjectRenderOptions();

			UpdateCurrentData(data);
			var person = m_DisplayCache[data.SignalLabel];

			if (!renderOptions.DrawSubject)
				return;

			Matrix personHeadMatrix = Matrix.CreateRotationZ(person.RotationEuler.Z) *
					Matrix.CreateRotationX(person.RotationEuler.X) *
					Matrix.CreateRotationY(person.RotationEuler.Y) *
					Matrix.CreateTranslation(person.WorldSpacePosition) *
					Camera.ViewspaceMatrix;

			if (renderOptions.DrawHeadModel)
			{
				m_PrimitiveBatch.ActiveColor = Color.Tan;
				ShapeHelper.DrawEllipse(Vector3.Zero, person.Size, m_PrimitiveBatch);
				//Vector3 sphereSize = new Vector3((person.Size.X + person.Size.Y + person.Size.Z) / 3.0f);
				//ShapeHelper.DrawEllipse(Vector3.Zero, sphereSize, m_PrimitiveBatch);

				m_PrimitiveBatch.DrawPolygons(personHeadMatrix);
			}

			m_PrimitiveBatch.ActiveColor = Color.White;
			if (renderOptions.DrawEyes)
			{
				ShapeHelper.DrawEllipse(person.LeftEyePosition, Vector3.One * 0.025F, m_PrimitiveBatch);
				ShapeHelper.DrawEllipse(person.RightEyePosition, Vector3.One * 0.025F, m_PrimitiveBatch);

				if (renderOptions.DrawPupils)
				{
					m_PrimitiveBatch.ActiveColor = Color.Black;
					ShapeHelper.DrawEllipse(person.LeftPupilPosition, Vector3.One * 0.005F, m_PrimitiveBatch);
					ShapeHelper.DrawEllipse(person.RightPupilPosition, Vector3.One * 0.005F, m_PrimitiveBatch);
				}
			}

			if (renderOptions.DrawHeadOrientationAttendance)
			{
				Color previousColor = m_PrimitiveBatch.ActiveColor;

				Vector3 headOrientationNormal = new Vector3(
					(float)(Math.Sin(-person.RotationEuler.Y) * Math.Cos(person.RotationEuler.X)),
					(float)Math.Sin(person.RotationEuler.X),
					-(float)(Math.Cos(-person.RotationEuler.Y) * Math.Cos(person.RotationEuler.X))
				) * 20.0f;

				FacialAttendanceDetector facialAttendanceDetector = new FacialAttendanceDetector();
				foreach (var module in moduleBatch)
				{
					if (module == data)
						continue;

					Vector3 currentTargetHeadSize;
					if (m_DisplayCache.ContainsKey(module.SignalLabel))
						currentTargetHeadSize = m_DisplayCache[module.SignalLabel].Size;
					else
						currentTargetHeadSize = person.Size;

					if (facialAttendanceDetector.IsHeadOrientationAttending(data.LastFaceData, module.LastFaceData, currentTargetHeadSize))
					{
						m_PrimitiveBatch.ActiveColor = Color.Green;
						break;
					}
				}

				m_PrimitiveBatch.AddLine(person.WorldSpacePosition, person.WorldSpacePosition + headOrientationNormal);

				m_PrimitiveBatch.ActiveColor = previousColor;
			}

			if (renderOptions.DrawGazeRay)
				m_PrimitiveBatch.AddLine(person.WorldSpaceForeheadPosition, person.VergencePoint);

			if (renderOptions.DrawExtendedGazeRay)
			{
				Vector3 vergenceNormal = (person.VergencePoint - person.WorldSpaceForeheadPosition);
				vergenceNormal /= vergenceNormal.Length();
				m_PrimitiveBatch.AddLine(person.WorldSpaceForeheadPosition, person.WorldSpaceForeheadPosition + vergenceNormal * 10.0F);
			}

			float rayLength = Units.Centimeter * 5.0F;

			Vector3 leftGazeRay = new Vector3(
					(float)(Math.Sin(-person.LeftEyeGaze.Y) * Math.Cos(person.LeftEyeGaze.X)),
					(float)Math.Sin(person.LeftEyeGaze.X),
					-(float)(Math.Cos(-person.LeftEyeGaze.Y) * Math.Cos(person.LeftEyeGaze.X))
				) * rayLength;

			Vector3 rightGazeRay = new Vector3(
					(float)(Math.Sin(-person.RightEyeGaze.Y) * Math.Cos(person.RightEyeGaze.X)),
					(float)Math.Sin(person.RightEyeGaze.X),
					-(float)(Math.Cos(-person.RightEyeGaze.Y) * Math.Cos(person.RightEyeGaze.X))
				) * rayLength;

			m_PrimitiveBatch.ActiveColor = Color.Yellow;
			
			if (renderOptions.DrawRightEyeGaze)
				m_PrimitiveBatch.AddLine(person.RightPupilPosition, person.RightPupilPosition + rightGazeRay);
			if (renderOptions.DrawLeftEyeGaze)
				m_PrimitiveBatch.AddLine(person.LeftPupilPosition, person.LeftPupilPosition + leftGazeRay);
	
			m_PrimitiveBatch.DrawPolygons(Camera.ViewspaceMatrix);

			m_PrimitiveBatch.DepthState = PrimitiveBatch.NoDepthState;

			//	Draw facial features
			if (renderOptions.DrawEyeBrows)
			{
				m_PrimitiveBatch.ActiveType = PrimitiveBatch.ListType.LineStrip;

				if (person.DataSource.LeftEyebrowVertices != null)
				{
					foreach (Vector3 vertex in person.DataSource.LeftEyebrowVertices)
						m_PrimitiveBatch.AddVertex(vertex);
					m_PrimitiveBatch.DrawPolygons(personHeadMatrix);
				}

				if (person.DataSource.RightEyebrowVertices != null)
				{
					foreach (Vector3 vertex in person.DataSource.RightEyebrowVertices)
						m_PrimitiveBatch.AddVertex(vertex);
					m_PrimitiveBatch.DrawPolygons(personHeadMatrix);
				}
			}

			if (renderOptions.DrawMouth)
			{
				if (person.DataSource.MouthOuterUpperLipVertices != null)
				{
					m_PrimitiveBatch.ActiveType = PrimitiveBatch.ListType.LineStrip;

					//	If one set is available they're all available
					foreach (Vector3 vertex in person.DataSource.MouthOuterUpperLipVertices)
						m_PrimitiveBatch.AddVertex(vertex);
					m_PrimitiveBatch.DrawPolygons(personHeadMatrix);

					foreach (Vector3 vertex in person.DataSource.MouthInnerUpperLipVertices)
						m_PrimitiveBatch.AddVertex(vertex);
					m_PrimitiveBatch.DrawPolygons(personHeadMatrix);

					foreach (Vector3 vertex in person.DataSource.MouthInnerLowerLipVertices)
						m_PrimitiveBatch.AddVertex(vertex);
					m_PrimitiveBatch.DrawPolygons(personHeadMatrix);

					foreach (Vector3 vertex in person.DataSource.MouthOuterLowerLipVertices)
						m_PrimitiveBatch.AddVertex(vertex);
					m_PrimitiveBatch.DrawPolygons(personHeadMatrix);

					m_PrimitiveBatch.ActiveType = PrimitiveBatch.ListType.Single;
					m_PrimitiveBatch.AddLine(person.DataSource.MouthOuterUpperLipVertices[0], person.DataSource.MouthInnerUpperLipVertices[0]);
					m_PrimitiveBatch.AddLine(person.DataSource.MouthOuterUpperLipVertices.Last(), person.DataSource.MouthInnerUpperLipVertices.Last());
					m_PrimitiveBatch.DrawPolygons(personHeadMatrix);
				}
			}

			m_PrimitiveBatch.DepthState = PrimitiveBatch.DefaultDepthState;
			m_PrimitiveBatch.ActiveType = PrimitiveBatch.ListType.Single;
			m_PrimitiveBatch.ActiveColor = Color.White;

			/* Draw the camera reference position */
			ShapeHelper.DrawBox(
				data.SensorConfiguration.GlobalTranslation +
					Vector3.Transform(data.SensorConfiguration.LocalTranslation, Matrix.CreateRotationY(MathHelper.ToRadians(data.SensorConfiguration.GlobalRotationDegrees.Y))),
				new Vector3(0.01F, 0.01F, 0.01F),
				m_PrimitiveBatch);

			m_PrimitiveBatch.DrawPolygons(Camera.ViewspaceMatrix);
		}
	}
}
