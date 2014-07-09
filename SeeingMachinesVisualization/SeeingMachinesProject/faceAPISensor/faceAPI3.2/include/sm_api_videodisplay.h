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
#ifndef SM_API_VIDEODISPLAY_H
#define SM_API_VIDEODISPLAY_H

/*! @file
    Defines all types and routines in the API relating to video display. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @struct smVideoDisplayHandle
    @brief Handle to a video display that can be used by the API.

    @see
    - @ref smVideoDisplayCreate()     
    - @ref sm_api_handles_detail */

/*! @struct smWindowHandle
    @brief Handle to a user interface window that can be used by the API.

    @see     
    - @ref sm_api_handles_detail */
#ifdef _WIN32
#ifndef _INC_WINDOWS
#define VC_EXTRALEAN
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif
typedef HWND smWindowHandle;
SM_API_DECLARE_HANDLE(smVideoDisplayHandle);
#else
#error "VideoDisplays not supported under this OS (yet)"
#endif

/*! @brief Creates the video window handle and gets the handle to the the window. 

    The engine draws video to this window. 
    The window is resizable, and has a minimum size of 320x240. This is also the default size.    
	@return @ref smReturnCode "SM_API_OK" if the window was created successfully. 
    @param engine_handle The engine to obtain video from.
	@param display_handle set to point to a new smVideoDisplayHandle.
    @param parent_window_handle The parent window of the video window. If you pass 0, the window will 
    appear as a top-level window, with it's own task-bar entry, otherwise, the window
    will be placed in the layout of the parent.
    @param show If non-zero, the VideoDisplay is also shown. If zero, the window will be hidden until manually shown.
    @pre Called from client's event loop thread, engine state >= @ref SM_API_ENGINE_STATE_IDLE
    @see smAPIProcessEvents() */
SM_API(smReturnCode) smVideoDisplayCreate(smEngineHandle engine_handle, smVideoDisplayHandle *display_handle, smWindowHandle parent_window_handle, int show);

/*! @brief Destroys the smVideoDisplayHandle.

    @param display_handle point to a valid smVideoDisplayHandle previously created with smVideoDisplayCreate 
    @return @ref smReturnCode "SM_API_OK" if display was destroyed successfully. */
SM_API(smReturnCode) smVideoDisplayDestroy(smVideoDisplayHandle *display_handle);

/*! @brief Set the parent window of the display.

    @param display_handle Must be an existing display created by smVideoDisplayCreate
    @param parent_handle Pointer to an parent window handle (a HWND under windows)
    @return @ref smReturnCode "SM_API_OK" if the parent was set successfully. */
SM_API(smReturnCode) smVideoDisplaySetParent(smVideoDisplayHandle display_handle, smWindowHandle parent_handle);

/*! @brief Show or hide the the display.

    @param display_handle Must be an existing display created by smVideoDisplayCreate
    @param show SM_API_TRUE shows the display. 
    @return @ref smReturnCode "SM_API_OK" if the window was shown / hidden ok. */
SM_API(smReturnCode) smVideoDisplayShow(smVideoDisplayHandle display_handle, smBool show);

/*! @brief Get the window handle of the smVideoDisplayHandle.

    @param display_handle Must be an existing display created by smVideoDisplayCreate
    @param window_handle Pointer to an existing smWindowHandle 
    @return @ref smReturnCode "SM_API_OK" if the window handle was retrieved successfully. */
SM_API(smReturnCode) smVideoDisplayGetWindowHandle(smVideoDisplayHandle display_handle, smWindowHandle *window_handle);

/*! @brief Masks for setting the video display options */
enum
{
    SM_API_VIDEO_DISPLAY_REFERENCE_FRAME    =0x01,  /*!< Shows the head coordinate-frame overlay */
    SM_API_VIDEO_DISPLAY_HEAD_MESH          =0x02,  /*!< Shows a mesh representation of the face */
    SM_API_VIDEO_DISPLAY_PERFORMANCE        =0x04,  /*!< Shows the CPU load and framerate */
    SM_API_VIDEO_DISPLAY_LANDMARKS          =0x08,  /*!< Shows the face landmarks being tracked */
    SM_API_VIDEO_DISPLAY_COLOR              =0x10   /*!< Shows the display in color if available. Warning, this consumes more CPU than grayscale. */
};

/*! @brief Sets the display status of various information shown on the video window.

    Set and clear the display flags using bitwise combinations of the values.
    @param display_handle The video_display to to change the display flags for.
    @param flags Value created from bitwise OR of the SM_API_VIDEO_DISPLAY_* flags.
	@return @ref smReturnCode "SM_API_OK" if the option was set successfully.
    @pre Must be called from client's event loop thread
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smVideoDisplaySetFlags(smVideoDisplayHandle display_handle, unsigned short flags);

/*! @brief Gets the display status of various information shown on the video window.

    @param display_handle The video_display to to change the display flags for.
    @param flags Pointer to existing unsigned short. The value pointed to is set using bitwise OR of the SM_API_VIDEO_DISPLAY_* flags.    
	@return @ref smReturnCode "SM_API_OK" if the flags were retreived successfully.
    @pre Must be called from client's event loop thread
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smVideoDisplayGetFlags(smVideoDisplayHandle display_handle, unsigned short *flags);

/*! @brief Status types returned by smVideoDisplayRecordingStatus */
enum smVideoDisplayRecordingStatusType
{
    SM_API_VIDEO_DISPLAY_RECORDING_STATUS_IDLE,
    SM_API_VIDEO_DISPLAY_RECORDING_STATUS_RECORDING,
    SM_API_VIDEO_DISPLAY_RECORDING_STATUS_COMPRESSING,
};

/*! @brief Starts recording the videodisplay output to a movie file.

    @param display_handle The video_display record.
    @param filepath The path and name of the file to record. Any file extension is ignored and replaced with ".avi". 
                    Any existing file is overwritten. The value is set to the absolute path and filename of the file that was
                    written (or attempted to be written if a SM_API_FAIL_WRITE_FILE error occured).
    @param compress If SM_API_TRUE the recording will be compressed in real-time to the file.
    @return @ref smReturnCode "SM_API_OK" if the option was set successfully, SM_API_FAIL_WRITE_FILE if the file could not be written.
    @pre Must be called from client's event loop thread
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smVideoDisplayRecordingStart(smVideoDisplayHandle display_handle, smStringHandle filepath, smBool compress);

/*! @brief Stops recording the videodisplay output and starts movie file compression.

    Does nothing unless the recording status is SM_API_VIDEO_DISPLAY_RECORDING_STATUS_RECORDING.
    
    @param display_handle The video_display record.        
	@return @ref smReturnCode "SM_API_OK" if the option was set successfully.
    @pre Must be called from client's event loop thread
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smVideoDisplayRecordingStop(smVideoDisplayHandle display_handle);

/*! @brief Cancels any recording/compression and returns the recorder to the idle status.

    Does nothing when called with the recording status SM_API_VIDEO_DISPLAY_RECORDING_STATUS_IDLE.

    @param display_handle The video_display record.        
	@return @ref smReturnCode "SM_API_OK" if the option was set successfully.
    @pre Must be called from client's event loop thread
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smVideoDisplayRecordingCancel(smVideoDisplayHandle display_handle);

/*! @brief Returns the status of the recorder.

    @param display_handle The video_display record.
    @param status Non-null output parameter to recieve a smVideoDisplayRecordingStatusType status.
    @return @ref smReturnCode "SM_API_OK" if the option was set successfully.
    @pre Must be called from client's event loop thread
    @pre engine state >= @ref SM_API_ENGINE_STATE_IDLE */
SM_API(smReturnCode) smVideoDisplayRecordingStatus(smVideoDisplayHandle display_handle, int* status);

#ifdef __cplusplus
}
#endif
#endif
