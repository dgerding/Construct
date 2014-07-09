using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Values to be configured:
 *	Local and Global Rotations and Transformations
 * 
 * Required Data Samples:
 *	XZ-axis binding sample
 *	Vergence intersection sample
 *	
 ********** XZ Axis Binding
 *	A subject will sit in front of the sensor with their head perfectly level, such that the vector
 *		of their head orientation is parallel to the plane of the surface that they are on. Aka, no
 *		looking up or down relative to the floor. The difference between the vector reported by
 *		faceLAB and the virtual XZ plane is used as the local X rotation.
 *		
 *		TODO: Head rotation may not end up being around the X axis. The rotation is dependent on the
 *			Y (and maybe Z?) rotation as well
 *	
 ********** Vergence Intersection
 *	A subject will be asked to stare at various points that have been mapped to positions within
 *		the virtual environment while keeping their head still. Assuming deviation is within
 *		acceptable bounds, the normalized gaze vectors will be recorded for their relative angular
 *		values. As vergence data is gathered, the translation and rotation of the root point will
 *		be transformed until there is only a single approximate position. Subtracting
 *		the faceLAB-reported head position (transformed by the calculated rotation) from this point
 *		would yield the composite translation of the faceLAB sensor.
 *		
 * Warning: Using a data set that is approximately mirrored over the local YZ plane would result in two
 *		points that would meet the vergence and XZ axis requirements.
 * 
 */

namespace CalibrationTool
{
	class ConfigurationSolver
	{
	}
}
