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

#ifndef SM_API_HEADTRACKERV2CONTROLS_H
#define SM_API_HEADTRACKERV2CONTROLS_H

/*! @file
    Defines functions for controlling the behavior of the head-tracker V2.0. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Gets the automatic tracking restart mode.

    The default restart mode is manual. This means that the 
    head-tracker will not be restarted unless smEngineStart() is called.
    If the mode is automatic, the tracker automatically calls smEngineStart() 
    whenever tracking conditions are detected to be poor.
    	
    @param engine_handle The engine to query.
    @param on Set to SM_API_FALSE when the mode is manual, SM_API_TRUE when the mode is automatic.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTV2SetAutoRestartMode() */
SM_API(smReturnCode) smHTV2GetAutoRestartMode(smEngineHandle engine_handle, smBool *on);

/*! @brief Set the automatic tracking restart mode.

    @param engine_handle The engine to change.
    @param on SM_API_TRUE for automatic restart, SM_API_FALSE for manual restart.
	@return @ref smReturnCode "SM_API_OK" if the value was written successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTV2GetAutoRestartMode() */
SM_API(smReturnCode) smHTV2SetAutoRestartMode(smEngineHandle engine_handle, smBool on);

/*! @brief Gets the tracking restart-timeout enabled state.

    When restart-timeout is enabled, the tracker will be automatically restarted
    when a the restart-timeout period has elapsed and no tracking has occurred.\n
	
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

/*! @brief Transfer tracking knowledge from one engine to another.

    This function may be useful when running multiple HeadTrackerEngineV2 engines,
    from different cameras that are pointing at the same region of space (for
    example a ring of cameras oriented around a central space). When any engine
    transitions from to the SM_API_ENGINE_STATE_TRACKING state from any other
    state, this function could be called to bootstrap all the other engines
    from it's state, avoiding the need for the other engines to search for 
    the face.

    In the example of a 360 degree ring of cameras, the fist engine to track
    the person would be from the camera that is directly in front of them. 
    When this engine transitions to the tracking state, all the other engines could
    be configured using this function, so when the person turns to the left or right
    there is no delay in adjacent engines also tracking the face.

    To do this, each engine will also need it's automatic restart mode disabled
    using smHTSetAutoRestartMode(SM_API_FALSE). An automatic restart algorithm 
    will need to be implemented by the user. This is a matter of monitoring the 
    set of engines and restarting them all using smEngineStart() when no engine 
    has been tracking for a certain length of time.

    @param engine_handle_from The engine to copy the knowledge from.
    @param engine_handle_to The engine to copy the knowledge to.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smHTV2TransferTrackingKnowledge(smEngineHandle engine_handle_from, smEngineHandle engine_handle_to);

/*! @brief Enable or disable head 3d shape being delivered in the output data.

    This is disabled by default.

    The shape of the head is represented by a 3d vertex array and a list of triangles
    defined by indices into the vertex array. The 3d vertices are defined in the 
    @ref sm_api_coord_frames_standard_head "head-coordinate frame".

    When enabled, the engine will output the shape as a smTriangleMesh in 
    the field smEngineData.face_data->shape, for every frame.

    @param engine_handle The engine to modify.
    @param enabled enables or disables the head-shape output.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully.

    @see 
    - smEngineFaceData
    - smHTV2GetOutputHeadShape() */
//SM_API(smReturnCode) smHTV2SetOutputHeadShape(smEngineHandle engine_handle, smBool enabled);

/*! @brief Determine if the head 3d shape is being delivered in the output data.

    @param engine_handle The engine to query.
    @param enabled Pointer to existing smBool. Set to true if the engine is configured to deliver the 3d shape.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully.

    @see 
    - smEngineFaceData
    - smHTV2SetOutputHeadShape() */
//SM_API(smReturnCode) smHTV2GetOutputHeadShape(smEngineHandle engine_handle, smBool *enabled);

#ifdef __cplusplus
}
#endif
#endif

