using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceFixUtility
{
	public static class PathExtensions
	{
		public static String ConvertToRelativePath(String path, String relativeBase)
		{
			/* http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path */

			if (relativeBase.Last() != Path.DirectorySeparatorChar)
				relativeBase += Path.DirectorySeparatorChar;

			Uri basePath, dependentPath;
			basePath = new Uri(relativeBase);
			dependentPath = new Uri(path);

			Uri relativeUri = basePath.MakeRelativeUri(dependentPath);
			String finalPath = Uri.UnescapeDataString(relativeUri.ToString());

			return finalPath.Replace('/', Path.DirectorySeparatorChar);
		}

		public static bool PathIsFile(String path)
		{
			FileInfo info = new FileInfo(path);
			return (info.Attributes & FileAttributes.Directory) == 0;
		}

		public static String GetExtension(String path)
		{
			if (path.IndexOf('.') == -1)
				return "";

			return path.Substring(path.LastIndexOf('.') + 1);
		}
	}
}
