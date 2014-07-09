using Microsoft.Xna.Framework;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace SMFramework
{
	public class FaceLabSignalConfiguration
	{
		public Vector3 GlobalTranslation
		{
			get;
			set;
		}

		public Vector3 GlobalRotationDegrees
		{
			get;
			set;
		}

		public Vector3 LocalTranslation
		{
			get;
			set;
		}

		public Vector3 LocalRotationDegrees
		{
			get;
			set;
		}

		public int Port
		{
			get;
			set;
		}

		[Browsable(false)]
		public String Label
		{
			get;
			set;
		}

        public override string ToString()
        {
            if (Label != null)
                return Label;
            else
                return base.ToString();
        }

		public Vector3 TransformAffine(Vector3 vector)
		{
			return Vector3.Transform(vector,
				Matrix.CreateRotationY(MathHelper.ToRadians(LocalRotationDegrees.Y)) *
				Matrix.CreateTranslation(LocalTranslation) *
				Matrix.CreateRotationY(MathHelper.ToRadians(GlobalRotationDegrees.Y)) *
				Matrix.CreateTranslation(GlobalTranslation)
				);
		}

		public Vector3 TransformLinear(Vector3 vector)
		{
			return Vector3.Transform(vector,
				Matrix.CreateRotationY(MathHelper.ToRadians(LocalRotationDegrees.Y + GlobalRotationDegrees.Y))
				);
		}
	}

	public class SensorClusterConfiguration
	{
		/* Vector3 doesn't have a serialization function for going from its ToString output to a regular Vector3
		 *	object; we define a conversion here.
		 */
		static SensorClusterConfiguration()
		{
			JsConfig<Vector3>.RawDeserializeFn = DeserializeVector3;
		}

		internal static Vector3 DeserializeVector3(String input)
		{
			Vector3 result = new Vector3();

			input = input.Substring(1, input.Length - 2);

			String[] components = input.Split(' ');

			result.X = float.Parse(components[0].Substring(2));
			result.Y = float.Parse(components[1].Substring(2));
			result.Z = float.Parse(components[2].Substring(2));

			return result;
		}

		public List<FaceLabSignalConfiguration> SensorConfigurations
		{
			get;
			private set;
		}

		public enum FileLoadResponse
		{
			GenerateIfMissing,
			DoNothingIfMissing,
			FailIfMissing
		}

		public SensorClusterConfiguration()
		{
			SensorConfigurations = new List<FaceLabSignalConfiguration>();
			GUID = Guid.NewGuid();
		}

		public SensorClusterConfiguration(String sourceFile, FileLoadResponse loadFailResponse)
		{
			if (!File.Exists(sourceFile))
			{
				switch (loadFailResponse)
				{
					case FileLoadResponse.GenerateIfMissing:
						SensorConfigurations = new List<FaceLabSignalConfiguration>();
						SaveToFile(sourceFile);
						break;

					case FileLoadResponse.DoNothingIfMissing:
						SensorConfigurations = new List<FaceLabSignalConfiguration>();
						break;

					case FileLoadResponse.FailIfMissing:
						throw new Exception("Unable to locate file " + sourceFile);
				}
			}
			else
			{
				LoadFromFile(sourceFile);
			}
		}

		public FaceLabSignalConfiguration FindConfigurationForLabel(String label)
		{
			foreach (FaceLabSignalConfiguration config in SensorConfigurations)
			{
				if (config.Label == label)
					return config;
			}

			return null;
		}

		public Guid GUID
		{
			get;
			private set;
		}

		public void LoadFromFile(String sourceFile)
		{
			try
			{
				using (StreamReader reader = new StreamReader(sourceFile))
				{
					SensorClusterConfiguration intermediateConfiguration =
						JsonSerializer.DeserializeFromReader<SensorClusterConfiguration>(reader);

					SensorConfigurations = intermediateConfiguration.SensorConfigurations;
					GUID = intermediateConfiguration.GUID;
				}
			}
			catch (System.Exception ex)
			{
				DebugOutputStream.SlowInstance.WriteLine("Unable to load lab configuration from " + sourceFile + "\n" + ex.Message);

				SensorConfigurations = null;
			}
		}

		public void SaveToFile(String targetfile)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(targetfile))
				{
					JsonSerializer.SerializeToWriter<SensorClusterConfiguration>(this, writer);
				}
			}
			catch (System.Exception ex)
			{
				DebugOutputStream.SlowInstance.WriteLine("Unable to save lab configuration to " + targetfile + "\n" + ex.Message);
			}
		}
	}
}
