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
    SM_API_ENGINE_TYPE_HEAD_TRACKER_V1  =0, /*!< V1.0 head-tracker. Low CPU. Only 1 engine of this type can be created at a time. */
    SM_API_ENGINE_TYPE_HEAD_TRACKER_V2  =1, /*!< V2.0 head-tracker. Robust. Multiple engines of this type can be created at the same time. */
    SM_API_ENGINE_LATEST_HEAD_TRACKER   =1  /*!< Set to the latest version head tracker. */
} smEngineType;

/*! @brief Possible states for an API Engine. */
enum
{
    SM_API_ENGINE_STATE_TERMINATED      =0, /*!< The engine is not yet created or has been destroyed (no resources allocated) */
    SM_API_ENGINE_STATE_INVALID         =1, /*!< The engine is in an invalid state and cannot be used. */
    SM_API_ENGINE_STATE_IDLE            =2, /*!< The engine exists but processing is inactive (not tracking). Video will be shown on a VideoDisplay. */
    SM_API_ENGINE_STATE_RECORDING       =3, /*!< The engine is recording raw images from the camera to disk. */
    SM_API_ENGINE_STATE_HT_INITIALIZING =4, /*!< The head-tracking engine is trying to acquire a new face. */
    SM_API_ENGINE_STATE_HT_TRACKING     =5, /*!< The head-tracking engine is tracking a face. */
    SM_API_ENGINE_STATE_HT_SEARCHING    =6  /*!< The head-tracking engine is searching for an existing face. */
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

/*! @brief Set the enabled state of UDP network logging of engine output data.
    
    Logging can only be enabled if the license permits.
    @param engine_handle A valid engine handle.
    @param enabled Set to SM_API_TRUE to enable UDP logging, SM_API_FALSE to disable it.
    @return @ref smReturnCode "SM_API_OK" if the logging enabled state was changed successfully 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see
    - smEngineGetUDPLoggingEnabled()
    - smEngineSetUDPOutputAddress()
	- @ref sm_api_data_interpreting_eod */
SM_API(smReturnCode) smEngineSetUDPLoggingEnabled(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the enabled state of UDP network logging of engine output data.

    @param engine_handle A valid engine handle.
    @param enabled Set to SM_API_TRUE if enabled, SM_API_FALSE if disabled. Cannot be 0.
    @return @ref smReturnCode "SM_API_OK" if the logging enabled state was retrieved successfully.    
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see
    - smEngineGetUDPLoggingEnabled()
    - smEngineSetUDPOutputAddress()
	- @ref sm_api_data_interpreting_eod */
SM_API(smReturnCode) smEngineGetUDPLoggingEnabled(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Set the network address and port to which the engine will send engine output data as UDP packets.

    These packets can be received using the "Client Tools SDK" available for 
    download from http://www.seeingmachines.com/support.htm#client_tools
    The default address is "localhost" at port 2001.    
    @param engine_handle A valid engine handle.
    @param hostname Name of computer or IP address (eg: "192.168.0.1" or "localhost" etc).
    @param udp_port Port number to connect UDP output socket to.
    @return @ref smReturnCode "SM_API_OK" if address was set successfully.
    @return @ref smReturnCode "SM_API_FAIL_INVALID_ARGUMENT" @a sm_engine_handle is not an engine, @a hostname
        is not a string or @a udp_port is not in the range [1024..49151].

    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @note This call will block the calling thread while the hostname is being resolved. This
    can take a few seconds.
	
	@see
	- @ref sm_api_data_interpreting_eod */
SM_API(smReturnCode) smEngineSetUDPOutputAddress(smEngineHandle engine_handle, smStringHandle hostname, int udp_port);

/*! @brief Get the network address and port to which the engine will send engine output data as UDP packets.

    @param engine_handle A valid engine handle.
    @param hostname Existing smStringHandle (created using smStringCreate()).
    @param udp_port Pointer to an exsiting integer (cannot be 0). Set to the Port number.
    @return @ref smReturnCode "SM_API_OK" if address information was retrieved successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
	
	@see 
	- @ref sm_api_data_interpreting_eod */
SM_API(smReturnCode) smEngineGetUDPOutputAddress(smEngineHandle engine_handle, smStringHandle hostname, int *udp_port);

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
    @return @ref smReturnCode "SM_API_OK" if the function exectuted without error (even if no control panel was shown).
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smEngineShowCameraControlPanel(smEngineHandle engine_handle);

#ifdef __cplusplus
}
#endif
#endif
