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

#ifndef SM_API_ENGINEDATATYPES_H
#define SM_API_ENGINEDATATYPES_H

/*! @file
    Defines types for engine output data. All units are S.I. unless otherwise indicated */
#ifdef __cplusplus
extern "C"
{
#endif

enum
{
    SM_API_VIDEO_QUALITY_TOO_DARK           =0x01,  /*!< The image is too dark for good tracking */
    SM_API_VIDEO_QUALITY_TOO_BRIGHT         =0x02,  /*!< The image is too bright for good tracking */
    SM_API_VIDEO_QUALITY_TOO_SLOW           =0x04,  /*!< Framerate is too slow */
};

/*! @brief Defines quality information about video passed into the engine.
    @see
    - smEngineData */
typedef struct smEngineVideoQualityData
{
    float intensity;                /*!< Normalized mean intensity [0..1] for the areas of the image that the engine needs to use for tracking. */
    float frame_rate;               /*!< Frame-rate of the video being passed into the engine */
    unsigned short quality_flags;   /*!< See SM_API_VIDE_QUALITY_* for a list of possible flag values. */
} smEngineVideoQualityData;

/*! @brief Defines the 3D pose of the head.
    @see
    - smEngineData */
typedef struct smEngineHeadPoseData
{
    smPos3f head_pos;        /*!< Position of the head relative to the camera. */
    smPos3f left_eye_pos;    /*!< Position of the left eyeball center relative to the camera. */
    smPos3f right_eye_pos;   /*!< Position of the right eyeball center relative to the camera. */
    smRotEuler3 head_rot;    /*!< Rotation of the head around the X,Y and Z axes respectively in euler angles. @see http://en.wikipedia.org/wiki/Euler_angles */
    float confidence;        /*!< Confidence of the head-pose measurement [0..1] derived from correlation measurements. A value of 0 indicates that the measurements cannot be trusted and may have undefined values. A value of 1.0 indicates the image is perfectly correlated with the internal state representation of the face being tracked. */
    int quality;             /*!< Long term indicator of tracking ability for a specific face. A larger number indicates the tracker has more detailed information about the face and in the average case will track it better than a smaller value (for the same face). */
} smEngineHeadPoseData;

/*! @brief Indicates if a face is wearing glasses. */
typedef enum smWearingGlassesType
{
    SM_API_WEARING_GLASSES_NO = 0,      /*!< Face is not wearing glasses. */
    SM_API_WEARING_GLASSES_YES = 1,     /*!< Face is wearing glasses. */
    SM_API_WEARING_GLASSES_UNKNOWN = 2, /*!< It is unknown if the face is wearing glasses. */
} smWearingGlassesType;

/*! @brief Contains facial landmarks and texture.

    This data may be produced by face algorithms. The contents of the structure are dynamic,
    depending on how many face landmarks are able to be located and if a 
    facial texture image can be generated. 

    @see
    - sm_api_flm_standard
    - smHTFaceDataCallback */
typedef struct smEngineFaceData
{
    int num_landmarks;                      /*!< Number of landmarks being tracked */
    smFaceLandmark *landmarks;              /*!< Pointer to first element in an array of size @a num_landmarks * sizeof(smFaceLandmark). Will be NULL of no landmarks are available.  The memory pointed to is owned by the engine and must not be modified or freed. */
    smFaceTexture *texture;                 /*!< Pointer to texture for the face region. Will be NULL if no texture is available. The memory pointed to is owned by the engine and must not be modified or freed. */
    smTriangleMesh *shape;                  /*!< Pointer to 3d shape of the head. Will be NULL if no shape is available. The memory pointed to is owned by the engine and must not be modified or freed. */
    smCoord3f origin_wc;                    /*!< The origin of the head, in world coordinates. @see sm_api_coord_frames_standard */
    smPixel origin_pc;                      /*!< The origin of the head, in pixel coordinates. @note Only currently used by smFaceSearch(). @see sm_api_coord_frames_standard */
    smWearingGlassesType wearing_glasses;   /*!< Indicates if a face is wearing glasses */
} smEngineFaceData;

/*! @brief Constants for referring to left and right eyes. */
typedef enum smEyeId
{
    SM_API_RIGHT_EYE = 0,                   /*!< The right eye of the person (not the right eye in the image). */
    SM_API_LEFT_EYE = 1                     /*!< The left eye of the person (not the left eye in the image). */ 
} smEyeId;

typedef struct smEngineEyeClosureData
{
    float closure[2];                       /*!< Eyelid closure for right and left eyes. Value in the range [0..1]. Value 0 equals fully open, 1 equals fully closed. */
    float conf[2];                          /*!< Confidence of the right and left eye closure measurements. If the eye is not visible, will be 0. 1 means ideal conditions. */
    smBool blinking;                        /*!< True when eyelid motion pattern indicates a blink event is occuring. */
} smEngineEyeClosureData;

/*! @brief Indicates the characteristic of an eye movement. */
typedef enum smEyeballMotionType
{
    SM_API_EYE_MOTION_UNKNOWN = 0,          /*!< Gaze motion analysis is disabled or not possible. */
    SM_API_EYE_MOTION_FIXATION = 1,         /*!< Gaze direction is momentarily fixed in a certain dirtection. */
    SM_API_EYE_MOTION_SACAADE = 2,          /*!< Gaze direction is rapidly changing. */
    SM_API_EYE_MOTION_SMOOTH_PURSUIT = 3    /*!< Gaze direction is steadily moving as if following a moving target. */
} smEyeballMotionType;

/*! @brief Gaze direction is defined by a ray originating from the centre of the eyeball,
    with a direction determined by pitch and yaw rotation angles. */
typedef struct smEngineGazeData
{
    smPos3f eye_pos[2];                     /*!< Position of the right and left eyes in world-coordinates. More precise than the value in smEngineHeadPoseData. */
    smRotEuler2 gaze_rot[2];                /*!< Rotation of the gaze direction around the X and Y axes of the world-coordinate system, for right and left eyes. If both rotation values values are 0, the gaze is aligned with the world-coordinate Z-axis. */
    smBool calibrated;                      /*!< Whether gaze has been calibrated to remove systematic measurement bias. */
    smEyeballMotionType motion_type;        /*!< Result of any motion analysis of gaze direction. */
} smEngineGazeData;

typedef struct smEnginePupilData
{
    smPos3f pupil_pos[2];                   /*!< Position of the centre of the pupil in world-coordinates for the right and left eyes. */
    float diameter[2];                      /*!< Diameter of the pupil of the right and left eyes. */
} smEnginePupilData;

typedef struct smEngineEyeData
{
    smEngineEyeClosureData *closure_data;   /*!< Pointer to a single smEngineEyeClosureData. NULL if no eye-closure data is available. Memory pointed to is allocated by the engine. Do not modify or free this memory. */
    smEngineGazeData *gaze_data;            /*!< Pointer to a single smEngineGazeData. NULL if no gaze data is available. Memory pointed to is allocated by the engine. Do not modify or free this memory. */
    smEnginePupilData *pupil_data;          /*!< Pointer to a single smEnginePupilData. NULL if no pupil data is available. Memory pointed to is allocated by the engine. Do not modify or free this memory. */
} smEngineEyeData;

/*! @brief This data structure represents all that is known and measured from a single person for the frame of video. */
typedef struct smEnginePersonData
{
    smUUID tracking_uuid;                   /*!< Integer for relating this set of data to other smEnginePersonData. 
                                                 Two smEnginePersonData with the same tracking_uuid will usually relate to the same physical person.
                                                 @note However is not a biometric value: two smEnginePersonData with different tracking_uuid may relate to the same physical person. */
    smEngineHeadPoseData *head_pose_data;   /*!< NULL if no head-pose data exists for this measurement. The memory pointed to is owned by the engine and must not be modified or freed. */
    smEngineFaceData *face_data;            /*!< NULL if no face data exists for this measurement. The memory pointed to is owned by the engine and must not be modified or freed. */
    smEngineEyeData *eye_data;              /*!< NULL if no data exists for this measurement. The memory pointed to is owned by the engine and must not be modified or freed. */
} smEnginePersonData;

/*! @brief This data structure contains all the possible output data an engine can produce.
    
    Most data fields are optional, as they depend on the type of tracking engine and also the state of the tracking algorithm.

    @see
    - smEngineDataWaitNext 
    - smEngineDataDestroy */
typedef struct smEngineData
{
    smCameraVideoFrame video_frame;                 /*!< Frame of video passed into the engine by the camera, consisting of the frame-number, UTC time and image. */
    smEngineVideoQualityData *video_quality_data;   /*!< Optional information on video input quality. Some engines provide this. NULL when unused. */
    int num_people;                                 /*!< Number of faces being tracked (or attempting to track), one per smEnginePersonData below. */
    smEnginePersonData *people;                     /*!< Pointer to first element in an array of smEnginePersonData of size @a num_people * sizeof(smEnginePersonData). NULL if num_people = 0. */
    void *user_data;                                /*!< Any @a user_data supplied by the function smCameraImagePushBlockEx(), or 0 otherwise. */
    void *__handle;                                 /*!< Internal handle for engine memory. Do not modify! */
} smEngineData;

#ifdef __cplusplus
}
#endif


#endif

