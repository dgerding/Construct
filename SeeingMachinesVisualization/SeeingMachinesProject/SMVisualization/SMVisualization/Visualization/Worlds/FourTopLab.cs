using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMFramework;

namespace SMVisualization.Visualization.Worlds
{
	public class FourTopLab : WorldRenderer
	{
		/* (from the perspective of sitting at signal 1) */
		float m_TableWidth = 1.215f * Units.Meter;
		float m_TableLength = 1.215f * Units.Meter;
		float m_TableHeight = 0.73f * Units.Meter;

		private PrimitiveBatch m_PrimitiveBatch;

		public List<Vector3> TargetPoints = new List<Vector3>();

		public FourTopLab()
		{
			m_PrimitiveBatch = new PrimitiveBatch(SMVisualization.Device);
		}

		public void Draw()
		{
			//	Floor
			m_PrimitiveBatch.ActiveColor = Color.DarkRed;
			m_PrimitiveBatch.AddQuad(
				new Vector3(-100.0f, -0.01f, 100.0f) * Units.Meter,
				new Vector3(100.0f, -0.01f, 100.0f) * Units.Meter,
				new Vector3(100.0f, -0.01f, -100.0f) * Units.Meter,
				new Vector3(-100.0f, -0.01f, -100.0f) * Units.Meter
				);

			m_PrimitiveBatch.DrawPolygons(Camera.ViewspaceMatrix);

			DrawWalls();
			DrawTable();
		}

		private void DrawTable()
		{
			m_PrimitiveBatch.ActiveColor = Color.Green * 0.5f;
			m_PrimitiveBatch.TemporaryDepthState = PrimitiveBatch.NoDepthState;

			ShapeHelper.DrawBox(
				new Vector3(0.0f, m_TableHeight / 2.0f, 0.0f),
				new Vector3(m_TableWidth, m_TableHeight, m_TableLength),
				m_PrimitiveBatch
				);

			m_PrimitiveBatch.DrawPolygons(Camera.ViewspaceMatrix);
		}

		public void DrawWalls()
		{
			float wallThickness = 0.075F * Units.Meter;
			float wallWidth = 3.0F * Units.Meter;
			float wallHeight = 1.87F * Units.Meter;

			m_PrimitiveBatch.ActiveColor = Color.Black * 0.5F;

			//m_PrimitiveBatch.temporaryDepthState = PrimitiveBatch.NoDepthState;

			//	Front wall
			ShapeHelper.DrawBox(
				new Vector3(0.0F, wallHeight / 2.0F, 1.4725F + wallThickness / 2.0F),
				new Vector3(3.18F * Units.Meter, wallHeight, wallThickness),
				m_PrimitiveBatch
				);

			//	Right wall
			ShapeHelper.DrawBox(
				new Vector3(1.5275F * Units.Meter + wallThickness / 2.0F, wallHeight / 2.0F, 0.0F),
				new Vector3(wallThickness, wallHeight, wallWidth),
				m_PrimitiveBatch
				);

			//	Back wall
			ShapeHelper.DrawBox(
				new Vector3(0.0F, wallHeight / 2.0F, -1.5775F - wallThickness / 2.0F),
				new Vector3(3.18F * Units.Meter, wallHeight, wallThickness),
				m_PrimitiveBatch
				);

			//	Left wall
			ShapeHelper.DrawBox(
				new Vector3(-1.5775F - wallThickness / 2.0F, wallHeight / 2.0F, -0.5F * Units.Meter),
				new Vector3(wallThickness, wallHeight, 2.03F * Units.Meter),
				m_PrimitiveBatch
				);


			m_PrimitiveBatch.ActiveColor = Color.White;

			foreach (Vector3 target in TargetPoints)
			{
				ShapeHelper.DrawEllipse(target, Vector3.One * 0.01F, m_PrimitiveBatch);
			}


			m_PrimitiveBatch.DrawPolygons(Camera.ViewspaceMatrix);
		}
	}
}
