using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Microsoft.Xna.Framework;

namespace FaceDataGenerator
{
	class GenerationProperty
	{
		public Vector3 HeadPosition { get; set; }
		public float HeadPositionJitter { get; set; }

		public Vector3 HeadOrientation { get; set; }
		public float HeadOrientationJitter { get; set; }

		public Vector3 VergencePosition { get; set; }
		public float VergencePositionJitter { get; set; }

		public float IOD { get; set; }

		[Description("Whether or not data generated for this sensor should be transformed by the transformation properties defined within the cluster.")]
		public bool AutoTransformData { get; set; }
	}
}
