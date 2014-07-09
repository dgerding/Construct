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
#ifndef SM_API_HEADTRACKERCONTROLS_H
#define SM_API_HEADTRACKERCONTROLS_H

/*! @file
    Defines functions for controlling the behaviour of all head-tracker types. */
#ifdef __cplusplus
extern "C"
{
#endif

#define SM_HT_MIN_HEADPOSE_PRED_PERIOD_OFFSET_MS -100   /*!< Absolute minimum offset for head-pose prediction */
#define SM_HT_MAX_HEADPOSE_PRED_PERIOD_OFFSET_MS 100    /*!< Absolute maximum offset for head-pose prediction */

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
    - smHTSetAutoRestartMode() */
SM_API(smReturnCode) smHTGetAutoRestartMode(smEngineHandle engine_handle, smBool *on);

/*! @brief Set the automatic tracking restart mode.

    @param engine_handle The engine to change.
    @param on SM_API_TRUE for automatic restart, SM_API_FALSE for manual restart.
	@return @ref smReturnCode "SM_API_OK" if the value was written successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetAutoRestartMode() */
SM_API(smReturnCode) smHTSetAutoRestartMode(smEngineHandle engine_handle, smBool on);

/*! @brief Gets the quality threshold at which the tracking is restarted.

    Higher values cause the tracker to automatically restart more often.
    This parameter has no effect if the the mode is set to SM_API_HT_RESTART_MANUAL.	
    @param engine_handle The engine to query.
    @param percent Set to the threshold value, [0-100].
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetRestartThreshold() */
SM_API(smReturnCode) smHTGetRestartThreshold(smEngineHandle engine_handle, float *percent);

/*! @brief Sets the quality threshold at which the tracking is restarted.  

    @param engine_handle The engine to change.
    @param percent The threshold value to set, [0-100].
	@return @ref smReturnCode "SM_API_OK" if the value was written successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
	@see 
    - smHTGetRestartThreshold() */
SM_API(smReturnCode) smHTSetRestartThreshold(smEngineHandle engine_handle, float percent);

/*! @brief Gets the period of time that the tracker will search before giving up and re-initializing.

    Lower values cause the tracker to automatically re-initialize more often.
    This parameter has no effect if the the init mode is set to SM_API_HT_RESTART_MANUAL    	
    @param engine_handle The engine to query.
    @param secs Set to the current timeout value in seconds.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetRestartTimeout() */
SM_API(smReturnCode) smHTGetRestartTimeout(smEngineHandle engine_handle, float *secs);

/*! @brief Sets the period of time that the tracker will search before giving up and re-initializing.  

    @param engine_handle The engine to change.
    @param secs The value in seconds, must be > 0.
	@return @ref smReturnCode "SM_API_OK" if the value was written successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetRestartTimeout() */
SM_API(smReturnCode) smHTSetRestartTimeout(smEngineHandle engine_handle, float secs);

/*! @brief Gets the head-pose prediction enabled state.

    "Head-pose prediction" may be used to compensate for the unavoidable latency imposed by camera capture time.\n
    It can improve the accuracy of measurement when the head is at constant velocity, but may decrease accuracy when
    the head is accelerating.

    @param engine_handle The engine to change.
    @param enabled Pointer to an existing smBool, set to SM_API_TRUE if head-pose prediction is enabled, SM_API_FALSE otherwise. 
	@return @ref smReturnCode "SM_API_OK" if the function was successful.
    @see
    - smHTSetHeadPosePredictionEnabled() 
    - smHTGetHeadPosePredictionPeriodOffset()
    - smHTSetHeadPosePredictionPeriodOffset() */
SM_API(smReturnCode) smHTGetHeadPosePredictionEnabled(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Sets the head-pose prediction enabled state.

    @param engine_handle The engine to change.
    @param enabled Set to SM_API_TRUE to enabled head-pose prediction, SM_API_FALSE to disable it. 
	@return @ref smReturnCode "SM_API_OK" if the function was successful.
    @see
    - smHTGetHeadPosePredictionEnabled() 
    - smHTGetHeadPosePredictionPeriodOffset()
    - smHTSetHeadPosePredictionPeriodOffset() */
SM_API(smReturnCode) smHTSetHeadPosePredictionEnabled(smEngineHandle engine_handle, smBool enabled);

/*! @brief Gets the head-pose prediction period offset in milliseconds.

    Latency may vary by a few ms, and is composed of processing latency + camera exposure time latency.\n
    Internally the tracker measures the exposure + processing latency so an @a offset of 0 normally
    gives the best performance.

    @param engine_handle The engine to query.
    @param offset Pointer to an existing integer which is set to the value in milliseconds.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see
    - smHTSetHeadPosePredictionPeriod()
    - smHTGetHeadPosePredictionEnabled()     
    - smHTSetHeadPosePredictionPeriodOffset() */
SM_API(smReturnCode) smHTGetHeadPosePredictionPeriodOffset(smEngineHandle engine_handle, int *offset);

/*! @brief Sets the head-pose prediction period offset in milliseconds.

    @param engine_handle The engine to change.
    @param offset Value in milliseconds, between [SM_HT_MIN_HEADPOSE_PRED_PERIOD_OFFSET_MS, SM_HT_MAX_HEADPOSE_PRED_PERIOD_OFFSET_MS].
	@return @ref smReturnCode "SM_API_OK" if the value was written successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see    
    - smHTGetHeadPosePredictionEnabled() 
    - smHTSetHeadPosePredictionEnabled() 
    - smHTGetHeadPosePredictionPeriodOffset() */
SM_API(smReturnCode) smHTSetHeadPosePredictionPeriodOffset(smEngineHandle engine_handle, int offset);

/*! @brief Determines minimum and maximum possible tracking ranges given the resolution and 
    lens configuration that were used to create the engine.

    The values passed to the function smHTSetTrackingRanges() will be automatically clipped by 
    these values.

    @param engine_handle The engine to query.
    @param min Must point to an existing float. Set to the minimum possible range in meters.
    @param max Must point to an existing float. Set to the maximum possible range in meters.
    @note Range values will be biased unless the lens field-of-view used to create the engine or camera is correct.
	@return @ref smReturnCode "SM_API_OK" if the values were read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see
    - smHTSetTrackingRanges()
    - smHTGetTrackingRanges()
    - smFaceSearchGetRangeBounds() */
SM_API(smReturnCode) smHTGetTrackingRangeBounds(smEngineHandle engine_handle, float *min, float *max);

/*! @brief Get the current range settings for head-tracking.

    These are the working distances of the tracker, outside which tracking will not occur. The smaller
    the difference in the min and max range, the faster the tracker will find faces.

    @param engine_handle The engine to query.
    @param min The minimum range that tracking can occur, in meters.
    @param max The maximum range that tracking will occur, in meters. Must be greater than @a min.
    @note Range values will be biased unless the lens field-of-view used to create the engine or camera is correct.
	@return @ref smReturnCode "SM_API_OK" if the values were read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetTrackingRanges() 
    - smHTGetTrackingRangeBounds() */
SM_API(smReturnCode) smHTGetTrackingRanges(smEngineHandle engine_handle, float *min, float *max);

/*! @brief Set the range settings for head-tracking.    

    If your application has known ranges for the distance of the head from the camera then
    setting the tracking ranges here ensures optimal performance in terms of time to acquire
    the face and probability of correct acquisition.

    @param engine_handle The engine to change.
    @param min The minimum range that tracking can occur, in meters. Must be less than or equal to @a max. 
               The value will be clipped by the minimum range lower bound. @see smHTGetTrackingRangeBounds().
    @param max The maximum range that tracking will occur, in meters. Must be greater than or equal to @a min. 
               The value will be clipped by the maximum range lower bound. @see smHTGetTrackingRangeBounds().
    @note The lower bound on @a min and the upper bound on @a max depend on the camera resolution and the lens focal length 
    and can be determined from smHTGetTrackingRangeBounds().
    @note Range values will be biased unless the lens field-of-view used to create the engine or camera is correct.
	@return @ref smReturnCode "SM_API_OK" if the values were written successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetTrackingRanges()
    - smHTGetTrackingRangeBounds() */
SM_API(smReturnCode) smHTSetTrackingRanges(smEngineHandle engine_handle, float min, float max);

/*! @brief Gets the automatic brightness control state.
    @param engine_handle The engine to query.
    @param on Pointer to a user-allocated integer. The value is set to SM_API_FALSE if the controller is off, 
    and SM_API_TRUE when it is on.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetAutoBrightness() */
SM_API(smReturnCode) smHTGetAutoBrightness(smEngineHandle engine_handle, smBool *on);

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
    - smHTGetAutoBrightness() */
SM_API(smReturnCode) smHTSetAutoBrightness(smEngineHandle engine_handle, smBool on);

/*! @brief Get the lip-tracking enabled state.

    By default the lip-tracking is disabled.

    @param engine_handle The engine to change.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the lip-tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
	@pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetLipTrackingEnabled()
    - smHTGetEyebrowTrackingEnabled()
    - smHTSetEyebrowTrackingEnabled() */
SM_API(smReturnCode) smHTGetLipTrackingEnabled(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Enable or disable the lip-tracking.

    By default the lip-tracking is disabled.

    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the lip-tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetLipTrackingEnabled()
    - smHTGetEyebrowTrackingEnabled()
    - smHTSetEyebrowTrackingEnabled() */
SM_API(smReturnCode) smHTSetLipTrackingEnabled(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the eyebrow-tracking enabled state.

    By default the eyebrow-tracking is disabled.

    @param engine_handle The engine to change.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the eyebrow-tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetLipTrackingEnabled()
    - smHTSetLipTrackingEnabled()
    - smHTSetEyebrowTrackingEnabled() */
SM_API(smReturnCode) smHTGetEyebrowTrackingEnabled(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Enable or disable the eyebrow-tracking.

    By default the eyebrow-tracking is disabled.
    
    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the eyebrow-tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetLipTrackingEnabled()
    - smHTSetLipTrackingEnabled()
    - smHTGetEyebrowTrackingEnabled() */
SM_API(smReturnCode) smHTSetEyebrowTrackingEnabled(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the eyebrow-tracking Degrees Of Freedom.

    Advanced used only. By default this is 4.\n
    You may want to decrease this value to obtain more robust tracking.
    
    @param engine_handle The engine to change.
    @param dof Pointer to an existing integer. Set to a value from [1,5].
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetEyebrowTrackingDOF() */
SM_API(smReturnCode) smHTGetEyebrowTrackingDOF(smEngineHandle engine_handle, int *dof);

/*! @brief Set the eyebrow-tracking Degrees Of Freedom.

    Advanced used only. By default this is 4.\n
    You may want to decrease this value to obtain more robust tracking.
    
    @param engine_handle The engine to change.
    @param dof A value from [1,5].
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetEyebrowTrackingDOF() */
SM_API(smReturnCode) smHTSetEyebrowTrackingDOF(smEngineHandle engine_handle, int dof);

#ifdef __cplusplus
}
#endif
#endif

