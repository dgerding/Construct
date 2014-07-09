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

#ifndef SM_API_RETURN_CODES_H
#define SM_API_RETURN_CODES_H

/*! @file
    Defines the set of possible return values from API routines.
    All failure codes are guaranteed to be < 0. All API functions return >=0 when successful. 

    @see Macros SM_API_FAILED and SM_API_SUCCEEDED.
*/
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Predefined values returned by API functions. 

    Note that errors are always negative values. 
    Some functions return positive values as valid results. 

    @see Macros SM_API_FAILED and SM_API_SUCCEEDED.
*/
typedef enum smReturnCode
{
    SM_API_OK                           =0,     /*!< API function call was successful. */
    // Errors
    SM_API_FAIL_INVALID_CALL            =-1,    /*!< Function is not valid or unsupported. */
    SM_API_FAIL_INVALID_ARGUMENT        =-2,    /*!< A bad parameter was passed. */
    SM_API_FAIL_CPU_INADEQUATE          =-3,    /*!< CPU features are inadequate for the function. */
    SM_API_FAIL_MEMORY_INADEQUATE       =-4,    /*!< Not enough memory for the function. */
    SM_API_FAIL_OUT_OF_MEMORY           =-5,    /*!< Ran out of memory while calling the function. */
    SM_API_FAIL_OPEN_FILE               =-6,    /*!< A file could not be found or opened. */
    SM_API_FAIL_PARSE_FILE              =-7,    /*!< A file format was not recognized. */
    SM_API_FAIL_WRITE_FILE              =-8,    /*!< A file or folder could not be created or written. */
    SM_API_FAIL_CAMERA_ERROR            =-9,    /*!< Could not find / initialize a camera. */
    SM_API_FAIL_NO_LICENSE              =-10,   /*!< The license is invalid. */
    SM_API_FAIL_INVALID_ENGINE_STATE    =-11,   /*!< The engine state is invalid for the function call. */
    SM_API_FAIL_STD_ERROR               =-12,   /*!< Error using the C++ std library */
    SM_API_FAIL_UNSPECIFIED             =-13,   /*!< An unknown error occurred. Check the application log. */
    SM_API_FAIL_PROGRAMMING_ERROR       =-14,   /*!< An internal check detected an invalid logic condition. */
    SM_API_FAIL_CORE_EXCEPTION          =-15,   /*!< A core-library exception occurred. */
    SM_API_FAIL_CAMERA_NOT_STARTED      =-16,   /*!< The camera is not started. */
    SM_API_FAIL_CAMERA_IMAGE_PUSH       =-17,   /*!< Failed to push image into camera. */
    SM_API_FAIL_UNITIALIZED             =-18,   /*!< The API is not initialized. */
    SM_API_FAIL_BUFFER_TOO_SMALL        =-19,   /*!< Supplied data buffer is too small. */
    SM_API_FAIL_CAMERA_HAS_STARTED      =-20,   /*!< The camera has already been started. */
    SM_API_FAIL_TIMEOUT                 =-21,   /*!< The function timed out waiting for data */
    SM_API_FAIL_NODATA                  =-22,   /*!< The function failed because no data was available */
    SM_API_FAIL_HARDWARE_INADEQUATE     =-23,   /*!< The function requires special hardware that is not present */
    SM_API_FAIL_NO_WIDGET_PARENTING     =-24    /*!< The current platform doesn't support parenting widgets */
} smReturnCode;

/*! @brief Macro to check that an @ref smReturnCode indicates failure

<b>Example code:</b>
@code
if (SM_API_FAILED(smAPIInit())
{
    exitAppWithMessage();
}
@endcode
@see Macro SM_API_SUCCEEDED.
*/
#define SM_API_FAILED(code) ((code) < 0)

/*! @brief Macro to check that an @ref smReturnCode indicates success

<b>Example code:</b>
@code
if (SM_API_SUCCEEDED(smAPIInitProductionLicense(NULL))
{
    enableProductionLicenseFeatures();
}
@endcode
@see Macro SM_API_FAILED.
*/
#define SM_API_SUCCEEDED(code) ((code) >= 0)

#ifdef __cplusplus
}
#endif
#endif
