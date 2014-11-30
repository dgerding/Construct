using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Construct.UX.Views.Visualizations
{
	//	Generated with http://phrogz.net/css/distinct-colors.html
	class LegendColors
	{
		public static Color[] Colors =
		{
			Color.FromRgb(115, 0, 0),
			Color.FromRgb(255, 170, 19),
			Color.FromRgb(0, 136, 204),
			Color.FromRgb(140, 64, 255),
			Color.FromRgb(242, 65, 0),
			Color.FromRgb(25, 191, 0),
			Color.FromRgb(16, 48, 64),
			Color.FromRgb(115, 94, 86),
			Color.FromRgb(163, 217, 177),
			Color.FromRgb(229, 0, 122)
		};

		public static Color GetUnusedColor(IEnumerable<Color> usedColors)
		{
			return Colors.First((color) => !usedColors.Contains(color));
		}
	}
}
