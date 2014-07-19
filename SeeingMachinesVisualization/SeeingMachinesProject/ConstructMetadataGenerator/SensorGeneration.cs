using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace ConstructMetadataGenerator
{
	public class SensorDeclaration
	{
		public int Version = 1000;

		public String SensorName;
		public Guid ParentSensorID = new Guid("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8");
		public Guid SensorID = Guid.NewGuid(); // You're free to change the GUID
		public Guid SensorHostTypeID = new Guid("EDA0FF3E-108B-45D5-BF58-F362FABF2EFE"); // Windows sensor host GUID
		public List<OutputDatatypeDeclaration> DataTypes = new List<OutputDatatypeDeclaration>();

		public static XmlDocument GenerateDocumentFromSensorDeclaration(SensorDeclaration sensor)
		{
			XmlDocument result = new XmlDocument();

			XmlDeclaration encodingDeclaration = result.CreateXmlDeclaration("1.0", "utf-16", null);
			result.AppendChild(encodingDeclaration);

			/* SensorTypeSource
			 *   ----> DataType
			 *       ---->DataTypeProperty
			 * 
			 */

			XmlNode sensorTypeSource = result.CreateElement("SensorTypeSource");
			var sensorNameAttribute = result.CreateAttribute("Name");
			var sensorIdAttribute = result.CreateAttribute("ID");
			var versionAttribute = result.CreateAttribute("Version");
			var parentIdAttribute = result.CreateAttribute("ParentID");
			var sensorHostTypeIdAttribute = result.CreateAttribute("SensorHostTypeID");

			sensorNameAttribute.Value = sensor.SensorName;
			sensorIdAttribute.Value = sensor.SensorID.ToString();

			versionAttribute.Value = sensor.Version.ToString();
			parentIdAttribute.Value = sensor.ParentSensorID.ToString();
			sensorHostTypeIdAttribute.Value = sensor.SensorHostTypeID.ToString();

			sensorTypeSource.Attributes.Append(sensorNameAttribute);
			sensorTypeSource.Attributes.Append(sensorIdAttribute);
			sensorTypeSource.Attributes.Append(versionAttribute);
			sensorTypeSource.Attributes.Append(parentIdAttribute);
			sensorTypeSource.Attributes.Append(sensorHostTypeIdAttribute);

			result.AppendChild(sensorTypeSource);

			//	ParentID/SensorHostTypeID/Version attributes?

			foreach (OutputDatatypeDeclaration outputDeclaration in sensor.DataTypes)
			{
				XmlNode typeNode = result.CreateElement("DataType");

				XmlAttribute typeName, typeID;
				typeName = result.CreateAttribute("Name");
				typeID = result.CreateAttribute("ID");

				typeName.Value = outputDeclaration.TypeName;
				typeID.Value = outputDeclaration.DataTypeID.ToString();

				typeNode.Attributes.Append(typeName);
				typeNode.Attributes.Append(typeID);

				foreach (PropertyInfo publicField in outputDeclaration.OutputType.GetProperties())
				{
					XmlNode fieldNode = result.CreateElement("DataTypeProperty");

					XmlAttribute fieldName, fieldId, fieldType;
					fieldName = result.CreateAttribute("Name");
					fieldId = result.CreateAttribute("ID");
					fieldType = result.CreateAttribute("Type");

					fieldName.Value = publicField.Name;
					fieldId.Value = Guid.NewGuid().ToString();
					fieldType.Value = publicField.PropertyType.FullName;

					fieldNode.Attributes.Append(fieldName);
					fieldNode.Attributes.Append(fieldId);
					fieldNode.Attributes.Append(fieldType);

					typeNode.AppendChild(fieldNode);
				}

				sensorTypeSource.AppendChild(typeNode);
			}

			return result;
		}
	}

	/* Types are generated from all public fields and properties */
	public class OutputDatatypeDeclaration
	{
		public Type OutputType;
		public Guid DataTypeID = Guid.NewGuid(); // You're free to change the GUID

		public String SourceAssembly;

		private String m_ManualTypeName = null;
		public String TypeName
		{
			get
			{
				if (m_ManualTypeName == null)
					return OutputType.Name;
				else
					return m_ManualTypeName;
			}

			set
			{
				m_ManualTypeName = value;
			}
		}

		public bool UsesAutoTypename
		{
			get { return m_ManualTypeName == null; }
		}

		public MemberDeclaration[] TypeMembers
		{
			get
			{
				List<MemberDeclaration> members = new List<MemberDeclaration>();

				foreach (PropertyInfo property in OutputType.GetProperties())
					members.Add(new MemberDeclaration(property.Name, property.PropertyType));

				return members.ToArray();
			}
		}
	}

	public class MemberDeclaration
	{
		public MemberDeclaration()
		{}

		public MemberDeclaration(String memberName, Type memberType)
		{
			MemberName = memberName;
			MemberType = memberType;
		}

		public String MemberName;
		public Type MemberType;
	}
}
