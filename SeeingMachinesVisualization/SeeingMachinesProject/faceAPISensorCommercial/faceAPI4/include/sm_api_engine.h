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

#ifndef SM_API_ENGINE_H
#define SM_API_ENGINE_H

/*! @file 
    Defines the functions for working with general tracking engines.

    Specific versions of engines have their own include file defining the
    functions specific to their operations. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @struct smEngineHandle
    @brief Passed to any API routines that use engines.
    @see
    - smEngineCreate()
    - @ref sm_api_handles_detail */
SM_API_DECLARE_HANDLE(smEngineHandle);

/*! @brief The types of engines that may be created.
    @see smEngineCreate() */
typedef enum smEngineType
{
#   ifndef INTERNAL_DEV
    SM_API_ENGINE_TYPE_HEAD_TRACKER_V2  =1, /*!< V2.0 head-tracker. Robust. Multiple engines of this type can be created at the same time. */
    SM_API_ENGINE_TYPE_HEAD_TRACKER_V3  =2, /*!< V3.0 head-tracker. Same as V2 but tracks multiple faces. */
    SM_API_ENGINE_LATEST_HEAD_TRACKER   =2, /*!< Set to the latest version head tracker. */
    SM_API_NUM_ENGINE_TYPES				=2
#   else 
    // SM internal use only
    SM_API_ENGINE_TYPE_HEAD_TRACKER_V2  =1, /*!< V2.0 head-tracker. Robust. Multiple engines of this type can be created at the same time. */
    SM_API_ENGINE_TYPE_HEAD_TRACKER_V3  =2, /*!< V3.0 head-tracker. Same as V2 but tracks multiple faces. */
    SM_API_ENGINE_LATEST_HEAD_TRACKER   =2, /*!< Set to the latest version head tracker. */
    SM_API_ENGINE_TYPE_FACE_SEARCHER_V1 =3, /*!< V1.0 face-searcher. */
    SM_API_ENGINE_LATEST_FACE_SEARCHER  =3, /*!< Set to the latest version face-searcher. */
    SM_API_ENGINE_GAZE_TRACKER_V1       =4, /*!< Gaze-tracking engine, requires gaze-tracking camera module. */
    SM_API_ENGINE_LATEST_GAZE_TRACKER   =4, /*!< Set to the latest version gaze-tracker. */
	SM_API_NUM_ENGINE_TYPES				=4
#   endif
} smEngineType;

/*! @brief Possible states for an API Engine. */
enum
{
    SM_API_ENGINE_STATE_TERMINATED      =0, /*!< The engine is not yet created or has been destroyed (no resources allocated) */
    SM_API_ENGINE_STATE_INVALID         =1, /*!< The engine is in an invalid state and cannot be used. */
    SM_API_ENGINE_STATE_IDLE            =2, /*!< The engine exists but processing is inactive (not tracking). Video will be shown on a VideoDisplay. */
    SM_API_ENGINE_STATE_RECORDING       =3, /*!< The engine is recording raw images from the camera to disk. */
    SM_API_ENGINE_STATE_SEARCHING       =4, /*!< The engine is running a high-level search algorithm to find targets. */
    SM_API_ENGINE_STATE_TRACKING        =5, /*!< The engine is tracking (updating) one or more targets frame-to-frame. */
    SM_API_ENGINE_STATE_RECOVERING      =6  /*!< The engine is trying to recover from a tracking failure. */
};

/*! @brief Creates a new engine using the first detected camera.
    
    Performs system checks and allocates resources for a new a tracking engine.  
    @param type Specifies the type of engine to create. Must be one of SM_API_ENGINE_TYPE_* 
    @param engine_handle Pointer to a smEngineHandle (cannot be a 0). Set to point to a new engine of the specified type.	
    @return @ref smReturnCode "SM_API_OK" if the engine was created successfully.
    @pre Must be called from client's main event loop thread.
    @post engine state == @ref SM_API_ENGINE_STATE_IDLE
    @note smCameraRegisterType() must be called first for any camera to be detected. */
SM_API(smReturnCode) smEngineCreate(smEngineType type, smEngineHandle *engine_handle);

/*! @brief Creates a new engine using a specific camera.
    
    Performs system checks and allocates resources for a new a tracking engine.
    @param type Specifies the type of engine to create. Must be one of SM_API_ENGINE_TYPE_* 
    @param camera_handle The camera that the engine will use.
    @param engine_handle Pointer to a smEngineHandle (cannot be a 0). Set to point to a new engine of the specified type.
    @return @ref smReturnCode "SM_API_OK" if the engine was created successfully.
	@pre Must be called from client's main event loop thread.
    @post engine state == @ref SM_API_ENGINE_STATE_IDLE
    @see smCameraCreate() */
SM_API(smReturnCode) smEngineCreateWithCamera(smEngineType type, smCameraHandle camera_handle, smEngineHandle *engine_handle);

/*! @brief Gets the camera being used by the engine. 

    If the camera was created automatically by the engine via smEngineCreate() 
    then do not call smCameraDestroy() for the camera.

    @param engine_handle A valid engine handle.
    @param camera_handle Pointer to a smCameraHandle (cannot be 0). Set to point to the camera the engine is using.
    @return @ref smReturnCode "SM_API_OK" if the camera handle was obtained successfully.
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smEngineGetCamera(smEngineHandle engine_handle, smCameraHandle *camera_handle);

/*! @brief Gets the type of an existing engine.

    @param engine_handle A valid engine handle.
    @param type Pointer to an existing smEngineHandle (cannot be 0), set to the type of the engine.
    @return @ref smReturnCode "SM_API_OK" if the type was set successfully. */
SM_API(smReturnCode) smEngineGetType(smEngineHandle engine_handle, smEngineType *type);

/*! @brief Get the current engine state.

    @param engine_handle A valid engine handle.
    @param state Pointer to an existing int. The value pointed to by this parameter is written to the state, one of SM_API_ENGINE_STATE_*
    @return @ref smReturnCode "SM_API_OK" if value was read ok.
    @note Never blocks. */
SM_API(smReturnCode) smEngineGetState(smEngineHandle engine_handle, int *state);

/*! @brief Start the engine tracking.

    If the engine is already tracking this will restart the tracking.	
    @param engine_handle A valid engine handle.
	@return @ref smReturnCode "SM_API_OK" if the engine was started successfully.
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE
	@post engine state > @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smEngineStart(smEngineHandle engine_handle);

/*! @brief Stops the engine.

    @param engine_handle A valid engine handle.
	@return @ref smReturnCode "SM_API_OK" if engine was stopped successfully.
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE
	@post engine state == @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smEngineStop(smEngineHandle engine_handle);

/*! @brief Destroys the engine.

    If the engine was created via smEngineCreate(), the camera will also be destroyed.

    Terminates all processing threads and releases all internal resources
    @param engine_handle Pointer to an existing smEngineHandle.    
	@return @ref smReturnCode "SM_API_OK" if engine was destroyed successfully.
    @pre engine state > @ref SM_API_ENGINE_STATE_TERMINATED
	@post state == @ref SM_API_ENGINE_STATE_TERMINATED */
SM_API(smReturnCode) smEngineDestroy(smEngineHandle *engine_handle);

/*! @brief Checks if the engine is licensed.

    If the specific engine is not licensed it will still operate in a demo mode, but 
    no output data will be available via the callbacks or network logging methods.

    For the non-commercial license this function always returns "SM_API_OK".

    @param engine_handle A valid engine handle.
    @return @ref smReturnCode "SM_API_OK" if the license is ok, or "SM_API_FAIL_NO_LICENSE" if not. 
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smEngineIsLicensed(smEngineHandle engine_handle);

/*! @brief Determines if a control panel for the camera the engine is using exists.

    @param engine_handle A valid engine handle. 
    @param result Pointer to an existing smBool.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded. */
SM_API(smReturnCode) smEngineHasCameraControlPanel(smEngineHandle engine_handle, smBool *result);

/*! @brief Shows a control panel for the camera the engine is using, or does nothing if one does not exist.

    @param engine_handle A valid engine handle. 
    @return @ref smReturnCode "SM_API_OK" if the function executed without error (even if no control panel was shown).
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smEngineShowCameraControlPanel(smEngineHandle engine_handle);

/*! @brief Blocks the calling thread until the next measurement is available, or timeout.

    @param engine A valid engine handle.
    @param engine_data Must point to an existing smEngineData struct. 
           The contents of @a engine_data depend on the type of engine and it's state, and the license details. 
           The contents contain memory allocated by the engine. Do not modify the contents or attempt to free this memory.
           When you are finished with the engine data, you must call smEngineDataDestroy() to avoid a memory leak.
    @param timeout_ms Maximum time to block the caller in milliseconds. 
           If @a timeout_ms <= 0 the function does not block and will immediately return the most recent measurement,
           and therefore repeated calls with @a timeout_ms <= 0 will usually return the same measurement.
           A recommended value is 1000 (1 second).
    @return 
        - @ref smReturnCode "SM_API_OK" if @a head_pose was set with data
        - @ref smReturnCode "SM_API_FAIL_TIMEOUT" if no new data arrived within @a timeout_ms, and @a head_pose was not set.
        - @ref smReturnCode "SM_API_FAIL_NODATA" if no data was available - this can occur when the engine has not yet processed the first frame of video.
        - Other values indicate fatal errors.
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE 
    @see
    - smEngineDataDestroy() */
SM_API(smReturnCode) smEngineDataWaitNext(smEngineHandle engine, smEngineData *engine_data, int timeout_ms);

/*! @brief Releases memory and resources used in the smEngineData provided by smEngineDataWaitNext().

    Call this function after copying the data you need to release memory and resources back to the engine.

    Failing to call this function the same number of times as you make successfull calls to 
    smEngineDataWaitNext() will result in a memory leak.

    @param engine_data smEngineData that was set using smEngineDataWaitNext 
    @return SM_API_OK if successful.
    @see
    - smEngineDataWaitNext() */
SM_API(smReturnCode) smEngineDataDestroy(smEngineData *engine_data);

#ifdef __cplusplus
}
#endif
#endif
