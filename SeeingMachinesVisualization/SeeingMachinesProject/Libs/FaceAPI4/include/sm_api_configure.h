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

#ifndef SM_API_CONFIGURE_H
#define SM_API_CONFIGURE_H

/*! @file
    Defines OS and compiler specific types and macros. */

// Detect OS
#if !defined(SAG_COM) && (defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__))
#    define SM_API_OS_WIN32
#elif defined(linux) || defined(__linux)
#    define SM_API_OS_LINUX
#elif defined(__APPLE__)
#	 include "TargetConditionals.h"
#    if defined(TARGET_OS_MAC)
#        define SM_API_OS_MAC
//#    elif defined(TARGET_OS_IPHONE) || defined(TARGET_IPHONE_SIMULATOR)
//#        define SM_API_OS_IOS
#    else
#    	error "Library only compatible with Windows, Linux & Mac OSX"
#    endif
#else
#    error "Library only compatible with Windows, Linux & Mac OSX"
#endif

// Detect Compiler
#if defined(_MSC_VER)
#    define SM_API_CC_MSVC
#elif defined(__GNUC__)
#    define SM_API_CC_GNU
#elif defined(__INTEL_COMPILER)
#    define SM_API_CC_INTEL
#endif

// Detect CPU architecture
#if defined(_M_X64)
    #define SM_API_ARCH_X64
#elif (defined(SM_API_CC_MSVC) && defined(_M_IX86)) || (defined(SM_API_CC_GNU) && defined(__i386__)) || (defined(SM_API_CC_INTEL) && defined(_M_IX86) && !defined(_M_IA64))
    #define SM_API_ARCH_X86
#elif (defined(SM_API_CC_GNU) && defined(__x86_64__))
    #define SM_API_ARCH_X64
#endif
#if !defined(SM_API_ARCH_X86) && !defined(SM_API_ARCH_X64)
#error "Unable to determine CPU architecture"
#endif

#ifdef SM_API_CC_MSVC
#   define STDCALL __stdcall
#else
#   define STDCALL __attribute__((stdcall))
#endif

// Internal or external build
#if defined(SM_API_OS_WIN32)
#    ifdef SM_API_DLL_EXPORT
#        define SM_API(type) __declspec(dllexport) type STDCALL
#    else
#        define SM_API(type) __declspec(dllimport) type STDCALL
#    endif
#else
#    ifdef SM_API_DLL_EXPORT
#       if defined(SM_API_OS_LINUX) && defined(SM_API_CC_GNU) && defined(SM_API_ARCH_X86)
            /*
                Workaround mixed ABIs on Linux where gcc assumes 16 byte aligned stack for SSE
                instructions but some libraries, such as python ctypes, use 4 byte alignment.
            */
#           define SM_API(type) __attribute__((visibility ("default"))) __attribute__((force_align_arg_pointer)) type
#       else
#           define SM_API(type) __attribute__((visibility ("default"))) type
#       endif
#    else
#        define SM_API(type) type
#    endif
#endif

// Integer types
#ifdef SM_API_CC_MSVC
typedef unsigned __int64 uint64;
typedef __int64 int64;
typedef unsigned __int32 uint32;
typedef __int32 int32;
typedef unsigned __int16 uint16;
typedef __int16 int16;
typedef __int8 int8;
typedef unsigned __int8 uint8;
#else
#include <stdint.h>
typedef uint64_t uint64;
typedef int64_t int64;
typedef uint32_t uint32;
typedef int32_t int32;
typedef uint16_t uint16;
typedef int16_t int16;
typedef uint8_t uint8;
typedef int8_t int8;
#endif

/*! @brief Boolean type. 0 is false, non-zero is true. */
typedef int smBool;
#define SM_API_FALSE 0
#define SM_API_TRUE 1

/*! @brief Macro declaring a handle to an opaque type exposed by the API.
    
    Each declared handle is a pointer to a unique structure. The compiler is therefore able to check that handles for
    one type of object are not passed to functions requiring handles of a different type.
    
    @see
    - @ref sm_api_handles_detail
*/
#define SM_API_DECLARE_HANDLE(name) \
    struct name##__ { int unused; }; \
    typedef struct name##__* name

#define SM_API_STATIC_ASSERT(expr,dummy) static char dummy[(expr) ? 1 : 0];

#ifdef __cplusplus
#include <cstddef>
#else
#include <stddef.h>
#endif

#if defined(SM_API_CC_MSVC)
#	define SM_API_DEPRECATED(text_message) __declspec(deprecated(text_message))
#elif defined(SM_API_CC_GNU) 
#	define SM_API_DEPRECATED(text_message) __attribute__ ((deprecated))
#else
#	define SM_DEPRECATED(text_message) 
#endif

#endif


