using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMVisualization.Visualization
{
	/// <summary>
	/// Simple method for rendering a group of colored primitive types. Not recommended for advanced drawing.
	/// </summary>
	public class PrimitiveBatch
	{
		GraphicsDevice m_Device;
		BasicEffect m_Effect = null;

		RasterizerState m_RasterizerState;
		public DepthStencilState DepthState = null;

		List<VertexPositionColor> m_TriangleVertices = new List<VertexPositionColor>();
		List<VertexPositionColor> m_LineVertices = new List<VertexPositionColor>();

		public Color ActiveColor;

		public static DepthStencilState NoDepthState = null;
		public static DepthStencilState DefaultDepthState = null;

		public enum ListType
		{
			TriangleStrip,
			LineStrip,
			Single
		}

		private ListType m_ActiveType = ListType.Single;
		public ListType ActiveType
		{
			get
			{
				return m_ActiveType;
			}

			set
			{
				m_ActiveType = value;
				m_PreviousCount = 0;
			}
		}

		//	Convenient parameter, depth state that is applied for the subsequent DrawPolygons call but reset after that single call.
		public DepthStencilState TemporaryDepthState = null;

		private bool m_UsesOrthographicProjection = true;
		public bool UsesOrthographicProjection
		{
			get
			{
				return m_UsesOrthographicProjection;
			}
			set
			{
				m_UsesOrthographicProjection = value;
				if (m_UsesOrthographicProjection)
				{
					m_Effect.Projection = Matrix.CreateOrthographicOffCenter(0.0F, SMVisualization.Instance.GraphicsDevice.Viewport.Width, SMVisualization.Instance.GraphicsDevice.Viewport.Height, 0.0F, 0.0F, 1.0F);

					DepthState = new DepthStencilState();
					DepthState.DepthBufferEnable = false;
					DepthState.DepthBufferWriteEnable = false;
				}
				else
				{
					DepthState = new DepthStencilState();
					m_Effect.Projection = Camera.ProjectionMatrix;
					DepthState.DepthBufferEnable = true;
					DepthState.DepthBufferWriteEnable = true;
				}
			}
		}

		//	Vertex buffer for strips
		Vector3[] m_PreviousVertex = new Vector3[2];
		int m_PreviousCount = 0;

		/// <summary>
		/// Creates a new primitive drawing batch that enables the rendering of individual primitive types.
		/// </summary>
		/// <param name="device">GraphicsDevice object to be used for displaying graphics.</param>
		public PrimitiveBatch(GraphicsDevice device)
		{
			m_RasterizerState = new RasterizerState();
			m_RasterizerState.CullMode = CullMode.None;
			m_RasterizerState.FillMode = FillMode.Solid;

			DepthState = new DepthStencilState();
			DepthState.DepthBufferEnable = true;
			DepthState.DepthBufferFunction = CompareFunction.LessEqual;
			DepthState.DepthBufferWriteEnable = true;

			if (NoDepthState == null)
			{
				NoDepthState = new DepthStencilState();
				NoDepthState.DepthBufferWriteEnable = false;
				NoDepthState.DepthBufferEnable = false;
			}

			if (DefaultDepthState == null)
			{
				DefaultDepthState = new DepthStencilState();
			}

			m_Device = device;
			
			m_Effect = new BasicEffect(m_Device);
			m_Effect.VertexColorEnabled = true;
			m_Effect.Texture = null;

			UsesOrthographicProjection = false;

			ActiveColor = Color.White;
		}

		/// <summary>
		/// Draws all polygons that have been added to the batch without any transformations.
		/// Clears the batch after drawing.
		/// </summary>
		public void DrawPolygons()
		{
			DrawPolygons(Matrix.Identity);
		}

		/// <summary>
		/// Draws all polygons that have been added to the batch with the transform defined
		/// by the given matrix. Clears the batch after drawing.
		/// </summary>
		/// <param name="transformMatrix">Matrix to transform by.</param>
		public void DrawPolygons(Matrix transformMatrix)
		{
			if (m_LineVertices.Count == 0 && m_TriangleVertices.Count == 0)
				return; // Nothing to draw

			m_Device.RasterizerState = m_RasterizerState;
			m_Device.DepthStencilState = TemporaryDepthState != null ? TemporaryDepthState : DepthState;

			RasterizerState previousRasterizer = m_Device.RasterizerState;
			DepthStencilState previousDepth = m_Device.DepthStencilState;

			m_Effect.View = transformMatrix;
			m_Effect.CurrentTechnique.Passes[0].Apply();

			if (m_TriangleVertices.Count > 0)
			{
				VertexPositionColor[] triangleBuffer = new VertexPositionColor[m_TriangleVertices.Count];

				int i = 0;
				foreach (VertexPositionColor v in m_TriangleVertices)
				{
					triangleBuffer[i++] = v;
				}

				m_Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, triangleBuffer, 0, triangleBuffer.Length / 3);

				m_TriangleVertices.Clear();
			}

			if (m_LineVertices.Count > 0)
			{
				VertexPositionColor[] lineBuffer = new VertexPositionColor[m_LineVertices.Count];

				int i = 0;
				foreach (VertexPositionColor v in m_LineVertices)
				{
					lineBuffer[i++] = v;
				}

				m_Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, lineBuffer, 0, lineBuffer.Length / 2);

				m_LineVertices.Clear();
			}

			m_Device.DepthStencilState = previousDepth;
			m_Device.RasterizerState = previousRasterizer;

			TemporaryDepthState = null;

			m_PreviousCount = 0;
		}

		public void AddVertex(Vector3 point)
		{
			if (ActiveType == ListType.Single)
				throw new Exception("AddVertex cannot be called when activeType is set to Single.");

			switch (ActiveType)
			{
				case (ListType.LineStrip):
					{
						if (m_PreviousCount >= 1)
						{
							AddLine(m_PreviousVertex[0], point);
						}

						++m_PreviousCount;
						m_PreviousVertex[0] = point;

						break;
					}

				case (ListType.TriangleStrip):
					{
						if (m_PreviousCount >= 2)
						{
							AddTriangle(m_PreviousVertex[1], m_PreviousVertex[0], point);
						}

						++m_PreviousCount;
						m_PreviousVertex[1] = m_PreviousVertex[0];
						m_PreviousVertex[0] = point;

						break;
					}
			}
		}

		/// <summary>
		/// Adds a line to the batch using the current active color.
		/// </summary>
		/// <param name="p1">Starting point of the line.</param>
		/// <param name="p2">Ending point of the line.</param>
		public void AddLine(Vector3 p1, Vector3 p2)
		{
			m_LineVertices.Add(new VertexPositionColor(p1, ActiveColor));
			m_LineVertices.Add(new VertexPositionColor(p2, ActiveColor));
		}

		/// <summary>
		/// Adds a rectangle to the batch using the current active color.
		/// </summary>
		/// <param name="rect">The rectangle to be drawn.</param>
		public void AddQuad(Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl)
		{
			AddTriangle(tl, tr, bl);
			AddTriangle(br, bl, tr);
		}

		/// <summary>
		/// Adds a triangle to the batch using the current active color. Beware of backface culling.
		/// </summary>
		/// <param name="p1">First point on the triangle.</param>
		/// <param name="p2">Second point on the triangle.</param>
		/// <param name="p3">Third point on the triangle.</param>
		public void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			m_TriangleVertices.Add(new VertexPositionColor(p1, ActiveColor));
			m_TriangleVertices.Add(new VertexPositionColor(p2, ActiveColor));
			m_TriangleVertices.Add(new VertexPositionColor(p3, ActiveColor));
		}
	}
}
