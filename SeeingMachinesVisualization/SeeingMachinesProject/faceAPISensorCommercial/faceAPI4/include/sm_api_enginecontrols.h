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

#ifndef SM_API_ENGINECONTROLS_H
#define SM_API_ENGINECONTROLS_H

/*! @file
    Defines functions for controlling the behavior of all head-tracker types. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Get the engine realtime mode enabled state.

    By default realtime tracking is enabled.

    @param engine_handle The engine to change.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the realtime tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smFileTrack() for easy processing of movie files.
    - smEngineSetRealtimeTracking() */
SM_API(smReturnCode) smEngineGetRealtimeTracking(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Enable or disable realtime tracking for the engine.

    By default realtime tracking is enabled.

    In realtime mode, the engine provides a real-time guarantee, which means 
    frames passed into the engine from the camera will always be output at the same 
    rate. Therefore any display will always show live video. In this mode, processing
    is skipped for frames whenever the CPU cannot keep up with the framerate of the
    camera.
    
    This is especially important to understand in the situation when the engine is 
    first trying to find a face. In this state (SM_API_ENGINE_STATE_SEARCHING)
    the engine is using a fairly slow face-detection algorithm to try to locate a face. 
    If images are being pushed at more than a few Hz, they simply pass through unprocessed.

    When realtime tracking is disabled, the images would be processed, and the 
    engine throughput will be a variable rate depending on many factors.

    Disabling realtime tracking is useful for offline processing of files. If doing so,
    it is recommended to enable this setting before the engine is started.

    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of realtime tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see
    - smFileTrack() for easy processing of movie files. 
    - smEngineGetRealtimeTracking() */
SM_API(smReturnCode) smEngineSetRealtimeTracking(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the engine realtime mode enabled state.

    By default realtime tracking is enabled.

    @param engine_handle The engine to change.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the realtime tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smFileTrack() for easy processing of movie files.
    - smEngineSetRealtimeTracking() */
SM_API(smReturnCode) smEngineGetRealtimeTracking(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Enable or disable output data blocking mode for the engine.

    This mode is only relevant when realtime tracking is disabled. 
    Enabling this mode is only possible when realtime tracking is disabled.

    By default output data is not blocked meaning that the engine is free to 
    process frames regardless of the calling pattern to smEngineDataWaitNext().

    when this mode is enabled, the engine will be prevented from processing the 
    next frame (be blocked) until the result is read-out by a call to 
    smEngineDataWaitNext().

    It is recommended to enable this setting before the engine is started.

    @code
    smEngineSetRealtimeTracking(engine_handle, SM_API_FALSE);
    smEngineSetOutputDataBlockingMode(engine_handle, SM_API_TRUE);
    smEngineStart(engine_handle);
    @endcode

    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the mode.
	@return @ref smReturnCode "SM_API_OK" if the mode was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see
    - smFileTrack() for easy processing of movie files. 
    - smEngineSetRealtimeTracking() 
    - smEngineGetOutputDataBlockingMode() */
SM_API(smReturnCode) smEngineSetOutputDataBlockingMode(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the engine output data blocking mode.

    By default output data blocking mode is disabled.

    This mode has no effect if realtime tracking is enabled.

    @param engine_handle The engine to change.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the mode.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smEngineSetOutputDataBlockingMode() */
SM_API(smReturnCode) smEngineGetOutputDataBlockingMode(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Gets the automatic brightness control state.
    The default state is off (false).     	
    @param engine_handle The engine to query.
    @param on Pointer to a user-allocated integer. The value is set to SM_API_FALSE if the controller is off, 
    and SM_API_TRUE when it is on.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smEngineSetAutoBrightness() */
SM_API(smReturnCode) smEngineGetAutoBrightness(smEngineHandle engine_handle, smBool *on);

/*! @brief Sets the automatic brightness control state.

    The default state is OFF (false) for WDM cameras, and ON (true) for PointGrey cameras.

    Enabling causes the tracker to try to control the camera's exposure related
    parameters in an intelligent way so that the face-region of the image is not
    washed out or too dark, ignoring surrounding image information.

    The control algorithm behaves differently for different camera exposure modes,
    as described below:
    - SM_API_EXPOSURE_MODE_AUTO: the algorithm has no effect because the camera driver is in control.
    - SM_API_EXPOSURE_MODE_FLICKERLESS_50HZ: the algorithm prefers to adjust gain and keep shutter speed at a fixed value to avoid flicker.
    - SM_API_EXPOSURE_MODE_FLICKERLESS_60HZ: the algorithm prefers to adjust gain and keep shutter speed at a fixed value to avoid flicker.
    - SM_API_EXPOSURE_MODE_FLICKERLESS_SUNLIGHT: the algorithm prefers to keep gain to a minimum and adjust shutter to optimize dynamic range in the image.
    - SM_API_EXPOSURE_MODE_IR: the algorithm prefers to keep shutter speed to a preset value to ensure synchronization with IR illumination (option only valid with faceAPI IR Lighting Kit).
    	
    @param engine_handle The engine to query.
    @param on Set to SM_API_TRUE to turn disable the brightness controller, SM_API_FALSE to turn it on.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smCameraSetExposureMode()
    - smEngineGetAutoBrightness() */
SM_API(smReturnCode) smEngineSetAutoBrightness(smEngineHandle engine_handle, smBool on);

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

#ifdef __cplusplus
}
#endif
#endif

