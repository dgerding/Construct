using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceFixUtility
{
	public static class StringExtensions
	{
		public static String RemoveAdjacentDuplicates(this String text, char character)
		{
			String result = "";
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != character)
					result += text[i];

				//	Only add if the previous character wasn't the same
				if (text[i] == character && (i > 0 && text[i-1] != character))
					result += text[i];
			}
			return result;
		}
	}
}
