using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

/*
 * Given a to-fix directory and a search directory, all C# project references within the to-fix directory will be updated to
 *	use relative paths to the same file in the search directory. This won't happen if a reference doesn't exist in the search
 *	directory. This is meant to fix broken/absolute references.
 *	
 * Got tired of opening Construct only to find that the references were set to absolute paths. Instead of fixing them all by
 *	hand, just run this. Commandline for debugging was set up so that the fix would be applied to my Construct location, just
 *	change the target/library-reference directories.
 * 
 */

namespace ReferenceFixUtility
{
	class Program
	{
		delegate void ProjectFoundHandler(String projectFile);

		static bool VerboseOutput = false;

		static void SearchForCSProj(String target, ProjectFoundHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler");

			if (PathExtensions.PathIsFile(target))
			{
				if (PathExtensions.GetExtension(target) != "csproj")
					throw new InvalidOperationException("Unable to specifically target a non-.csproj file.");

				handler(target);
				return;
			}

			foreach (String file in Directory.EnumerateFiles(target))
			{
				String extension = PathExtensions.GetExtension(file);
				if (extension == "csproj")
					handler(file);
			}

			foreach (String directory in Directory.EnumerateDirectories(target))
				SearchForCSProj(directory, handler);
		}

		static void ProcessCSProj(String projectFile)
		{
			Console.WriteLine("PROJECT {0} ", projectFile);

			CSProject project = new CSProject(projectFile);

			var references = project.References;
			foreach (LibraryReference reference in references)
			{
				//	No hint path, no resolution
				if (reference.HintPath == null)
				{
					if (VerboseOutput)
						Console.WriteLine("No hintpath info for reference {0}, cannot deduce relative path", reference.ReferenceName);
					continue;
				}

				LibraryReference resolvedReference = ReferenceResolver.ResolveCorrectReference(project.DotNetRuntimeVersion, reference, Path.GetDirectoryName(projectFile));
				if (resolvedReference == null)
				{
					if (VerboseOutput)
						Console.WriteLine("\tUnable to resolve {0}", reference.ReferenceName);
				}
				else
				{
					if (reference.HintPath == resolvedReference.HintPath)
					{
						if (VerboseOutput)
							Console.WriteLine("\tReference {0} was already well-resolved", reference.HintPath);
					}
					else
					{
						Console.WriteLine("\tResolved:\n\t\tOLD: {0} \n\t\tNEW: {1}", reference.HintPath, resolvedReference.HintPath);
						project.ReplaceReference(reference, resolvedReference);
					}
				}
			}

			project.SaveChanges();
		}

		static void Main(string[] args)
		{
			while (true)
			{
				Console.Write("Should versions match exactly? (The latest version would be used if a match could not be found.) [Y/N] ");

				ConsoleKey input = Console.ReadKey().Key;
				if (input == ConsoleKey.Y)
				{
					ReferenceResolver.IgnoreVersionMismatch = false;
					break;
				}

				if (input == ConsoleKey.N)
				{
					ReferenceResolver.IgnoreVersionMismatch = true;
					break;
				}
			}

			Console.WriteLine();

			List<String> targetDirectories = new List<string>();

			foreach (String arg in args)
			{
				String[] components = arg.Split(new char[] { ':' }, 2);
				switch (components[0])
				{
					case ("s"): ReferenceResolver.ReferenceSources.Add(components[1]); break;
					case ("t"): targetDirectories.Add(components[1]); break;
				}
			}

			targetDirectories.ForEach((path) => SearchForCSProj(path, ProcessCSProj));
			Console.WriteLine("Done.");
			Console.ReadLine();
		}
	}
}
