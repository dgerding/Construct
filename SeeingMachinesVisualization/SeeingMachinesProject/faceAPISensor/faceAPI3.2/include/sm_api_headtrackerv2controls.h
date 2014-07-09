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
#ifndef SM_API_HEADTRACKERV2CONTROLS_H
#define SM_API_HEADTRACKERV2CONTROLS_H

/*! @file
    Defines functions for controlling the behaviour of the head-tracker V2.0. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Gets the tracking restart-timeout enabled state.

    When restart-timeout is enabled, the tracker will be automatically restarted
    when a the restart-timeout period has elapsed and no tracking has occured.\n
	
    @param engine_handle The engine to query.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the tracking timeout.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTV2SetRestartTimeoutEnabled()
    - smHTGetRestartTimeout()
    - smHTSetRestartTimeout() */
SM_API(smReturnCode) smHTV2GetRestartTimeoutEnabled(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Sets the tracking restart-timeout enabled state.
	
    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the tracking timeout.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTV2GetRestartTimeoutEnabled()
    - smHTGetRestartTimeout()
    - smHTSetRestartTimeout() */
SM_API(smReturnCode) smHTV2SetRestartTimeoutEnabled(smEngineHandle engine_handle, smBool enabled);

/*! @brief Gets the tracking restart on poor-tracking enabled state.
	
    @param engine_handle The engine to change.
    @param enabled Gets the enabled state of the poor-tracking detector.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTV2SetRestartPoorTrackingEnabled() 
    - smHTGetRestartThreshold() 
    - smHTSetRestartThreshold() */
SM_API(smReturnCode) smHTV2GetRestartPoorTrackingEnabled(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Sets the tracking restart on poor-tracking enabled state.

	@pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the poor-tracking detector.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @see 
    - smHTV2GetRestartPoorTrackingEnabled() 
    - smHTGetRestartThreshold() 
    - smHTSetRestartThreshold() */
SM_API(smReturnCode) smHTV2SetRestartPoorTrackingEnabled(smEngineHandle engine_handle, smBool enabled);

/*! @brief Sets the level of head-pose filtering.

    @param engine_handle The engine to change.
    @param level Set to a filter level:
    - 0 is for no filtering
    - 1 is for mild filtering (default)
    - 2 is for maximum filtering
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTV2GetHeadPoseFilterLevel() */
SM_API(smReturnCode) smHTV2SetHeadPoseFilterLevel(smEngineHandle engine_handle, int level);

/*! @brief Gets the level of head-pose filtering.

    @param engine_handle The engine to query.
    @param level Pointer to an existing integer, set to the filter level:
    - 0 is for no filtering
    - 1 is for mild filtering (default)
    - 2 is for maximum filtering
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTV2SetHeadPoseFilterLevel() */
SM_API(smReturnCode) smHTV2GetHeadPoseFilterLevel(smEngineHandle engine_handle, int *level);

/*! @brief Gets the maximum level of head-pose filtering.

    @param engine_handle The engine to query.
    @param max_level Pointer to an existing integer, set to the maximum filter level (currently 2).
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTV2SetHeadPoseFilterLevel()
    - smHTV2GetHeadPoseFilterLevel() */
SM_API(smReturnCode) smHTV2GetHeadPoseMaxFilterLevel(smEngineHandle engine_handle, int *max_level);

/*! @brief Enable or disable realtime tracking for the engine.

    By default realtime tracking is enabled.

    In realtime mode, the engine provides a real-time guarantee, which means 
    frames passed into the engine from the camera will always be output at the same 
    rate. Therefore any display will always show live video. In this mode, processing
    is skipped for frames whenever the CPU cannot keep up with the framerate of the
    camera.
    
    This is especially important to understand in the situation when the engine is 
    first trying to find a face. In this state (SM_API_ENGINE_STATE_HT_INITIALIZING)
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
    - smFileTrack() for easy processing of movie files. */
SM_API(smReturnCode) smHTV2SetRealtimeTracking(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the engine realtime mode enabled state.

    By default realtime tracking is enabled.

    @param engine_handle The engine to change.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the realtime tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetRealtimetracking() */
SM_API(smReturnCode) smHTV2GetRealtimeTracking(smEngineHandle engine_handle, smBool *enabled);

#ifdef __cplusplus
}
#endif
#endif

