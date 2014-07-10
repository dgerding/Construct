using System;
using System.IO;
using System.Xml;
using MonoTouch.Foundation;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace RemoteSensor
{
	public static class AppData
	{
		private enum TagNames
		{
			Config,
			SensorID
		}
		
		private static readonly string configPath = Path.Combine( Settings.ConfigDirectory, "config.xml" );
		
		private static string sensorID = null;
		public static string SensorID
		{
			get
			{
				if ( sensorID == null )
				{
					sensorID = getSensorID();
				}
				return sensorID;
			}
		}
		
		private static string displayName;
		public static string DisplayName
		{
			get
			{
				if ( displayName == null )
				{
					displayName = getDisplayName();
				}
				return displayName;
			}
		}
		
		private static string createDefaultAppConfigXml()
		{
            XmlTextWriter writer = XmlAPI.CreateWriter();

            writer.WriteStartElement(TagNames.Config.ToString());
            writer.WriteStartElement(TagNames.SensorID.ToString());

            writer.WriteCData( Guid.NewGuid().ToString() );
		
			writer.WriteEndElement();
            writer.WriteEndElement();
            
			return XmlAPI.FlushWriter(writer);
		}
		
		
		private static string getXml()
		{
			if ( File.Exists( configPath ) == false )
			{
				string xml = createDefaultAppConfigXml();
				File.WriteAllText( configPath, xml );
				return xml;
			}
			return File.ReadAllText( configPath );
		}
		
		private static string getSensorID()
		{
            try
            {
                string xml = getXml();
				XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                XmlElement rootElement = document.SelectSingleNode(TagNames.Config.ToString()) as XmlElement;
                XmlElement sensorIdElement = rootElement.SelectSingleNode(TagNames.SensorID.ToString()) as XmlElement;
				string id = sensorIdElement.InnerText;
				return id;
            }
            catch
            {
                return null;
            }
		}
		
		private static string getDisplayName()
		{
			string name = UIDevice.CurrentDevice.Name;
			if ( string.IsNullOrEmpty(name) == false )
			{
				name += " ";
			}
			name += string.Format("({0} running iOS {1})", UIDevice.CurrentDevice.Model, UIDevice.CurrentDevice.SystemVersion);
			return name;
		}
	
	}
}

