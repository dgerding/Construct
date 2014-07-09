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

#ifndef SM_API_LOGGING_H
#define SM_API_LOGGING_H

/*! @file 
    Defines routines for handling API log messages. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief Signature of function for obtaining logging messages from API functions.

    Define your own function matching this interface and register using smRegisterLoggingCallback() 
    @param user_data Value passed into @a user_data argument in smLoggingRegisterCallback(). 
    @param buf 0 (null) terminated buffer of char. Do not delete / free. 
    @param buf_len Size of the buffer (including the trailing @c '\\0'). 
    @note
    - To construct a std::string from this data you would construct with std::string(buf)
    - This function can be called before smAPIInit() */
typedef void (STDCALL *smLoggingCallback)(void *user_data, const char *buf, size_t buf_len);

/*! @brief Registers logging callback function. 

    Only one function can be registered at a time.
    @param user_data Can be 0. This value is passed back to your callback routine. 
    It is typically used to pass the 'this' pointer of an object to enable the callback 
    to call a member function in the object.
	@param callback_fn Address of callback function, or 0 to unregister the callback.
    @note
    - This function can be called before smAPIInit()
	@return @ref smReturnCode "SM_API_OK" if callback was registered successfully. */
SM_API(smReturnCode) smLoggingRegisterCallback(void *user_data, smLoggingCallback callback_fn);

/*! @brief Set logging to a file enable / disable.

    Logging to file is disabled by default.
    @param enable Set to non-zero to enable logging to a file.
    @note
    - This function can be called before smAPIInit()
    @return @ref smReturnCode "SM_API_OK" if the logging was enabled successfully.
    @see 
    - smLoggingGetFileOutputEnable() 
    - smLoggingGetPath() 
    - smLoggingSetPath() */
SM_API(smReturnCode) smLoggingSetFileOutputEnable(int enable);

/*! @brief Get logging to a file enable / disable.

    Logging to file is disabled by default.
    @param enable Pointer to an existing int. Set to SM_API_FALSE when logging to file is disabled, SM_API_TRUE when enabled.
    @note
    - This function can be called before smAPIInit()
    @return @ref smReturnCode "SM_API_OK" if the value was retreived successfully.
    @see 
    - smLoggingSetFileOutputEnable() 
    - smLoggingGetPath() 
    - smLoggingSetPath() */
SM_API(smReturnCode) smLoggingGetFileOutputEnable(int *enable);

/*! @brief Gets the path where logfiles are stored.

    @param path Handle to an existing smStringHandle created with smStringCreate(). Set to the path where logfiles are created.
    @note
    - The API must be initialized before this function can be called.
    @return @ref smReturnCode "SM_API_OK" if the path was obtained successfully.
    @see 
    - smLoggingSetPath() 
    - smLoggingSetFileOutputEnable() */
SM_API(smReturnCode) smLoggingGetPath(smStringHandle path);

/*! @brief Sets path to directory where logfiles are created.

    The default path is "C:\Documents and Settings\<Username>\Application Data\Seeing Machines\Temp\Run Logs"
    @param path Path to directory where logfiles will be created.
    @note
    - The API must be initialized before this function can be called.
	@return @ref smReturnCode "SM_API_OK" if the path was set successfully. */
SM_API(smReturnCode) smLoggingSetPath(smStringHandle path);

#ifdef __cplusplus
}
#endif
#endif
