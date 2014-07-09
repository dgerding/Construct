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

#ifndef SM_API_H
#define SM_API_H


/*! @file
    The master include file for the Face Tracking API. */

// Compiler and OS configuration
// We can't include sm_api_pshpack before this include as some
// of the headers include by sm_api_configure break the pragma pack stack.
#include "sm_api_configure.h"

#include "sm_api_pshpack.h"
#include "sm_api_uuid.h"

// Basic includes
#include "sm_api_returncodes.h"
#include "sm_api_string.h"
#include "sm_api_logging.h"
#include "sm_api_geomtypes.h"
#include "sm_api_imagetypes.h"
#include "sm_api_image.h"
#include "sm_api_time.h"
#include "sm_api_facelandmark.h"
#include "sm_api_facetexture.h"

// Engine includes
#include "sm_api_cameratypes.h"
#include "sm_api_camera.h"
#include "sm_api_enginedatatypes.h"
#include "sm_api_engine.h"
#include "sm_api_enginecontrols.h"
#include "sm_api_videodisplay.h"
#include "sm_api_headtracker.h"
#include "sm_api_headtrackercontrols.h"
#include "sm_api_headtrackerv2controls.h"

// File and image tracking functions
#include "sm_api_facesearch.h"
#include "sm_api_filetrack.h"

// Utility functions
#include "sm_api_coordutils.h"
#include "sm_api_ar.h"

#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Gets the version of the API.

    This function is both @a reentrant and @a threadsafe
    @param major Major number. The API interface has changed you will need to recompile.
    @param minor Minor number. New interfaces may have been added to the API, but binary compatibility is maintained.
    @param maint Maintenance number. The API interface is unaltered. 
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully.
    @note 
    - This function can be called before smAPIInit() */
SM_API(smReturnCode) smAPIVersion(int *major, int *minor, int *maint);

/*! @return SM_API_TRUE if the API is a non-commercial license, SM_API_FALSE otherwise. 
    @see @ref sm_api_license */
SM_API(smBool) smAPINonCommercialLicense();

/*! @brief Gets a string describing the license status, including any time-trial expiry date.

    @param buff Pointer to user-allocated buffer of char, or 0 to determine maximum required length.
    @param size Must point to an existing integer. Set this to the size of your input buffer. 
           If @a buff is 0, is set to the required size for the string, otherwise set to the number of characters copied including trailing null character.
    @param detailed If true, a more detailed license string is generated.
    @return SM_API_OK if function executed successfully 
    @note 
    - This function can be called before smAPIInit()

    @code
    // C++ example
    char *buff;
    size_t size;
    smReturnCode error = smAPILicenseInfoString(0,&size,SM_API_FALSE);
    assert(!error);
    buff = new char[size];
    error = smAPILicenseInfoString(buff,&size,SM_API_FALSE);
    assert(!error);
    cout << "API license info: " << buff << std::endl;
    @endcode */
SM_API(smReturnCode) smAPILicenseInfoString(char *buff, size_t *size, smBool detailed);

/*! @brief Notify faceAPI that Qt is being used by the calling process. 

    This disables the internal use of Qt GUI classes. 
    
    This call will disable all video-display related functions such as smVideoDisplayCreate().

    @note This call must be made before smAPIInit() or it has no effect. 

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @return @ref smReturnCode "SM_API_FAIL_INVALID_CALL" if smAPIInit() has already been called. */
SM_API(smReturnCode) smAPIInternalQtGuiDisable();

/*! @brief Get status of internal Qt Gui disablement. 

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @see
    - smAPISetDisableInternalQtGui() */
SM_API(smBool) smAPIInternalQtGuiIsDisabled();

/*! @brief Notifies the API of it's location on the file-system.

    smAPIInit() attempts to determine the path of the shared library on the 
    filesystem so it can manually load the resource libraries containing trained 
    coefficients for detecting faces under IR or visible light conditions. 
    These resource libraries are expected to be in the same folder as the faceAPI library.

    Under some (unusual) circumstances the underlying OS calls can fail, which may
    result in an error when calling smAPIInit(). In that case this function may be used
    to manually set the path to the resource libraries. 
    
    This function must be called before smAPIInit() to have any effect. 
    
    @param path Null terminated string which is absolute path to location of the faceAPI library.
    @param buf_len Size of the buffer in characters. */
SM_API(smReturnCode) smAPISetPathW(const wchar_t *path, size_t buf_len);

/*! @brief Initializes the API.

    Must be called by the main thread of your application.

    Almost all other functions will return SM_API_FAIL_INVALID_CALL until this function is called.
    The current list of exceptions are:
    - smAPIVersion()
    - smAPINonCommercialLicense()
    - smAPILicenseInfoString()
    - smLoggingRegisterCallback()
    - smLoggingSetFileOutputEnable()
    - smLoggingGetFileOutputEnable()

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @return @ref smReturnCode "SM_API_FAIL_NO_LICENSE" if a valid license could not be determined.

    @note This function must be called the main thread for the process. 
          Calling this function will initialize COM for the thread as a 
          Single Threaded Apartment. At present the API may not work if COM has 
          already been initialized as MTA. This is due to the fact that Qt calls 
          OleInitialize which initializes COM as a STA to support clipboard, 
          drag and drop, OLE and in-place activation. 
          http://msdn.microsoft.com/en-us/library/ms690134%28v=vs.85%29.aspx */
SM_API(smReturnCode) smAPIInit();

/*! @brief For production license builds, use this function instead of smAPIInit(). 
    @param dummy This parameter is reserved for future releases. Pass in null. 
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
    @return @ref smReturnCode "SM_API_FAIL_NO_LICENSE" if a valid license could not be determined. */
SM_API(smReturnCode) smAPIInitProductionLicense(const char *dummy);

/*! @brief Call at the end of your program to clean up API resources

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smAPIQuit();

/*! @brief Manually process any window events

    This is only required for console-style applications which
    need to show a video display window and that do not have an 
    existing window event loop. It may also be called to manually
    update the video window when an application is showing a modal
    dialog.

    <b>Example code:</b>
    @code
    void main()
    {
        // Initialize API...
        // Create and start an engine...
        // Create a video-display
        while (!quit)
        {
            // Perhaps check if the ESC key has been hit
            ...
            // Refresh video display
            smAPIProcessEvents();
            // Prevent CPU overload in this loop
            Sleep(30);
        }
    }
    @endcode 

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smAPIProcessEvents();

/*! @brief If a VideoDisplay is shown, then this function needs to be called before entering a modal event loop.
    
    <b>Example code:</b>
    @code
    smAPIEnterModalLoop();
    MessageBox(...);
    smAPIExitModalLoop();
    @endcode
    
    @see
    - smAPIExitModalLoop() 

    @return @ref smReturnCode "SM_API_OK" if the function completed successfully.

    @platform On non-windows platforms this function is does nothing and will always return @ref smReturnCode "SM_API_OK". */
SM_API(smReturnCode) smAPIEnterModalLoop();

/*! @brief If a VideoDisplay is shown, then this function needs to be called after exiting a modal event loop.

    <b>Example code:</b>
    @code
    smAPIEnterModalLoop();
    MessageBox(...);
    smAPIExitModalLoop();
    @endcode
    
    @see
    - smAPIEnterModalLoop() 
    
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. 

    @platform On non-windows platforms this function is does nothing and will always return @ref smReturnCode "SM_API_OK". */
SM_API(smReturnCode) smAPIExitModalLoop();

/*! @brief Set the default icon for all windows that faceAPI can create. 
    @param icon_png_filename Path and filename to a PNG file.
    @return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smAPISetIconFromPNG(smStringHandle icon_png_filename);

/*! @brief Call this function when your program receives a notification that the OS is about to sleep / hibernate.
    
    This ensures that faceAPI will operate normally when the system is resumed.

    @return @ref smReturnCode "SM_API_OK" if the function executed without error. 
    @see 
    - smAPINotifySystemWake() */
SM_API(smReturnCode) smAPINotifySystemSleep();

/*! @brief Call this function when your program receives a notification that the OS is waking from sleep / hibernation.
    
    This ensures that faceAPI will resume normally.

    @return @ref smReturnCode "SM_API_OK" if the function executed without error.
    @see 
    - smAPINotifySystemSleep() */
SM_API(smReturnCode) smAPINotifySystemWake();

#ifdef __cplusplus
}
#endif

#include "sm_api_poppack.h"

#endif
