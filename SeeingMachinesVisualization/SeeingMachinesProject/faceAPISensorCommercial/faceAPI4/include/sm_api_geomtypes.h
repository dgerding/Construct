/*
	Copyright (C) 2013 Seeing Machines Ltd. All rights reserved.

	This file is part of the FaceTrackingAPI, also referred to as faceAPI.

	This file may be distributed under the terms of the Seeing Machines 
	FaceTrackingAPI Development License Agreement.

	Licensees holding a valid License Agreement may use this file in
	accordance with the rights, responsibilities and obligations
	contained therein. Please consult your licensing agreement or
	contact info@seeingmachines.com if any conditions of this licensing
	agreement are not clear to you.

	This file is provided AS IS with NO WARRANTY OF ANY KIND, INCLUDING THE
	WARRANTY OF DESIGN, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

	Further information about faceAPI licensing is available at:
	http://www.seeingmachines.com/faceapi/licensing.html
*/

#if !defined(SWIG) && !defined(SM_API_H)
#error Do not include this file. Include sm_api.h instead.
#endif

#ifndef SM_API_GEOMTYPES_H
#define SM_API_GEOMTYPES_H


/*! @file
    Defines types for basic geometry. All units are S.I. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Defines a coordinate in 2 dimensions using single precision floating 
    point. */
typedef struct smCoord2f
{
    float x; /*!< Coordinate X value */
    float y; /*!< Coordinate Y value */
} smCoord2f;

/*! @brief Defines a coordinate in 3 dimensions using single precision floating 
    point. */
typedef struct smCoord3f
{
    float x; /*!< Coordinate X value */
    float y; /*!< Coordinate Y value */
    float z; /*!< Coordinate Z value */
} smCoord3f;

/*! @brief Defines a position in 2 dimensions using single precision floating 
    point. */
typedef smCoord2f smPos2f;

/*! @brief Defines a position in 3 dimensions using single precision floating 
    point. */
typedef smCoord3f smPos3f;

/*! @brief Defines a coordinate in 2 dimensions using double precision floating 
    point. */
typedef struct smCoord2d
{
    double x; /*!< Coordinate X value */
    double y; /*!< Coordinate Y value */
} smCoord2d;

/*! @brief Defines a coordinate in 3 dimensions using double precision floating 
    point. */
typedef struct smCoord3d
{
    double x; /*!< Coordinate X value */
    double y; /*!< Coordinate Y value */
    double z; /*!< Coordinate Z value */
} smCoord3d;

/*! @brief Defines a position in 2 dimensions using double precision floating 
    point. */
typedef smCoord2d smPos2d;

/*! @brief Defines a position in 3 dimensions using double precision floating 
    point. */
typedef smCoord3d smPos3d;

/*! @brief Defines a quaternion using double precision floating point. 

    @see http://en.wikipedia.org/wiki/Quaternions_and_spatial_rotation */
typedef struct smQuaternion
{
    double rw;
    double rx;
    double ry;
    double rz;
} smQuaternion;

/*! @brief Defines the transform between coordinate frames as a translation and
    rotation quaternion. */
typedef struct smTransform3d
{
    smPos3d t;      /*!< Translation component */
    smQuaternion q; /*!< Rotation component, a unit quaternion, |q| = 1 */
} smTransform3d;

/*! @brief Euler angle representation of direction of a ray. All values are in 
    radians.

    These can also be thought of as pitch (x) and yaw (y). */
typedef struct smRotEuler2
{
    float x_rads; /*!< Rotation angle around X-axis, in radians */
    float y_rads; /*!< Rotation angle around Y-axis, in radians */
} smRotEuler2;

/*! @brief Euler angle representation of orientation. All values are in radians.

    The euler angles are represented in the X-Y-Z convention.
    @see
    - http://en.wikipedia.org/wiki/Euler_angles */
typedef struct smRotEuler3
{
    float x_rads; /*!< Rotation angle around X-axis, in radians */
    float y_rads; /*!< Rotation angle around Y-axis, in radians */
    float z_rads; /*!< Rotation angle around Z-axis, in radians */
} smRotEuler3;

/*! @brief Defines a coordinate in 2 dimensions using integer values. */
typedef struct smCoord2i
{
    int x;  /*!< Coordinate X value */
    int y;  /*!< Coordinate Y value */
} smCoord2i;

/*! @brief Defines a size in 2 dimensions using integer values. */
typedef struct smSize2i
{
    int w;  /*!< Width */
    int h;  /*!< Height */
} smSize2i;

/*! @brief Defines a 3d vertex using single precision floating point. */
typedef smCoord3f smVertexf;

/*! @brief Defines a triangle by three indices into an array of vertices. */
typedef struct smTriangle
{
    int v1;  /*!< Index of vertex 1 */
    int v2;  /*!< Index of vertex 2 */
    int v3;  /*!< Index of vertex 3 */
} smTriangle;

/*! @brief Defines a 3d shape as a set of vertices and triangles. */
typedef struct smTriangleMesh
{
    smVertexf *vertices;
    int num_vertices;
    smTriangle *triangles;
    int num_triangles;
} smTriangleMesh;

#ifdef __cplusplus
}
#endif


#endif

