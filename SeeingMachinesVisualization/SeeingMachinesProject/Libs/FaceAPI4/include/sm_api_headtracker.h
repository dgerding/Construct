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

#ifndef SM_API_HEADTRACKER_H
#define SM_API_HEADTRACKER_H

/*! @file
    Defines types and routines in the API relating to tracking head-pose. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Obtains estimate of the head-pose that may be interpolated from the time of call.

    If head-pose prediction is enabled, this function will predict ahead by an additional time
    offset to take into account exposure and processing time. @see smHTGetHeadPosePredictionEnabled().

    Otherwise it immediately returns the current head-pose measurement.

    @param engine A valid engine type.
    @param head_pose Must point to an existing smEngineHeadPoseData struct. The contents are set to the current estimate of the head-pose.
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smHTCurrentHeadPose(smEngineHandle engine, smEngineHeadPoseData *head_pose);

/*! @brief Signature of function for obtaining the head-pose solution
    
    @deprecated Use smEngineDataWaitNext() and smEngineDataDestroy()

    Define your own function matching this interface and register it with the engine to receive head-pose information.
    @param user_data Value passed into @a user_data argument in smHTRegisterHeadPoseCallback(). 
    @param head_pose The 3D head-pose solution.
    @param video_frame The video-frame that the head-pose relates to - includes source image, frame-number and UTC time.
    @see
    - smHTRegisterHeadPoseCallback() */
typedef void (STDCALL *smHTHeadPoseCallback)(void *user_data, smEngineHeadPoseData head_pose, smCameraVideoFrame video_frame);

/*! @brief Registers a callback function for receiving smEngineHeadPoseData data from an smEngineHandle.
    
    @deprecated Use smEngineDataWaitNext() and smEngineDataDestroy()

    Use this callback mechanism to obtain measurements at the same rate as the camera.\n
    Use smHTCurrentHeadPose() to obtain measurements that are interpolated use the time of call.\n

    Only one callback of this type can be registered with a particular smEngineHandle at any given time.

    @param engine_handle The engine to register the callback with.
    @param user_data Can be 0. This value is passed back to your callback routine. 
    It is typically used to pass the 'this' pointer of an object to enable the callback 
    to call a member function in the object.
	@param callback_fn Address of callback function, or 0 to unregister the callback.
	@return @ref smReturnCode "SM_API_OK" if callback was registered successfully. */
SM_API_DEPRECATED("Callbacks are being phased out. See smEngineDataWaitNext and smEngineDataDestroy") 
SM_API(smReturnCode) smHTRegisterHeadPoseCallback(smEngineHandle engine_handle, void *user_data, smHTHeadPoseCallback callback_fn);

/*! @brief Signature of function for obtaining smEngineFaceData from a head-tracker

    @deprecated Use smEngineDataWaitNext() and smEngineDataDestroy()

    Define your own function matching this interface and register it with the engine. 
    to receive face texture and landmark information.    

    @param user_data Value passed into @a user_data argument in smHTRegisterFaceDataCallback(). 
    @param face_data Face landmark positions and face-texture information.
    @param video_frame The video-frame that the face-data relates to - includes source image, frame-number and UTC time.
    @see
    - smHTRegisterFaceDataCallback()
    
    @note Any pointers in @a face_data will point to memory allocated by the API that only exists 
          for the duration of the callback routine. Your callback routine must deep copy the 
          information you require from this data structure. */
typedef void (STDCALL *smHTFaceDataCallback)(void *user_data, smEngineFaceData face_data, smCameraVideoFrame video_frame);

/*! @brief Registers a callback function for receiving smEngineFaceData data from an smEngineHandle

    @deprecated Use smEngineDataWaitNext() and smEngineDataDestroy()

    Only one callback of this type can be registered with a particular smEngineHandle at any given time.

    @param engine_handle The engine to register the callback with.
    @param user_data Can be 0. This value is passed back to your callback routine. 
    It is typically used to pass the 'this' pointer of an object to enable the callback 
    to call a member function in the object.
	@param callback_fn Address of callback function, or 0 to unregister the callback.
	@return @ref smReturnCode "SM_API_OK" if callback was registered successfully. 
    @note
    - This call will succeed even for engine versions that do not produce smEngineFaceData. 
      In that case, the callback never gets called.
    - Any pointers in @a face_data will point to memory allocated by the API that only exists 
      for the duration of the callback routine. Your callback routine must deep copy the 
      information you require from this data structure. */
SM_API_DEPRECATED("Callbacks are being phased out. See smEngineDataWaitNext and smEngineDataDestroy")
SM_API(smReturnCode) smHTRegisterFaceDataCallback(smEngineHandle engine_handle, void *user_data, smHTFaceDataCallback callback_fn);

#ifdef __cplusplus
}
#endif
#endif
