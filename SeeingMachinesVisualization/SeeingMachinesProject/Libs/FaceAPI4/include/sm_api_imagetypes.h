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

#ifndef SM_API_IMAGETYPES_H
#define SM_API_IMAGETYPES_H


/*! @file
    Defines types for images. */
#ifdef __cplusplus
extern "C"
{
#endif

/*! @brief The maximum possible number of image memory planes in an smImage.*/
#define SM_API_IMAGE_MAX_PLANES 4

/*! @brief Possible types (formats) of images that the API can work with. */
typedef enum smImageCode
{
    SM_API_IMAGECODE_GRAY_8U  = 0x30303859, /*!< 8-bit per-pixel grayscale. */	
    SM_API_IMAGECODE_GRAY_16U = 30,         /*!< 16-bit per-pixel grayscale. */
	SM_API_IMAGECODE_YUY2     = 0x32595559, /*!< 16-bit per-pixel packed YUV 4:2:2 format. @see http://www.fourcc.org/yuv.php#YUY2 */
	SM_API_IMAGECODE_I420     = 0x30323449, /*!< 12-bit per-pixel planar YUV 4:2:2 format. @see http://www.fourcc.org/yuv.php#IYUV */
	SM_API_IMAGECODE_BGRA_32U = 11,         /*!< 32-bit per-pixel BGRA. Byte ordering compatible with Windows 32-bit RGB bitmaps and OpenGL GL_RGBA. */
    SM_API_IMAGECODE_ARGB_32U = 12,         /*!< 32-bit per-pixel ARGB. */
	SM_API_IMAGECODE_BGR_24U  = 20,         /*!< 24-bit per-pixel BGR. Byte ordering compatible with Windows 24-bit RGB bitmaps. */
    SM_API_IMAGECODE_RGB_24U  = 21,         /*!< 24-bit per-pixel RGB. */	
    SM_API_NUM_IMAGE_CODES    = 8
} smImageCode;

/*! @brief Pixel origin is the center of the top-left pixel of an image. (x,y) == (column,row) */
typedef smCoord2f smPixel;

/*! @brief Defines a rectangular region within an image. */
typedef struct smImageRect
{
    smCoord2i top_left;     /*!< The top-left coordinate of the rectangle, with (0,0) being the first pixel in an image. */
    smSize2i size;          /*!< The size of the rectangle. Values greater than 0 indicate a valid rectangle. */
} smImageRect;

/*!@struct smImageHandle
   @brief Handle to an image that can be used by the API.
    
    @see
    - @ref smImageCreateFromInfo()
    - @ref smImageCreateFromPNG()    
    - @ref sm_api_handles_detail */
SM_API_DECLARE_HANDLE(smImageHandle);

/*! @brief Memory copying - deep, try to share, or automatically choose. */
typedef enum smImageMemoryCopyMode
{
    SM_API_IMAGE_MEMORYCOPYMODE_DEEP = 0,     /*!< Always deep copy the image memory. */
    SM_API_IMAGE_MEMORYCOPYMODE_SHARED = 1,   /*!< Attempt to share memory (i.e shallow copy). FAILS with an error if not possible due to incorrect alignment. */
    SM_API_IMAGE_MEMORYCOPYMODE_AUTO = 2      /*!< Share memory when alignment is ok, otherwise deep copy. This is the preferred option. */
} smImageMemoryCopyMode;

/*! @brief Describes pixel-format, size and memory of an image.

    All images used internally by the API are 32 byte aligned, meaning that
    the address of each scanline (row) is on a 32-byte address boundary.

    You can create images from your own data using smImageCreateFromInfo() 
    in which case the function will detect incorrect alignment and deep copy
    the memory, realigning the data. However if your memory is correctly
    aligned then the memory can be (optionally) shared with the API avoiding
    expensive image copying. @see smImageCreateFromInfo() for details.

    Most images consist of a single memory block, however up to 4 channel planar images are supported.
    - @a plane_addr is a vector of pointers to the start address of each plane. Set to 0 for unused planes.
    - For single-plane images, only @a plane_addr[0] and @a step_bytes[0] are used.
    
    @see
    - smImageCode for the number of bytes per pixel for each format. */
typedef struct smImageInfo
{
    smImageCode format;                                     /*!< The image format. @see smImageCode. */
    smSize2i res;                                           /*!< The resolution (width, height) of the image in pixels. */   
    int step_bytes[SM_API_IMAGE_MAX_PLANES];                /*!< The number of bytes per scanline (row), including any memory padding, for each image plane. The value is negative if the image is upside down. */
    uint8* plane_addr[SM_API_IMAGE_MAX_PLANES];             /*!< Pointers to the start address of image data for each image plane. This is the end of the memory block if the image is upside down.*/
    void *user_data;                                        /*!< You can optionally set this to the 'this' pointer of an object that owns image data.
                                                                 If the image is generated by the API, then this value is always 0. */
} smImageInfo;

/*! @brief Signature of function for being notified when an image that is using shared memory, is no longer in use by the API.
    
    Define your own function matching this interface and register it using smRegisterImageReleaseCallback() to be notified that 
    image data that was passed into smImageCreateFromInfo() is no longer in use by any API internal routines.
    Typically, this function is useful for freeing the image memory.
    @param image_info The info that was originally passed into smImageCreateFromInfo().
    @see smRegisterImageReleaseCallback() */
typedef void (STDCALL *smImageReleasedCallback)(smImageInfo image_info);

#ifdef __cplusplus
}
#endif


#endif

