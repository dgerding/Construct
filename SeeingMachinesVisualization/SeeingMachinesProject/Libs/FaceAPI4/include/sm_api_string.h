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

#ifndef SM_API_STRING_H
#define SM_API_STRING_H

/*! @file
	Defines API string routines. All functions in this file are reentrant and threadsafe. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @struct smStringHandle
	@brief Used to send and receive strings to API routines.

	@see
	- @ref smStringCreate()    
	- @ref sm_api_handles_detail */
SM_API_DECLARE_HANDLE(smStringHandle);

/*! @brief Creates a new empty smStringHandle.

	This function is @a reentrant and @a threadsafe.
	@param string The address of an invalid smStringHandle, cannot be 0.
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringCreate(smStringHandle *string);

/*! @brief Determines if a string is valid (has been created and not destroyed)

	This function is @a reentrant and @a threadsafe.
	@param string A valid smStringHandle. 
	@param valid Set to SM_API_TRUE if string has been created and not destroyed, SM_API_FALSE otherwise. 
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringIsValid(smStringHandle string, smBool *valid);

/*! @brief Reads a buffer of characters into a smStringHandle. 

	This function is @a reentrant and @a threadsafe.

	Characters are read until the first termination character is reached or buf_len characters have been read.
	@param string A valid smStringHandle.
	@param buf User allocated memory buffer, may be 0 if buf_len is also 0 (this will set @a string to be empty)
	@param buf_len Size of memory buffer (bytes).
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringReadBuffer(smStringHandle string, const char *buf, size_t buf_len);

/*! @brief Reads a buffer of platform specific wide characters into a smStringHandle.

	This function is @a reentrant and @a threadsafe.

	The characters in the buffer are interpreted in unicode UTF-16 format on windows
	and UTF-32 on other platforms.
	Characters are read until the first termination character is reached or buf_len characters have been read.
	@param string A valid smStringHandle.
	@param buf User allocated memory buffer, may be 0 if buf_len is also 0 (this will set @a string to be empty)
	@param buf_len Size of memory buffer (words).
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully.
	@see
	- http://en.wikipedia.org/wiki/UTF-16 */
SM_API(smReturnCode) smStringReadBufferW(smStringHandle string, const wchar_t *buf, size_t buf_len);

/*! @brief Reads a buffer of UTF-16 unicode characters into a smStringHandle.

	This function is @a reentrant and @a threadsafe.

	Characters are read until the first termination character is reached or buf_len characters have been read.
	@param string A valid smStringHandle.
	@param buf User allocated memory buffer, may be 0 if buf_len is also 0 (this will set @a string to be empty)
	@param buf_len Size of memory buffer (words).
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully.
	@see
	- http://en.wikipedia.org/wiki/UTF-16 */
SM_API(smReturnCode) smStringReadBufferUtf16(smStringHandle string, const uint16 *buf, size_t buf_len);

/*! @brief Set buf to point to the the strings internal UTF-16 memory buffer, which is 0 terminated.

	This function is @a reentrant and @a threadsafe.
	@param string A Valid smStringHandle.
	@param buf Address of a pointer to char. The pointer is set by the routine to point to the internal memory buffer of the smStringHandle.
	@param buf_len A pointer to an size_t. The value of the size_t is set to the length of the buffer, including the terminator.
	@note The pointer will remain valid until the string contents are changed.
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. 
*/
SM_API(smReturnCode) smStringGetBufferUtf16(smStringHandle string, const uint16 **buf, size_t *buf_len);

/*! @brief Writes an ASCII 0 terminated string into the buffer.

	This function is @a reentrant and @a threadsafe.

	If the provided buffer @a buf is too small to hold the entire string, a truncated string is written.
	@param string A valid smStringHandle.
	@param buf User allocated memory buffer. If 0, this function does nothing.
	@param buf_len Size of the memory buffer (bytes). If 0, this function does nothing.
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully.
	@note smStringHandle uses unicode internally. This function interprets into ASCII but may produce non-printable ASCII characters. */
SM_API(smReturnCode) smStringWriteBuffer(smStringHandle string, char *buf, size_t buf_len);

/*! @brief Writes platform specific wide characters into the buffer, which is 0 terminated.

	This function is @a reentrant and @a threadsafe.

	The characters in the buffer are interpreted in unicode UTF-16 format on windows
	and UTF-32 on other platforms.
	If the provided buffer @a buf is too small to hold the entire string, a truncated string is written.
	@param string A valid smStringHandle.
	@param buf User allocated memory buffer. If 0, this function does nothing.
	@param buf_len Size of the memory buffer (words). If 0, this function does nothing.
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringWriteBufferW(smStringHandle string, wchar_t *buf, size_t buf_len);

/*! @brief Writes a unicode UTF-16 0 terminated string into the buffer.

	This function is @a reentrant and @a threadsafe.

	If the provided buffer @a buf is too small to hold the entire string, a truncated string is written.
	@param string A valid smStringHandle.
	@param buf User allocated memory buffer. If 0, this function does nothing.
	@param buf_len Size of the memory buffer (bytes). If 0, this function does nothing.
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully.
	@note smStringHandle uses unicode internally. This function interprets into ASCII but may produce non-printable ASCII characters. */
SM_API(smReturnCode) smStringWriteBufferUtf16(smStringHandle string, uint16 *buf, size_t buf_len);

/*! @brief Copies the @a src_string into @a dst_string.

	This function is @a reentrant and @a threadsafe.

	@param dst_string Destination string, must be valid.
	@param src_string Source string, must be valid.
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringCopy(smStringHandle dst_string, smStringHandle src_string);

/*! @brief String equality test.

	This function is @a reentrant and @a threadsafe.

	@param string1 A valid smStringHandle 
	@param string2 A valid smStringHandle 
	@param equal Pointer to an existing smBool. Set to SM_API_TRUE if the two strings contain equal characters, SM_API_FALSE otherwise.
	@note Two empty strings are considered equal.
	@return SM_API_FAIL_INVALID_ARGUMENT if either of the strings are invalid or @a equal is 0. */
SM_API(smReturnCode) smStringIsEqual(smStringHandle string1, smStringHandle string2, smBool *equal);

/*! @brief Test if string contains any characters.

	This function is @a reentrant and @a threadsafe.

	@param string A valid smStringHandle
	@param empty Set to SM_API_TRUE if @a string contains 0 characters, SM_API_FALSE if not empty.
	@return SM_API_FAIL_INVALID_ARGUMENT if @a string is invalid or @a empty is 0. */
SM_API(smReturnCode) smStringIsEmpty(smStringHandle string, smBool *empty);

/*! @brief Clear the contents of a string (make it empty).

	This function is @a reentrant and @a threadsafe.

	@param string A valid smStringHandle 
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringClear(smStringHandle string);

/*! @brief Gets the length of the string in characters, not including any termination character.

	This function is @a reentrant and @a threadsafe.

	@param string A valid smStringHandle
	@param len Set to the length of string. Must point to an existing integer.
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringLength(smStringHandle string, size_t *len);

/*! @brief Destroys an smStringHandle, making it invalid.

	This function is @a reentrant and @a threadsafe.

	Internal memory used for the string will be released. 
	@param string The address of a valid smStringHandle. 
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringDestroy(smStringHandle *string);

/*! @brief Reads a smUUID into a smStringHandle.

	This function is @a reentrant and @a threadsafe.

	The format of the string is: {HHHHHHHH-HHHH-HHHH-HHHH-HHHHHHHHHHHH}
	@param string A valid smStringHandle 
	@param uuid Pointer to smUUID to be read
	@return @ref smReturnCode "SM_API_OK" if the function completed successfully. */
SM_API(smReturnCode) smStringReadUUID(smStringHandle string, const smUUID *uuid);

#ifdef __cplusplus
}
#endif
#endif
