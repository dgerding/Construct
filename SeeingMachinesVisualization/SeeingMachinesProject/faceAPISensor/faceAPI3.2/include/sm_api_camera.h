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
#ifndef SM_API_CAMERA_H
#define SM_API_CAMERA_H

/*! @file 
    @ingroup sm_api_group_camera
    Defines functions for working with cameras. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Register types of cameras for detection. 

    This function only needs to be called for hardware cameras. 
    The types @ref smCameraType "SM_API_CAMERA_TYPE_IMAGE_PUSH" and @ref smCameraType "SM_API_CAMERA_TYPE_FILE"
    do not need to be registered.
    
    On the first call, this will scan the system and attempt to connect to each instance of the camera type.
    If many cameras are connected to the system, this call may take a while.\n
    
    If called a second time with the same argument, the function will have no effect.    
    
    @param type Set this to one of the possible smCameraType values.

    @return @ref smReturnCode "SM_API_OK" if the camera type was registered successfully.

    @note
    - On windows, the category @ref smCameraType "SM_API_CAMERA_TYPE_WDM" will cause most 
      cameras to be detected unless they have a custom driver supplied by the camera manufacturer.\n
    - For custom cameras, you will need to create an @ref smCameraType "SM_API_CAMERA_TYPE_IMAGE_PUSH" camera, 
      obtain images using the vendor-specific API and use the smCameraImagePush() function.

    @see
    - smCameraDeregisterTypes()
    
    @ingroup sm_api_group_camera */
SM_API(smReturnCode) smCameraRegisterType(smCameraType type);

/*! @brief Deregisters all camera types.
    
    Use this to force redetection of cameras on the next call to smCameraRegisterType() */
SM_API(smReturnCode) smCameraDeregisterTypes();

/*! @brief Creates a list of information for all the available cameras. 

    @param info_list Must point to an existing smCameraInfoList structure.
           The function allocates memory for the smCameraInfoList.info array which is deallocated by smCameraDestroyInfoList().
    @return @ref smReturnCode "SM_API_OK" if camera detection process was successful.
    @return @ref smReturnCode "SM_API_FAIL_INVALID_ARGUMENT" @a info_list is null.
    @note
    - The list will be empty unless smCameraRegisterType() has been called.
    - If a camera is already in use by this process or has been claimed by another process it will still appear in this list. 
    To redetect cameras available for creation, call smCameraDeregisterTypes() followed by smCameraRegisterType().
    
    @ingroup sm_api_group_camera */
SM_API(smReturnCode) smCameraCreateInfoList(smCameraInfoList *info_list);

/*! @brief Frees any memory allocated by smCameraCreateInfoList()
    @param info_list must have been passed to smCameraCreateInfoList() before being passed to this routine.
    @return @ref smReturnCode "SM_API_OK" if the info list was destroyed correctly.
    @return @ref smReturnCode "SM_API_FAIL_INVALID_ARGUMENT" @a info_list is null.
    
    @ingroup sm_api_group_camera*/
SM_API(smReturnCode) smCameraDestroyInfoList(smCameraInfoList *info_list);

/*! @brief Creates the camera defined by @a camera_info and initializes it using any supplied smCameraSettings (optional).
    
    @param camera_info Must be one of the list entries returned by smCameraCreateInfoList(). Do not modify the contents.
    @param settings Optional lens and video format settings for the camera. 
                    If 0, default settings for lens fov (55 deg) and format index will be used.
    @param camera_handle Pointer to smCameraHandle. On success set to a valid smCameraHandle handle.

    @return @ref smReturnCode "SM_API_OK" if the creation process was successful.
    @return @ref smReturnCode "SM_API_FAIL_INVALID_ARGUMENT" @a camera_info or @a camera_handle are null.

    @note
    - Use smCameraDestroy to destroy the created camera.
    - If the camera has been previously created and not yet destroyed, then the function will fail. 
    - The function will fail if during the time between the call to smCameraCreateInfoList() and smCameraCreate()
      another process has taken ownership of the camera. 
    @code
    // Create first webcam using lens field-of-view of 30 deg
	int MY_CAMERA_FOV = 30;
    smCameraSettings settings;
    memset(&settings,0,sizeof(smCameraSettings));
    settings.approx_fov_deg = &MY_CAMERA_FOV;
    smReturnCode result = smCameraCreate(camera_info_list[0],&settings,&my_camera_handle);
    @endcode 
*/
SM_API(smReturnCode) smCameraCreate(const smCameraInfo* camera_info, const smCameraSettings *settings, smCameraHandle *camera_handle);

/*! @brief Creates an "Image Push" camera, which can then be used by the client to "push" image data into the camera,
    using smCameraCreateImagePush().

    Use an Image Push camera to pass your own image data into API functions that require a smCameraHandle argument.

    @param video_format Format of the video the camera will produce. 
           Images pushed into the camera must be of the same resolution and smImageCode as defined by this format.
    @param max_fifo_len Maximum number of video-frames that can be stored internally in the camera before the oldest
           video-frame will be discarded (dropped).
           If the camera is being used by an engine, the engine will consume the images, so this fifo is used to 
           buffer timing fluctuations between the producer (you) and the consumer (the engine).
           Set to SM_API_CAMERA_DEFAULT_FIFO_LEN to remove any upper limit on the fifo (limited by RAM).
    @param settings Optional settings for the camera. If 0, default settings will be used. The format_index field is ignored.
    @param push_camera_handle Pointer to smCameraHandle. On success this is set to a valid smCameraHandle handle.

    @return @ref smReturnCode "SM_API_OK" if the creation process was successful. 
    @return @ref smReturnCode "SM_API_FAIL_INVALID_ARGUMENT" @a video_format or @a push_camera_handle are null.

    @note
    - The image resolution specified by @a video_format must be larger than 0.
    - For @ref smImageCode "SM_API_IMAGECODE_YUY2" images, the image width must be divisible by 2. 
    - For @ref smImageCode "SM_API_IMAGECODE_I420", both the image width and height must be divisible by 2.         
    - Use smCameraDestroy to destroy the created camera.
    - If the camera has been previously created and not yet destroyed, then the function will fail.
    - All subsequent calls to smCameraImagePush must provide image arguments that are consistent with the specified @a vdeo_format. */  
SM_API(smReturnCode) smCameraCreateImagePush(const smCameraVideoFormat* video_format, int max_fifo_len, const smCameraSettings *settings, smCameraHandle *push_camera_handle);

/*! @brief Creates a camera that obtains its image data from a movie file.

    Valid file types are .mov, .avi, .wmv, .asf, .mp4 and potentially others depending on the
    codecs installed on the PC. As a general guide if the file is playable in a media-player application
    and is internally indexed by frame-number, then it is likely to work. Files that do not have valid durations 
    under the file properties (right-click the file under windows explorer) generally cannot be used. 
    @param filename Path to the movie file including file extension.
    @param settings Optional settings for the camera. If 0, default settings will be used. The format_index field is ignored.
    @param file_camera_handle Pointer to an smCameraHandle. On success this is set to a valid smCameraHandle handle.
    @return @ref smReturnCode "SM_API_OK" if the camera was created and the movie file was successfully checked to be valid. */
SM_API(smReturnCode) smCameraCreateFile(smStringHandle filename, const smCameraSettings *settings, smCameraHandle *file_camera_handle);

/*! @brief Determines if an smCameraHandle is valid (has been created and not destroyed)

    @param camera_handle A camera to test for validity.
    @param valid Set to SM_API_TRUE if the image is valid, SM_API_FALSE otherwise.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smCameraIsValid(smCameraHandle camera_handle, smBool *valid);

/*! @brief Gets the smCameraType for the camera.

    @param camera_handle The camera to obtain information from. Must be an existing valid camera.
    @param camera_type Pointer to an existing smCameraType which is set to the type.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smCameraGetType(smCameraHandle camera_handle, smCameraType *camera_type);

/*! @!brief Gets the model name for the camera.

    @param camera_handle The camera to obtain information from. Must be an existing valid camera.
    @param model_name Must be an existing valid string handle. The string is set to the camera model name.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smCameraGetModelName(smCameraHandle camera_handle, smStringHandle model_name);

/*! @brief Gets the current video format of the camera as a smCameraVideoFormat, given a specific smCameraHandle.

    @param camera_handle The camera to obtain information from. Must be an existing valid camera.
    @param video_format Pointer to an existing smCameraVideoFormat which is set to contain the format information.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smCameraGetCurrentFormat(smCameraHandle camera_handle, smCameraVideoFormat *video_format);

/*! @brief Notifies any engines using the ImagePush camera that the rate of pushing images has changed. 

    Call this if you change the frequency of calls to smCameraImagePush(), usually because your custom
    camera is changing it's frame-rate. 
    @param push_camera_handle Handle to an existing image-push camera
    @param frame_rate Must be > 0.0 
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smCameraImagePushSetFrameRate(smCameraHandle push_camera_handle, float frame_rate);

/*! @brief Push an smImage into an "Image Push" smCameraHandle
    
    An smEngine must be created using the image-push camera ( using smEngineCreateWithCamera() ) before smCameraImagePush()
    can be called.    

    @param push_camera_handle An "Image Push" camera. This must previously have been created with smCameraCreateImagePush().
    @param video_frame Contains the image and other information to be pushed.  See smImageCreateFromInfo().
    @param lens_params Optional argument (can be 0) that can be used to override any existing lens-parameters the camera is using.
           This argument is only necessary if the lens is changing dynamically (e.g motorized zoom lens).
    @param src_rect <b>NOTE: This argument is not currently supported. Set to 0 only.</b>\n
		   Optional argument (can be 0) describing the position and size of the region inside the image dimensions of the 
           image used to calibrate the sensor, where the image being pushed is obtained from.\n
           This is typically used when a camera driver is able to provide a region-of-interest (ROI)
           sub-image. This argument then defines where that sub-image is located.\n
           It can also be used to specify a ROI image that has also been resized / decimated by the camera driver.
           For example, if the camera was calibrated at the native sensor resolution of (1280,1024) and
           is then configured to deliver (640,480) sized images at particular offsets, then the @a top_left 
           parameter is set to the offset in the (1280,1024) image.
           The following @ref sm_api_image_push_camera_roi "diagram" illustrates the @a src_rect concept.
                
    @return @ref smReturnCode "SM_API_OK" if the push succeeds.
    @return @ref smReturnCode "SM_API_FAIL_CAMERA_IMAGE_PUSH" if the push fails because the fifo is full.
    @return @ref smReturnCode "SM_API_FAIL_INVALID_ARGUMENT" @a sm_camera_handle is null or invalid, @a sm_video_frame is null.
    
    @see
    - smImageHandle
    - smImageInfo    
    - smRegisterImageReleaseCallback() 
    - smCameraCreateImagePush() */
SM_API(smReturnCode) smCameraImagePush(smCameraHandle push_camera_handle, const smCameraVideoFrame* video_frame, const smCameraLensParams* lens_params, smImageRect *src_rect);

/*! @brief Push an smImage into an "Image Push" smCameraHandle, blocking if the fifo is full, until the fifo is
    able to accept the new image, or timeout occurs.
    
    An smEngine must be created using the image-push camera ( using smEngineCreateWithCamera() ) before smCameraImagePush()
    can be called.    

    @param push_camera_handle An "Image Push" camera. This must previously have been created with smCameraCreateImagePush().
    @param timeout_ms Maximum time to block caller before failing to push. A value of 0 will return immediately if the input fifo is full. 
           Set to INT_MAX to wait forever on a full fifo.
           For live video tracking a recommended value is > 2x the frame period (e.g. for 30Hz, set to 67ms).
    @param video_frame Contains the image and other information to be pushed.  See smImageCreateFromInfo().
    @param lens_params Optional argument (can be 0) that can be used to override any existing lens-parameters the camera is using.
           This argument is only necessary if the lens is changing dynamically (e.g motorized zoom lens).
    @param src_rect <b>NOTE: This argument is not currently supported. Set to 0 only.</b>\n
		   Optional argument (can be 0) describing the position and size of the region inside the image dimensions of the 
           image used to calibrate the sensor, where the image being pushed is obtained from.\n
           This is typically used when a camera driver is able to provide a region-of-interest (ROI)
           sub-image. This argument then defines where that sub-image is located.\n
           It can also be used to specify a ROI image that has also been resized / decimated by the camera driver.
           For example, if the camera was calibrated at the native sensor resolution of (1280,1024) and
           is then configured to deliver (640,480) sized images at particular offsets, then the @a top_left 
           parameter is set to the offset in the (1280,1024) image.
           The following @ref sm_api_image_push_camera_roi "diagram" illustrates the @a src_rect concept.
                
    @return @ref smReturnCode "SM_API_OK" if the push succeeds.
    @return @ref smReturnCode "SM_API_FAIL_CAMERA_IMAGE_PUSH" if the push fails because the fifo is full and timeout.
    @return @ref smReturnCode "SM_API_FAIL_INVALID_ARGUMENT" @a sm_camera_handle is null or invalid, @a sm_video_frame is null, or timeout_ms < 0.
    
    @see
    - smImageHandle
    - smImageInfo    
    - smRegisterImageReleaseCallback() 
    - smCameraCreateImagePush() */
SM_API(smReturnCode) smCameraImagePushBlock(smCameraHandle push_camera_handle, int timeout_ms, const smCameraVideoFrame* video_frame, const smCameraLensParams* lens_params, smImageRect *src_rect);

/*! @brief Loads the lens parameters from a filename.
    
    Lens parameter files can be generated by the API tool "CamCal.exe", which is part of the API distribution.
    @param filename Path and filename of the calibration file.
    @param params Must point to an existing smCameraLensParams struct (cannot be 0) 
    @return @ref smReturnCode "SM_API_OK" if the load occured ok. */
SM_API(smReturnCode) smCameraLoadLensParamsFile(smStringHandle filename, smCameraLensParams *params);

/*! @brief Sets the lens parameters from an approximate field-of-view measurement

    This generates an approximate set of lens coefficients, which may be suitable for some algorithms to use. 
    @param hfov_deg Approximate field-of-view of the lens, >0 and <180 degrees.
    @param res Resolution of the images that the camera will produce when tracking.
    @param params Must point to an existing smCameraLensParams struct (cannot be 0), which is set.
    
    @return @ref smReturnCode "SM_API_OK" if params was set ok.
    
    @see
    - smCameraLoadLensParamsFile() */
SM_API(smReturnCode) smCameraLensParamsFromHFOV(int hfov_deg, smSize2i res, smCameraLensParams *params);

/*! @brief Set the lens-parameters of the camera.
    
    Lens parameters are required for accurate measurement and for measurements in world-coordinates such as the 
    maximum and minimum tracking ranges to make sense. If the lens parameters are not set, camera are constructed
    to use an approximate lens configuration, which may be suitable for some applications.
    
    @param camera_handle The camera to assign the lens-parameters for.
    @param lens_params The lens parameters to assign. This parameter must have been previously initialized 
                       with valid parameters using smCameraLensParamsFromHFOV() or smCameraLoadLensParamsFile().
    @note
    - To calibrate the camera lens accurately, use the tool "CamCal.exe" provided as part of this API.
      The tool produces a lens-calibration file which can be loaded using smCameraLoadLensParamsFile().

    @return @ref smReturnCode "SM_API_FAIL_INVALID_ARGUMENT" @a sm_camera_handle is null or invalid or @a lens_params is null. */
SM_API(smReturnCode) smCameraSetLensParams(smCameraHandle camera_handle, const smCameraLensParams* lens_params);

/*! @brief Get the current lens-parameters the camera is using.

    @param camera_handle Must be a valid smCameraHandle.
    @param lens_params Pointer to an existing smCameraLensParams. The contents are changed to be the lens parameters the camera is using. 
    @return @ref smReturnCode "SM_API_OK" if the function succeeded. */
SM_API(smReturnCode) smCameraGetLensParams(smCameraHandle camera_handle, smCameraLensParams *lens_params);

/*! @brief Get the number of possible exposure modes that the camera supports.

    @param camera_handle Must be a valid smCameraHandle.
    @param num_exposure_modes Must point to an existing integer. Set to the number of modes.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded.
    @see 
    - smCameraEnumExposureModes() 
    - smCameraGetExposureMode() 
    - smCameraSetExposureMode()
    - smCameraExposureMode */
SM_API(smReturnCode) smCameraGetNumExposureModes(smCameraHandle camera_handle, int *num_exposure_modes);

/*! @brief Get the Nth supported exposure-mode from the set of possible exposure modes that the camera supports.

    @param camera_handle Must be a valid smCameraHandle.
    @param index Value from [0,max_modes-1] where max_modes determined by smCameraGetNumExposureModes().
    @param exposure_mode Must point to an existing smCameraExposureMode variable. Set to supported mode value.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded.
    @see 
    - smCameraGetNumExposureModes() 
    - smCameraGetExposureMode() 
    - smCameraSetExposureMode()
    - smCameraExposureMode */
SM_API(smReturnCode) smCameraEnumExposureModes(smCameraHandle camera_handle, int index, smCameraExposureMode *exposure_mode);

/*! @brief Get the current exposure-mode that the camera is using.

    @param camera_handle Must be a valid smCameraHandle.
    @param exposure_mode Must point to an existing smCameraExposureMode variable. Set to the current exposure-mode value.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded.
    @see 
    - smCameraGetNumExposureModes() 
    - smCameraEnumExposureModes() 
    - smCameraSetExposureMode()
    - smCameraExposureMode */
SM_API(smReturnCode) smCameraGetExposureMode(smCameraHandle camera_handle, smCameraExposureMode *exposure_mode);

/*! @brief Set the current exposure-mode for the camera to use.

    @param camera_handle Must be a valid smCameraHandle.
    @param exposure_mode Must be a exposure mode supported by the camera, otherwise the function will return an error.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded.
    @see 
    - smCameraGetNumExposureModes() 
    - smCameraEnumExposureModes() 
    - smCameraGetExposureMode()
    - smCameraExposureMode */
SM_API(smReturnCode) smCameraSetExposureMode(smCameraHandle camera_handle, smCameraExposureMode exposure_mode);

/*! @brief Enable or disable IR specular reflection minimization pre-processing. 

	This processing removes specular reflections from glasses that can occur when illuminating the face
	with local light sources. These reflections can cause problems for applications that need to see the
	eye clearly.

    @note: This option is only valid when using the faceAPI IR Lighting Kit which provides illuminators that are synchronized to the camera. 

    @param camera_handle Must be a valid smCameraHandle. The camera must be a PointGrey camera, and the exposure mode must be set to SM_API_EXPOSURE_MODE_IR.
	@param enabled Pass either SM_API_TRUE or SM_API_FALSE.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded.
    @see 
    - smCameraGetReflectionMinimize() */
SM_API(smReturnCode) smCameraSetReflectionMinimize(smCameraHandle camera_handle, smBool enabled);

/*! @brief Determine if IR specular reflection minimization pre-processing is enabled. 

    @param camera_handle Must be a valid smCameraHandle.
	@param enabled Pointer to existing smBool variable which is set to either SM_API_TRUE or SM_API_FALSE.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded.
    @see 
    - smCameraSetReflectionMinimize() */
SM_API(smReturnCode) smCameraGetReflectionMinimize(smCameraHandle camera_handle, smBool *enabled);

/*! @brief Set brightness of image that camera produces. 

	The behaviour of this function may vary depending on the specific camera and the exposure mode.

    @note Some engines can automatically control the camera brightness based on the image processing
	they do. For automatic control over this brightness when using the camera with a HeadTracker refer to:
    - smHTGetAutoBrightness()
    - smHTSetAutoBrightness() 

    @param camera_handle Must be a valid smCameraHandle.
    @param brightness_percent Brightness value to set, as integer from 0 to 100.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded.
    @see 
    - smCameraGetImageBrightness() 
    - smHTGetAutoBrightness()
    - smHTSetAutoBrightness() */
SM_API(smReturnCode) smCameraSetImageBrightness(smCameraHandle camera_handle, int brightness_percent);

/*! @brief Get brightness of image that camera produces.

    @param camera_handle Must be a valid smCameraHandle.
    @param brightness_percent Pointer to an existing integer, set to a value from 0 to 100.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded.
    @see 
    - smCameraSetImageBrightness() */
SM_API(smReturnCode) smCameraGetImageBrightness(smCameraHandle camera_handle, int *brightness_percent);

/*! @brief Restores camera register settings from memory for a PointGrey camera.

    @param ptgrey_camera_handle Must be a valid smCameraHandle and the camera must be a PointGrey camera.
    @param channel_num Memory channel index, from 0 to the number of channels the camera has (depends on the PtGrey camera model).
           Passing 0 usually restores the camera configuration to it's factory settings (check the manual).

    @return @ref smReturnCode "SM_API_OK" if the function succeeded. 
    
    @pre This function must be called before the camera is used in any engine, so usually this would be
         immediately after the camera is created using smCameraCreate().
    
    @note This function is provided so PointGrey cameras can be pre-configured using the "FlyCap" application
          (which is a tool in the PointGrey FlyCapture SDK). Using FlyCap you can store the camera configuration you need into 
          a memory channel for the camera, and then call this faceAPI function in your application to restore those settings. 
          This has the limitation that you must not alter the image resolution or frame-rate as they are configured using smCameraCreate(). 
          It is provided to allow custom control over shutter speed and other functions that effect image-quality. */
SM_API(smReturnCode) smCameraPtGreyRestoreMemoryChannel(smCameraHandle ptgrey_camera_handle, int channel_num);

/*! @brief Destroys the specific camera, freeing any internally allocated resources and releasing any handles.

    @param camera_handle Pointer to an existing valid (created) smCameraHandle. Set to 0 upon success.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded. */
SM_API(smReturnCode) smCameraDestroy(smCameraHandle *camera_handle);

/*! @brief Shows a control panel for the camera.

    @param camera_handle Existing (created) smCameraHandle handle.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded. */
SM_API(smReturnCode) smCameraShowControlPanel(smCameraHandle camera_handle);

/*! @brief Determines if a control panel for the camera exists.

    @param camera_handle Existing (created) smCameraHandle handle. 
    @param result Pointer to an existing smBool.
    @return @ref smReturnCode "SM_API_OK" if the function succeeded. */
SM_API(smReturnCode) smCameraHasControlPanel(smCameraHandle camera_handle, smBool *result);

#ifdef __cplusplus
}
#endif
#endif
