/*
	Copyright (C) 2008 Seeing Machines Ltd. All rights reserved.

	This file is part of the FaceTrackingAPI, also referred to as "faceAPI".

	This file may be distributed under the terms of the Seeing Machines 
	FaceTrackingAPI Non-Commercial License Agreement.

	This file is provided AS IS with NO WARRANTY OF ANY KIND, INCLUDING THE
	WARRANTY OF DESIGN, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

	Further information about faceAPI licensing is available at:
	http://www.seeingmachines.com/faceapi/licensing.html
*/
#ifndef SM_API_AR_H
#define SM_API_AR_H

/*! @file
    Defines utility functions for augmented reality applications. All functions in this file are reentrant. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Obtains perspective projection and model-view matrices to match the lens-parameters 
    so that a vertex in faceAPI world-coordinates can be rendered to match the underyling image data
    from a camera with such lens-parameters.

    This function is designed for operation with OpenGL - it provides projection and 
    model-view matrices that can be used to directly set GL_MODELVIEW and GL_PROJECTION.

    A good guide:
    http://www.songho.ca/opengl/gl_transform.html

    Matrices are column-major order.

    It is suggested that any image data be rendered as a texture on the far image plane.

    @code
    // Render a teapot at faceAPI world coordinate (0,0,1) - this is 1m in front of the (physical) camera.

    // Get the lens-parameters the camera is using.
    smCameraLensParams lens_params;
    smCameraGetLensParams(camera_handle,&lens_params);

    // Get the image size the camera is producing
    smCameraVideoFormat video_format;
    smCameraGetCurrentFormat(camera_handle,&video_format);

    // Get matrices for rendering faceAPI world-coordinates
    float model_view_matrix[16];
    float projection_matrix[16];
    smARGetWorldCoordMatricesFromLensParams(lens_params,video_format.res,&model_view_matrix[0],&projection_matrix[0]);

    // Set perspective viewing frustum
    glMatrixMode(GL_PROJECTION);
    glLoadMatrix(&projection_matrix[0]);

    // Set modelview matrix in order to set scene
    glMatrixMode(GL_MODELVIEW);
    glLoadMatrix(&model_view_matrix[0]);

    // Transform a teapot to be 1m in front of the camera
    glPushMatrix();
    glTranslatef(0.0f,0.0f,1.0f);
    drawTeapot();
    glPopMatrix();

    @endcode

    @see
    - sm_api_coord_frames_standard_world
*/

SM_API(smReturnCode) smARGetWorldCoordMatricesFromLensParams(
    smCameraLensParams lens_params, 
    smSize2i image_size,
    float near_z, 
    float far_z, 
    float *model_view_matrix,
    float *projection_matrix);

/*! @brief Obtain 4x4 transform matrix from world-coordinates to face-coordinates.

    Matrix is in column-major order.

    @code
    // Render a teapot at faceAPI face coordinate (0,0,0) - this the midpoint between your eyes.

    // Get the lens-parameters the camera is using.
    smCameraLensParams lens_params;
    smCameraGetLensParams(camera_handle,&lens_params);

    // Get the image size the camera is producing
    smCameraVideoFormat video_format;
    smCameraGetCurrentFormat(camera_handle,&video_format);

    // Get matrices for rendering faceAPI world-coordinates
    float model_view_matrix[16];
    float projection_matrix[16];
    smARGetWorldCoordMatricesFromLensParams(lens_params,video_format.res,&model_view_matrix[0],&projection_matrix[0]);

    // Set perspective viewing frustum
    glMatrixMode(GL_PROJECTION);
    glLoadMatrixf(&projection_matrix[0]);

    // Set modelview matrix in order to set scene
    glMatrixMode(GL_MODELVIEW);
    glLoadMatrixf(&model_view_matrix[0]);

    // Transform the teapot so it is stuck onto your face.
    glPushMatrix();
    float face_to_world[16];
    smARGetFaceToWorldTransformMatrix(current_head_pose, &face_to_world[0]);
    glMultMatrixf(&face_to_world[0]);
    drawTeapot();
    glPopMatrix();

    @endcode

    @see
    - sm_api_coord_frames_standard_world
*/
SM_API(smReturnCode) smARGetFaceToWorldTransformMatrix(smEngineHeadPoseData head_pose, float *f2w_matrix);

#ifdef __cplusplus
}
#endif
#endif



