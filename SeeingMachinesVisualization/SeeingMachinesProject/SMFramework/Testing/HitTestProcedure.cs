using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * We do confidence values on a per-gaze basis, the gaze rays that have a non-zero confidence
 *	of intersection with a hitbox are taken into account for the total confidence for that area.
 * 
 * Hit-Test Ratio Full Implementation:
 *	How often that hitbox was hit in the last X frames. If you're actually looking at something,
 *		we're going to detect a hit on that hitbox over several frames. The angular difference
 *		between your gaze and your head orientation is also taken into account.
 *		
 * Projection and Jitter-to-Area
 *	The geometry being compared against is projected onto a plane perpendicular to the gaze ray.
 *		Whether or not the point is within the geometry determines if a confidence value can be
 *		calculated. Confidence is the ratio of standard deviation to the area of the projected
 *		geometry combined with the angular difference between gaze and head orientation.
 *		
 * Projection and Circular Ratio
 *	The geometry being compared against is projected onto a plane perpendicular to the gaze ray.
 *		A circle is circumscribed about the vergence point on the plane with a radius equal
 *		to the projected standard angular deviation. (Projection method of deviation is TBD.) The
 *		ratio of the portion of the circle that is within the geometry to the portion outside of
 *		the geometry is combined with a ratio between the standard deviation and the geometry's area
 *		and the angular difference between gaze and head orientation.
 * 
 */

namespace SMFramework.Testing
{
	class HitTestProcedure
	{
	}
}
