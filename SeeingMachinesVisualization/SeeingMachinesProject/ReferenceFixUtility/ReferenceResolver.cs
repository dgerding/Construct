using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Cryptography;

namespace ReferenceFixUtility
{
	static class ReferenceResolver
	{
		static Dictionary<String, LibraryReference> m_Cache = new Dictionary<String, LibraryReference>();
		public static List<String> ReferenceSources = new List<string>();
		public static bool IgnoreVersionMismatch = false; // If true, a search for a matching assembly will occur. If a matching version could not be found, then the latest version is used. (All versioning should use Major.Minor.Build.Revision)

		static List<String> SearchForReferenceInSources(String request, List<String> source, List<String> resultCompilation = null)
		{
			if (resultCompilation == null)
				resultCompilation = new List<string>();

			source.ForEach((path) => SearchForReference(request, path, resultCompilation));

			return resultCompilation;
		}

		/* Search for reference "request" in the "target" directory */
		static List<String> SearchForReference(String request, String target, List<String> resultCompilation = null)
		{
			if (resultCompilation == null)
				resultCompilation = new List<string>();

			foreach (String file in Directory.GetFiles(target))
			{
				if (Path.GetFileName(file) == request)
				{
					//	There are some strange edge-cases with Path.GetFileName where the path
					//		returned may be C:\\SomeFolder\\\\SomeOtherFolder\\file.txt
					//	Replace all cases of multiple @'\' characters with single @'\'

					resultCompilation.Add(file.RemoveAdjacentDuplicates('\\'));
				}
			}

			foreach (String directory in Directory.GetDirectories(target))
				SearchForReference(request, directory, resultCompilation);

			return resultCompilation;
		}

		enum VersionCompareMode
		{
			Strong,
			Weak
		}
		
		static bool VersionIsLaterThan(String first, String second, VersionCompareMode compareMode)
		{
			if (first == null)
				return false;
			if (second == null)
				return true;

			first = first.Replace("v", "");
			second = second.Replace("v", "");

			Version a, b;
			//	Is a later than b
			a = new Version(first);
			b = new Version(second);
			
			if (a.Major < b.Major)
				return false;
			if (a.Major > b.Major)
				return true;
			
			if (a.Minor < b.Minor)
				return false;
			if (a.Minor > b.Minor)
				return true;

			if (compareMode == VersionCompareMode.Strong)
			{
				if (a.Build < b.Build)
					return false;
				if (a.Build > b.Build)
					return true;

				if (a.Revision < b.Revision)
					return false;
				if (a.Revision > b.Revision)
					return true;
			}
			
			//	They're the exact same
			return false;
		}

		static Assembly GetMatchingAssembly(Version dnetRequestedRuntimeVersion, List<String> assemblyList, LibraryReference reference)
		{
			Assembly latestAssembly = null;
			String latestVersion = null;
			foreach (String assemblyFile in assemblyList)
			{
				//	For some reason ReflectionOnlyLoadFrom is the only assembly-loading function
				//		that will *always* load the specified file; LoadFrom is supposed to
				//		be able to do the same thing, but instead it also checks for cached
				//		local version in C:\Windows. This isn't documented behavior but can be
				//		observed in practice.
				Assembly currentAssembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
				String currentVersion = currentAssembly.GetName().Version.ToString();

				//	Test .NET runtimes
				String dnetVersionString = currentAssembly.ImageRuntimeVersion;
				if (VersionIsLaterThan(dnetVersionString, dnetRequestedRuntimeVersion.ToString(), VersionCompareMode.Weak))
					continue;
				
				if (VersionIsLaterThan(currentVersion, latestVersion, VersionCompareMode.Strong))
				{
					latestAssembly = currentAssembly;
					latestVersion = currentVersion;
				}

				//	If there's a library with the exact same version, use it instead of any
				//		other library (even if there are later versions available)
				if (currentVersion == reference.Version)
					return currentAssembly;
			}

			if (IgnoreVersionMismatch || reference.Version == null)
				return latestAssembly;
			else
				return null;
		}

		static String CompileReferenceName(LibraryReference reference)
		{
			return String.Format("{0}-{1}-{2}", reference.ReferenceName, reference.PublicKeyToken, reference.Version);
		}

		public static LibraryReference ResolveCorrectReference(Version dnetVersion, LibraryReference request, String currentProjectLocation)
		{
			if (request == null)
				return null;

			if (Path.IsPathRooted(request.HintPath))
			{
				if (File.Exists(Path.Combine(currentProjectLocation, request.HintPath)))
					return request;
			}

			if (ReferenceSources.Count == 0)
				throw new ArgumentNullException("ReferenceSource");

			String compiledName = CompileReferenceName(request);

			if (m_Cache.ContainsKey(compiledName))
				return m_Cache[compiledName];

			List<String> searchResult = SearchForReferenceInSources(Path.GetFileName(request.HintPath), ReferenceSources);
			if (searchResult.Count == 0)
				return null;

			Assembly matchedAssembly = GetMatchingAssembly(dnetVersion, searchResult, request);
			if (matchedAssembly == null)
			{
				Console.WriteLine("\tWarning: Assemblies found for {0}, but no version (library version or .NET version) matched.", request.ReferenceName);

				Console.WriteLine("\t\tRequested:\n\t\t\t{0}", request.Version);
				Console.WriteLine("\t\tFound versions:");
				searchResult.ForEach((assemblyPath) =>
				{
					Assembly currentAssembly = Assembly.LoadFile(assemblyPath);
					Console.WriteLine("-\t\t\t" + currentAssembly.GetName().Version.ToString() + "\t(" + assemblyPath + ")");
				});
				return null;
			}

			LibraryReference newReference = new LibraryReference();

			String relativePath = PathExtensions.ConvertToRelativePath(matchedAssembly.Location, currentProjectLocation);
			newReference.HintPath = relativePath;

			byte[] publicKey = matchedAssembly.GetName().GetPublicKeyToken();
			newReference.PublicKeyToken = BitConverter.ToString(publicKey).Replace("-", "");
			newReference.Version = matchedAssembly.GetName().Version.ToString();
			newReference.ReferenceName = request.ReferenceName;


			m_Cache[compiledName] = newReference;

			return newReference;
		}
	}
}
