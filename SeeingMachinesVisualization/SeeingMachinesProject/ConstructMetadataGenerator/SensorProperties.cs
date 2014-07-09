﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ConstructMetadataGenerator
{
	public partial class SensorPropertiesForm : Form
	{
		SensorDeclaration m_Source;
		SensorPropertiesAdapter m_DisplaySource;

		public SensorPropertiesForm(SensorDeclaration source)
		{
			m_Source = source;

			InitializeComponent();

			m_DisplaySource = new SensorPropertiesAdapter();
			m_DisplaySource.SyncFromDeclaration(m_Source);
			propertyGrid.SelectedObject = m_DisplaySource;
		}

		private void btnDone_Click(object sender, EventArgs e)
		{
			m_DisplaySource.SyncToDeclaration(m_Source);
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}

	class SensorPropertiesAdapter
	{
		[Category("Versioning"), Description(
			"Major version for this sensor."
			)]
		public int MajorVersion { get; set; }
		[Category("Versioning"), Description(
			"Minor version for this sensor."
			)]
		public int MinorVersion { get; set; }
		[Category("Versioning"), Description(
			"Revision of this sensor for the current version."
			)]
		public int RevisionVersion { get; set; }

		[Description("The GUID of the sensor type that is the parent to the current sensor. The base sensor type has a GUID of 5C11FBBD-9E36-4BEA-A8BE-06E225250EF8.")]
		public Guid ParentSensorTypeID { get; set; }

		[Description("The GUID for the sensor type being generated.")]
		public Guid SensorTypeID { get; set; }

		[Description("The GUID of the type of sensor host that will maintain this type of sensor.")]
		public Guid SensorHostTypeID { get; set; }

		public void SyncFromDeclaration(SensorDeclaration sensor)
		{
			MajorVersion = sensor.MajorVersion;
			MinorVersion = sensor.MinorVersion;
			RevisionVersion = sensor.RevisionVersion;

			ParentSensorTypeID = sensor.ParentSensorID;
			SensorTypeID = sensor.SensorID;
			SensorHostTypeID = sensor.SensorHostTypeID;
		}

		public void SyncToDeclaration(SensorDeclaration target)
		{
			target.MajorVersion = MajorVersion;
			target.MinorVersion = MinorVersion;
			target.RevisionVersion = RevisionVersion;

			target.ParentSensorID = ParentSensorTypeID;
			target.SensorID = SensorTypeID;
			target.SensorHostTypeID = SensorHostTypeID;
		}
	}
}
