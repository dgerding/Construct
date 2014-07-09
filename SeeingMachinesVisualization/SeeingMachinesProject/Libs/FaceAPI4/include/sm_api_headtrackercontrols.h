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

/*! @brief Gets the maximum faces that may be tracked at one time.

    @note HeadTrackerEngineV2 is only able to track a single face at one time
    so this function will always set max_faces to 1 for that engine type.

    @param engine_handle The engine to query.
    @param max_faces Set to the maximum number of faces the engine is configured to track at one time.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetMaxFaces() */
SM_API(smReturnCode) smHTGetMaxFaces(smEngineHandle engine_handle, int *max_faces);

/*! @brief Sets the maximum faces that may be tracked at one time.

    @note HeadTrackerEngineV2 is only able to track a single face at one time
    so this function will always set max_faces to 1 for that engine type.

    @note HeadTrackerEngineV3 is more lightweight when configured to track a maximum
    of 1 face as it will not continuously hunt for new faces.

    @param engine_handle The engine to change.
    @param max_faces The maximum number of faces the engine will track at one time.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetMaxFaces() */
SM_API(smReturnCode) smHTSetMaxFaces(smEngineHandle engine_handle, int max_faces);

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

/*! @brief Get the current head-pose pitch settings for head-tracking.

    These are minimum and maximum vertical angles between the head and the camera.

    They are not absolute limits, the values are used as hints for the tracker to be 
    able to locate the face efficiently.

    The default values are -30 to 30 degrees. 
    The maximum recommended values are -30 (min) to 45 (max).

    If the camera is below the face, looking up, then increase the values upwards by the up-angle
    For example, if the camera is tilted up 5 degrees, set -25, 35.
    And if the camera is tilted up 30 degrees, set to 0, 45 (max).

    @param engine_handle The engine to query.
    @param min The minimum pitch angle in degrees of the head-pose.
    @param max The maximum pitch angle in degrees of the head-pose.
	@return @ref smReturnCode "SM_API_OK" if the values were read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetHeadPosePitchRange() */
SM_API(smReturnCode) smHTGetHeadPosePitchRange(smEngineHandle engine_handle, float *min, float *max);

/*! @brief Set the head-pose pitch range settings for head-tracking.    

    These are minimum and maximum vertical angles between the head and the camera.

    They are not absolute limits, the values are used as hints for the tracker to be 
    able to locate the face efficiently.

    The default values are -30 to 30 degrees. 
    The maximum recommended values are -30 (min) to 45 (max).

    If the camera is below the face, looking up, then increase the values upwards by the up-angle
    For example, if the camera is tilted up 5 degrees, set -25, 35.
    And if the camera is tilted up 30 degrees, set to 0, 45 (max).

    @param engine_handle The engine to change.
    @param min The minimum pitch angle in degrees of the head-pose.
    @param max The maximum pitch angle in degrees of the head-pose.
	@return @ref smReturnCode "SM_API_OK" if the values were written successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetHeadPosePitchRange() */
SM_API(smReturnCode) smHTSetHeadPosePitchRange(smEngineHandle engine_handle, float min, float max);

/*! @brief Sets the level of head-pose filtering.

    @param engine_handle The engine to change.
    @param level Set to a filter level:
    - 0 is for no filtering (default)
    - 1 is for mild filtering 
    - 2 is for maximum filtering
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetHeadPoseFilterLevel() */
SM_API(smReturnCode) smHTSetHeadPoseFilterLevel(smEngineHandle engine_handle, int level);

/*! @brief Gets the level of head-pose filtering.

    @param engine_handle The engine to query.
    @param level Pointer to an existing integer, set to the filter level:
    - 0 is for no filtering (default)
    - 1 is for mild filtering 
    - 2 is for maximum filtering
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetHeadPoseFilterLevel() */
SM_API(smReturnCode) smHTGetHeadPoseFilterLevel(smEngineHandle engine_handle, int *level);

/*! @brief Gets the maximum level of head-pose filtering.

    @param engine_handle The engine to query.
    @param max_level Pointer to an existing integer, set to the maximum filter level (currently 2).
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully.
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetHeadPoseFilterLevel()
    - smHTGetHeadPoseFilterLevel() */
SM_API(smReturnCode) smHTGetHeadPoseMaxFilterLevel(smEngineHandle engine_handle, int *max_level);


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

    Advanced used only. By default this is 5.\n
    You may want to decrease this value to obtain more robust tracking.
    
    @param engine_handle The engine to change.
    @param dof Pointer to an existing integer. Set to a value from [1,5].
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetEyebrowTrackingDOF() */
SM_API(smReturnCode) smHTGetEyebrowTrackingDOF(smEngineHandle engine_handle, int *dof);

/*! @brief Set the eyebrow-tracking Degrees Of Freedom.

    Advanced used only. By default this is 5.\n
    You may want to decrease this value to obtain more robust tracking.
    
    @param engine_handle The engine to change.
    @param dof A value from [1,5].
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetEyebrowTrackingDOF() */
SM_API(smReturnCode) smHTSetEyebrowTrackingDOF(smEngineHandle engine_handle, int dof);

/*! @brief Get the enabled state for eye-closure tracking.

    By default the eye-closure tracking is disabled.
    
    @param engine_handle The engine to change.
    @param enabled Pointer to an existing smBool. Sets to the enabled state of the eye-closure tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was obtained successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetEyeClosureTrackingEnabled()
    - smHTGetLipTrackingEnabled()
    - smHTSetLipTrackingEnabled()
    - smHTGetEyebrowTrackingEnabled() */
SM_API(smReturnCode) smHTGetEyeClosureTrackingEnabled(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Enable or disable eye-closure tracking.

    By default the eye-closure tracking is disabled.
    
    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the eye-closure tracking.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetEyeClosureTrackingEnabled()
    - smHTGetLipTrackingEnabled()
    - smHTSetLipTrackingEnabled()
    - smHTGetEyebrowTrackingEnabled() */
SM_API(smReturnCode) smHTSetEyeClosureTrackingEnabled(smEngineHandle engine_handle, smBool enabled);

/*! @brief Determine if eye-closure algorithm is treating both eyes independently.

    By default the algorithm assumes both eyes have the same closure amount in 
    order to produce the most robust "blink" estimation.

    A "blink" is defined as when both eyes open and close simultaneously. So a 
    "wink" is not a blink. When @a independent_eyes is true, a wink can be 
    represented in the output data, but the data will be more noisy. 
    When @a independent_eyes is false (default) the closure data for the left
    and right eyes are always the same.

    To get the best results from the algorithm:
    - ensure the resolution on the eyes is high (close to camera)
    - ensure bright ambient lighting to avoid motion blur in the image
    - avoid rapid head movements.
    
    @param engine_handle The engine to change.
    @param independent_eyes Pointer to an existing smBool. Sets to the state of the eye-independence flag.
	@return @ref smReturnCode "SM_API_OK" if the value was obtained successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetEyeClosureEyeIndependence()
    - smHTGetEyeClosureTrackingEnabled() */
SM_API(smReturnCode) smHTGetEyeClosureEyeIndependence(smEngineHandle engine_handle, smBool *independent_eyes);

/*! @brief Determine if the eye-closure algorithm should treat both eyes independently.

    By default the algorithm assumes both eyes have the same closure amount in 
    order to produce the most robust "blink" estimation.

    A "blink" is defined as when both eyes open and close simultaneously. So a 
    "wink" is not a blink. When @a independent_eyes is true, a wink can be 
    represented in the output data, but the data will be more noisy. 
    When @a independent_eyes is false (default) the closure data for the left
    and right eyes are always the same.

    To get the best results from the algorithm:
    - ensure the resolution on the eyes is high (close to camera)
    - ensure bright ambient lighting to avoid motion blur in the image
    - avoid rapid head movements.
    
    @param engine_handle The engine to change.
    @param independent_eyes Sets the state of the eye independence flag.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetEyeClosureEyeIndependence()
    - smHTSetEyeClosureTrackingEnabled() */
SM_API(smReturnCode) smHTSetEyeClosureEyeIndependence(smEngineHandle engine_handle, smBool independent_eyes);

/*! @brief Set the engine face-detection "strict-mode" option.

    By default strict-mode is disabled.

    Strict mode decreases the false-positive detection rate and likelihood of inaccurate
    face location measurement, especially in the prescence of glasses. 

    Strict mode increases processing time marginally when the face is first searched for,
    but has no effect on the frame-to-frame processing.

    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the strict-mode.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetStrictFaceDetection() */
SM_API(smReturnCode) smHTSetStrictFaceDetection(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the engine face-detection "strict-mode" option.

    By default strict-mode is disabled.

    Strict mode decreases the false-positive detection rate and likelihood of 
    inaccurate face location measurement, especially in the presence of glasses. 

    Strict mode increases processing time marginally when the face is first 
    searched for, but has no effect on the frame-to-frame processing.

    @param engine_handle The engine to query.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the strict-mode.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetStrictFaceDetection() */
SM_API(smReturnCode) smHTGetStrictFaceDetection(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Set the engine face-detection "motion-detection" option.

    By default this mode is enabled.

    Motion detection mode causes the searching of faces to stop when there is no 
    movement in the image. The face-searching algorithm can consume significant 
    CPU, so this option allows the CPU to idle when there are no faces to be 
    tracked to save power.

    The downside is that if a user holds their face very still and restarts 
    tracking, their face may not be detected for tracking, but even a small 
    movement will trigger the tracker so this is generally safe.

    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the motion-detection mode.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetMotionFaceDetection() */
SM_API(smReturnCode) smHTSetMotionFaceDetection(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the engine face-detection "motion-detection" option.

    By default this mode is enabled.

    @param engine_handle The engine to query.
    @param enabled Pointer to an existing smBool. Set to the enabled state of the motion-detection mode.
	@return @ref smReturnCode "SM_API_OK" if the value was read successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetMotionFaceDetection() */
SM_API(smReturnCode) smHTGetMotionFaceDetection(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Set the engine option to emit a face-texture per-frame (if face is oriented toward the camera).

    By default this option is disabled.

    When this option is disabled, and the face-texture is only produced on the 
    first frame where the face is detected.

    When this option is enabled, if the face is oriented toward the camera then image
    processing will be performed to obtain a fresh texture snapshot (mugshot) of the 
    face region.

    Enabling this option increases the CPU load slightly due to the extra processing
    of the face-texture.

    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the option.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetFaceTexturePerFrame() */
SM_API(smReturnCode) smHTSetFaceTexturePerFrame(smEngineHandle engine_handle, smBool enabled);

/*! @brief Get the engine option to emit a face-texture per-frame (if face is oriented toward the camera).

    By default this option is disabled.

    When this option is disabled, and the face-texture is only produced on the 
    first frame where the face is detected.

    When this option is enabled, if the face is oriented toward the camera then image
    processing will be performed to obtain a fresh texture snapshot (mugshot) of the 
    face region.

    Enabling this option increases the CPU load slightly due to the extra processing
    of the face-texture.

    @param engine_handle The engine to query.
    @param enabled Pointer to an existing smBool, which is set to the enabled state of the option.
	@return @ref smReturnCode "SM_API_OK" if the value was obtained successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetFaceTexturePerFrame()
    - smHTGetFaceTextureSize() 
    - smHTSetFaceTextureSize() */
SM_API(smReturnCode) smHTGetFaceTexturePerFrame(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Set the size of the face-texture image the engine produces.

    By default the engine produces 256x256 sized textures.

    @param engine_handle The engine to change.
    @param size Size of the texture image in pixels.
	@return @ref smReturnCode "SM_API_OK" if the values were set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetFaceTextureSize() 
    - smHTGetFaceTexturePerFrame() */
SM_API(smReturnCode) smHTSetFaceTextureSize(smEngineHandle engine_handle, smSize2i size);

/*! @brief Get the size of the face-texture image the engine produces.

    By default the engine produces 256x256 sized textures.

    @param engine_handle The engine to query.
    @param size Pointer to an existing smSize2i struct which is set to the size of the texture image in pixels.
	@return @ref smReturnCode "SM_API_OK" if the values were obtained successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetFaceTextureSize() 
    - smHTGetFaceTexturePerFrame() */
SM_API(smReturnCode) smHTGetFaceTextureSize(smEngineHandle engine_handle, smSize2i *size);

/*! @brief Determine number of supported GPUs the engine can potentially execute on.

    @param engine_handle The engine to query.
    @param num_gpus Pointer to an existing int, which is set to the number of GPUs, or 0 if none are available.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetGPUName()
    - smHTGetGPUAccel()
    - smHTSetGPUAccel() 
    - smHTGetGPUAffinity()
    - smHTSetGPUAffinity() */
SM_API(smReturnCode) smHTGetNumGPUs(smEngineHandle engine_handle, int *num_gpus);

/*! @brief Get the model name of a specific GPU device.

    @param engine_handle The engine to query.
    @param gpu_index A value between [0 .. @a num_gps-1] @see smHTGetNumGPUs()
    @param gpu_name Must be an existing valid string handle. The string is set to the gpu model name.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @see
    - smHTGetNumGPUs() 
    - smHTGetGPUAccel()
    - smHTSetGPUAccel() 
    - smHTGetGPUAffinity()
    - smHTSetGPUAffinity() */
SM_API(smReturnCode) smHTGetGPUName(smEngineHandle engine_handle, int gpu_index, smStringHandle gpu_name);

/*! @brief Set engine to execute using GPU hardware.

    By default this option is disabled.

    This is currently implemented using CUDA and requires an NVidia GPU and driver
    that supports CUDA to be installed.
    
    @note Please ensure that the absolute latest drivers are installed for any CUDA devices.
    @see http://www.nvidia.com/object/cuda_get.html.

    If the CUDA is not available, attempting to enable the option will return 
    @ref smReturnCode "SM_API_FAIL_HARDWARE_INADEQUATE".

    Performance differences:
    - Decrease in host system CPU load (system dependent)
    - Slightly increased PCIe bus traffic (approximately 35MB/s)
    - The tracker will be less susceptible to performance spikes due to contention with other processes.
    - On some systems with fast CPUs, latency may be increased slightly. 
      GPU processing latency is about 8ms on compute 2.x devices and some CPUs are worse, some better, than this figure.

    @param engine_handle The engine to change.
    @param enabled Sets the enabled state of the option.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTGetGPUAccel()
    - smHTGetNumGPUs()
    - smHTGetGPUName()
    - smHTGetGPUAffinity()
    - smHTSetGPUAffinity() */
SM_API(smReturnCode) smHTSetGPUAccel(smEngineHandle engine_handle, smBool enabled);

/*! @brief Determine if engine is executing using GPU hardware.

    @param engine_handle The engine to query.
    @param enabled Pointer to an existing smBool, which is set to the enabled state of the option.
	@return @ref smReturnCode "SM_API_OK" if the value was set successfully. 
    @pre state >= @ref SM_API_ENGINE_STATE_IDLE
    @see 
    - smHTSetGPUAccel() 
    - smHTGetNumGPUs()
    - smHTGetGPUName()
    - smHTGetGPUAffinity()
    - smHTSetGPUAffinity() */
SM_API(smReturnCode) smHTGetGPUAccel(smEngineHandle engine_handle, smBool *enabled);

/*! @brief Get the index of the GPU that engine prefers to execute on.

    @param engine_handle The engine to query.
    @param gpu_index Pointer to an existing int, which is set to a value between [0 .. @a num_gps-1]
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @see
    - smHTSetGPUAffinity()
    - smHTGetNumGPUs() 
    - smHTGetGPUName()
    - smHTGetGPUAccel()
    - smHTSetGPUAccel() */
SM_API(smReturnCode) smHTGetGPUAffinity(smEngineHandle engine_handle, int *gpu_index);

/*! @brief Set the GPU that engine prefers to execute on.

    @param engine_handle The engine to modify.
    @param gpu_index A value between [0 .. @a num_gps-1] @see smHTGetNumGPUs()
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @see
    - smHTGetGPUAffinity()
    - smHTGetNumGPUs() 
    - smHTGetGPUName()
    - smHTGetGPUAccel()
    - smHTSetGPUAccel() */
SM_API(smReturnCode) smHTSetGPUAffinity(smEngineHandle engine_handle, int gpu_index);

/*! @brief Get the maximum value of the CPU effort parameter.

    The maximum effort depends on the type of engine.

    @param engine_handle The engine to query.
    @param max_effort Set to the maximum possible effort value.

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @see
    - smHTSetTrackingEffort()
    - smHTGetTrackingEffort() */
SM_API(smReturnCode) smHTGetMaxTrackingEffort(smEngineHandle engine_handle, int *max_effort);

/*! @brief Get the current CPU effort setting.

    Increased tracking effort equates more CPU resources and better tracking results.

    @param engine_handle The engine to query.
    @param effort Set to the current effort value.

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @see
    - smHTSetTrackingEffort()
    - smHTGetMaxTrackingEffort() */
SM_API(smReturnCode) smHTGetTrackingEffort(smEngineHandle engine_handle, int *effort);

/*! @brief Set the CPU effort to track the face.

    Increased tracking effort requires more CPU resources and better tracking results.

    The outcome of increased effort is engine dependent but typically produces
    reduced noise and increased tracking robustness. 
    
    This setting is provided so programs that use faceAPI have the option to scale their 
    performance depending on the CPU resources available.

    Performance is tuned so that the CPU footprint of a specific effort setting 
    will not change across releases. Currently, 0 is designed to offer the same 
    performance as the HeadTrackerEngineV1 in faceAPI V3, 
    1 is the default effort of HeadTrackinerEngineV2 (which is an increase CPU by about 5% on a Core i7 CPU).
    2 is the default effort of HeadTrackinerEngineV3.

    @param engine_handle The engine to modify.
    @param effort Current a value between 0 (low) and the engine dependent maximum value.

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @see
    - smHTGetTrackingEffort()
    - smHTGetMaxTrackingEffort() */
SM_API(smReturnCode) smHTSetTrackingEffort(smEngineHandle engine_handle, int effort);

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

#ifdef __cplusplus
}
#endif
#endif

