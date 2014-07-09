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

/*! @file

    Restores the byte packing of structures before the previous call to sm_api_pshpack.
    On non-Windows platforms nothing is done
*/
#ifdef SM_API_OS_WIN32
// pragma pack from Windows SDK headers
#include <PopPack.h>
#endif


