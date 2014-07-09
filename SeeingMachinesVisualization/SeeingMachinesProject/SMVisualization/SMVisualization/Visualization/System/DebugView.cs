using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMVisualization.Visualization
{
	public class DebugView
	{
		static SpriteFont m_Font;
		static SpriteBatch m_SpriteBatch;
		static String m_Text = "";

		public static bool Enabled = true;

		public static void LoadContent()
		{
			m_Font = SMVisualization.Instance.Content.Load<SpriteFont>("DebugFont");
			m_SpriteBatch = new SpriteBatch(SMVisualization.Device);
		}

		public static void AddText(String text)
		{
			m_Text += "\n" + text;
		}

		public static void Draw()
		{
			m_SpriteBatch.Begin();
			if (Enabled)
			{
				m_SpriteBatch.DrawString(m_Font, "--- DEBUG ---\n" + m_Text, new Vector2(10, 10), Color.White);
			}
			m_SpriteBatch.End();
			m_Text = "";
		}
	}
}
