using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReferenceFixUtility
{
	class CSProject
	{
		public CSProject(String sourceFile)
		{
			using (StreamReader fileReader = new StreamReader(sourceFile))
			{
				m_ProjectXML = XElement.Load(fileReader);
			}

			m_SourceFile = sourceFile;

			XNamespace ns = m_ProjectXML.GetDefaultNamespace();
			String dotNetVersion =
				m_ProjectXML
				.Element(ns + "PropertyGroup")
				.Element(ns + "TargetFrameworkVersion").Value;

			//	Version is stored as vX.x, 'v' can't be part of the version
			DotNetRuntimeVersion = new Version(dotNetVersion.Replace("v", ""));
		}

		public void SaveChanges()
		{
			m_ProjectXML.Save(m_SourceFile);
		}

		String m_SourceFile;
		XElement m_ProjectXML;

		public Version DotNetRuntimeVersion
		{
			get;
			private set;
		}

		public bool WasChanged
		{
			get;
			private set;
		}

		private void GenerateReferences()
		{
			m_References = new List<LibraryReference>();

			XNamespace ns = m_ProjectXML.GetDefaultNamespace();
			var nodeList = m_ProjectXML.Descendants(ns + "Reference");

			foreach (XElement node in nodeList)
			{
				String details = node.Attribute("Include").Value;

				String[] properties = details.Split(',');
				LibraryReference reference = new LibraryReference();

				foreach (String property in properties)
				{
					String trimmedProperty = property.Trim();
					String[] propertyPair = trimmedProperty.Split('=');

					if (propertyPair.Length == 1)
					{
						reference.ReferenceName = propertyPair[0];
						continue;
					}

					switch (propertyPair[0])
					{
						case ("Version"): reference.Version = propertyPair[1]; break;
						case ("PublicKeyToken"): reference.PublicKeyToken = propertyPair[1]; break;
						case ("processorArchitecture"): reference.ProcessorArchitecture = propertyPair[1]; break;
						case ("Culture"): reference.Culture = propertyPair[1]; break;
					}
				}

				/* Find the HintPath node */
				var hintPathSearch = node.Descendants(node.GetDefaultNamespace() + "HintPath");

				/* Native reference (i.e. System, Microsoft.CSharp, etc.) don't have HintPaths */
				if (hintPathSearch.Count() > 0)
					reference.HintPath = hintPathSearch.First().Value;

				m_References.Add(reference);
			}
		}

		List<LibraryReference> m_References = null;
		public List<LibraryReference> References
		{
			get
			{
				if (m_References == null)
					GenerateReferences();

				return m_References;
			}
		}

		public void ReplaceReference(LibraryReference oldReference, LibraryReference newReference)
		{
			WasChanged = true;

			XNamespace ns = m_ProjectXML.GetDefaultNamespace();

			List<XElement> matches = new List<XElement>();
			foreach (XElement element in m_ProjectXML.Descendants(ns + "Reference"))
			{
				var includeValue = element.Attribute("Include");
				var referenceString = oldReference.ToString();
				if (includeValue.Value == oldReference.ToString())
					matches.Add(element);
			}

			var elementSearchResults =
				from element in m_ProjectXML.Descendants(ns + "Reference")
				where (
					from attribute in element.Attributes()
					where (attribute.Name == "Include" && attribute.Value == oldReference.ToString())
					select attribute
					).Count() > 0
				select element;

			if (elementSearchResults.Count() != 1)
				throw new Exception("Invalid number of matching attributes found for given PublicKeyToken.");

			var referenceElement = elementSearchResults.First();
			referenceElement.Attribute("Include").Value = newReference.ToString();
			referenceElement.Descendants(ns + "HintPath").First().Value = newReference.HintPath;
		}
	}
}
