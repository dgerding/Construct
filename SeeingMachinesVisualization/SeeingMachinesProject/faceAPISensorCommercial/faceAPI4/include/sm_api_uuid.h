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

#ifndef SM_API_UUID_H
#define SM_API_UUID_H

/*! @file
    Defines UUID type used in faceAPI. */
#ifdef __cplusplus
extern "C"
{
#endif
/*! @brief Defines the UUID type used in faceAPI.

    See http://en.wikipedia.org/wiki/Universally_unique_identifier
*/
typedef struct smUUID {
  uint32 data1;
  uint16 data2;
  uint16 data3;
  uint8  data4[8];
} smUUID;

/*! @brief Clear the smUUID memory.

    @param srcdst Pointer to a smUUID to be cleared.
            If @a srcdst is NULL then nothing is done.
*/
SM_API(void) smUUIDClear(smUUID *srcdst);

/*! @brief Compare two smUUID

    @param arg1 Pointer to a smUUID to be compared.
    @param arg2 Pointer to a smUUID to be compared.
    @return 0 if equal or arg1 is NULL or arg2 is NULL.
    @return 1 if arg1 > arg2
    @return -1 if arg1 < arg2
*/
SM_API(int) smUUIDCompare(const smUUID *arg1, const smUUID *arg2);

#ifdef __cplusplus
}
#endif
#endif
