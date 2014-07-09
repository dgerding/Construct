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

#ifndef SM_API_FACESEARCH_H
#define SM_API_FACESEARCH_H


/*! 
    @file
    Functions and types for locating faces in images or short image sequences. 
*/
#ifdef __cplusplus
extern "C"
{
#endif

#define SM_API_FACE_SEARCH_MAX_NUM_FACES       100  /*!< The maximum number of faces that can be searched for. */
#define SM_API_FACE_SEARCH_MIN_FACE_WIDTH_PIXELS 24 /*!< The minimum size of a face in pixels that can be searched for. */
#define SM_API_FACE_SEARCH_MAX_SEQUENCE_LENGTH  20  /*!< The maximum image sequence length that can be passed to smFaceSearch() */

/*! @brief Defines the intensity of the search routine, 
    with higher levels taking longer to process but delivering 
    more fine-grained information. */
typedef enum smFaceSearchLevel
{
    SM_API_FACE_SEARCH_LEVEL_0 = 0,     /*!< Approximately locates each face in the image or image sequence. */
    SM_API_FACE_SEARCH_LEVEL_1 = 1,     /*!< More accurately locates each face in the image or image sequence. */
    SM_API_FACE_SEARCH_LEVEL_MAX = 1    /*!< Highest search level */
} smFaceSearchLevel;

// TODO replace smFaceSearchResult with smEngineData

/*! @brief Defines search result information for a single face. */
typedef struct smFaceSearchResult
{
    unsigned int image_index;           /*!< The index of the image in the sequence where the face was located. */
    smEngineHeadPoseData head_pose;     /*!< The position and orientation of the head in 3D. */
    smEngineFaceData face_data;         /*!< Face landmark locations in face-coordinates and optional face texture for the face. */
} smFaceSearchResult;

/*! @brief Defines search result information for multiple faces. */
typedef struct smFaceSearchResultList
{
    unsigned int num_faces_found;       /*!< The number of faces found by the search, set by the smFaceSearch() routine. Can be 0. */
    smFaceSearchResult *result;         /*!< Array of smFaceSearchResult set by the smFaceSearch() routine. There is one result for each face. */
    void *__vec;                        /*!< Internal use only. */
} smFaceSearchResultList;

/*! @brief Flag value to indicate that the face width pixels constraints are valid.
    If <code>flags & SM_API_FACESEARCHCONSTRAINT_USE_FACE_WIDTH_PIXELS</code> is not 0 then
    the face searcher will use the @ref smFaceSearchConstraints::min_face_width_pixels "min_face_width_pixels"
    and @ref smFaceSearchConstraints::max_face_width_pixels "max_face_width_pixels" values.
*/
#define SM_API_FACESEARCHCONSTRAINT_USE_FACE_WIDTH_PIXELS 1

/*!
    @brief Constraints used to narrow the search space for finding faces.
*/
typedef struct smFaceSearchConstraints
{
    unsigned int byte_size; /*!< The size of the structure in bytes. Used for extending the structure in the future. */
    unsigned int flags;     /*!< Bit flags indicating what fields are present. @see SM_API_FACESEARCHCONSTRAINT_USE_FACE_WIDTH_PIXELS. */

	float max_roll_deg;     /*!< Maximum roll of face in the @ref sm_api_coord_frames_standard_camera "camera coordinate frame" that a face may have and still be found. */
	float max_pitch_deg;    /*!< Maximum pitch of face in the @ref sm_api_coord_frames_standard_camera "camera coordinate frame" that a face may have and still be found. */
	float max_yaw_deg;      /*!< Maximum yaw of face in the @ref sm_api_coord_frames_standard_camera "camera coordinate frame" that a face may have and still be found. */

	unsigned int min_face_width_pixels; /*!< How small a face can be in the image and still be found. Only used when @ref smFaceSearchConstraints::flags "flags" contains @ref SM_API_FACESEARCHCONSTRAINT_USE_FACE_WIDTH_PIXELS "SM_API_FACESEARCHCONSTRAINT_USE_FACE_WIDTH_PIXELS". Tighter bounds will allow faster searching. */
	unsigned int max_face_width_pixels; /*!< How big a face can be in the image and still be found. Only used when @ref smFaceSearchConstraints::flags "flags" contains @ref SM_API_FACESEARCHCONSTRAINT_USE_FACE_WIDTH_PIXELS "SM_API_FACESEARCHCONSTRAINT_USE_FACE_WIDTH_PIXELS". Tighter bounds will allow faster searching. */

    float min_face_range_m; /*!< Minimum face range in meters from the camera. Tighter bounds will allow faster searching. */
    float max_face_range_m; /*!< Maximum face range in meters from the camera. Tighter bounds will allow faster searching. */

    float detection_threshold_level0; /*!< Minimum confidence of a level 0 facesearcher result, range is [0,1]. Lowering this value will increase the number of false positives. */

} smFaceSearchConstraints;

/*!
    @brief Initializes a smFaceSearchConstraints structure with default values.

    @param image_size The size of the image.
    @param lens_params The lens parameters to use for calculating @ref smFaceSearchConstraints::min_face_range_m "min_face_range_m"
        and @ref smFaceSearchConstraints::max_face_range_m "max_face_range_m". This may be NULL in which case 
        the lens parameters returned from smCameraLensParamsFromHFOV() with the supplied @a image_size and
        a 55 degree FOV will be used.
    @param constraints_size The size of the smFaceSearchConstraints structure. This
        is required for extending the size of the structure in future versions.
    @param constraints The non-NULL pointer to the smFaceSearchConstraints structure.
	@return @ref smReturnCode "SM_API_OK" if the function succeeded.

    <b>Example code:</b>
    @code
    smFaceSearchConstraints constraints;
    return_code = smFaceSearchConstraintsInitialize(image_size, lens_params, sizeof(constraints), &constraints);
    @endcode
*/
SM_API(smReturnCode) smFaceSearchConstraintsInitialize(
    smSize2i image_size, 
    const smCameraLensParams* lens_params, 
    size_t constraints_size,
    smFaceSearchConstraints* constraints);

/*!
    @brief Options to configure the results of the face searcher.
*/
typedef struct smFaceSearchResultOptions
{
    unsigned int byte_size; /*!< The size of the structure in bytes. Used for extending the structure in the future. */
    smBool detect_glasses;  /*!< When non-zero the face searcher will try to detect glasses. Setting to 0 will allow faster searching. */
    smBool search_lips;     /*!< When non-zero and using a search level greater than @ref smFaceSearchLevel::SM_API_FACE_SEARCH_LEVEL_0 "SM_API_FACE_SEARCH_LEVEL_0", the face searcher will try to search for lips. Setting to 0 will allow faster searching. */
    smBool search_eyebrows; /*!< When non-zero and using a search level greater than @ref smFaceSearchLevel::SM_API_FACE_SEARCH_LEVEL_0 "SM_API_FACE_SEARCH_LEVEL_0", the face searcher will try to search for eyebrows. Setting to 0 will allow faster searching. */
    unsigned int max_faces; /*! The maximum number of faces to return. Cannot be greater than SM_API_FACE_SEARCH_MAX_NUM_FACES. */
    smBool generate_face_texture; /*! When non-zero face texture will be generated. */
    smSize2i face_texture_size; /*! Size of the face texture when generate_face_texture is non-zero. */
} smFaceSearchResultOptions;

/*!
    @brief Initializes a smFaceSearchResultOptions structure with default values.

    @param options_size The size of the smFaceSearchResultOptions structure. This
        is required for extending the size of structure in the future versions.
    @param options The non-NULL pointer to the smFaceSearchResultOptions structure.
	@return @ref smReturnCode "SM_API_OK" if the function succeeded.

    <b>Example code:</b>
    @code
    smFaceSearchResultOptions options;
    return_code = smFaceSearchResultOptionsInitialize(sizeof(options), &options);
    @endcode
*/
SM_API(smReturnCode) smFaceSearchResultOptionsInitialize(
    size_t options_size,
    smFaceSearchResultOptions* options);

/*! @brief Search for faces in an image sequence. 

    This function searches a set of images that are consecutive in time for faces.
    
	The function is able to search on two levels of refinement, see @ref smFaceSearchLevel "smFaceSearchLevel".
    - Level-0 runs a fast face detector that can locate a face region and approximate the size and orientation of that region
    and provides a rough 3D solution.
    - Level-1 runs an algorithm that attempts to locate the interior facial landmarks of each face found by level-0. If the 
      landmarks cannot be located, the face is not "found". Therefore level-1 detects less faces than level-0, but if a
      face is found, then it is found much more accurately in 3D.

    @par Infrared vs Visible Coefficients
    The algorithm uses face-detection coefficients that are located in resource files in the API binary distribution.
    These resources files are described in the documentation API\doc\binaries.txt.
    The first time the function is called, the local folder is searched for the presence of coefficient resource files.
    If no resources files are found in the local folder, the function will return SM_API_FAIL_OPEN_FILE.

    There are four sets of coefficients files to consider. There should be at most 2 of these files copied into the output folder.
    Visible light:
    - sm_ca_resource_facesearchv2_level0_cdefault_<version>: Required for Level-0 and Level-1 search.
    - sm_ca_resource_facesearchv2_level1_cdefault_<version>: Required for Level-1 search.
    Infrared light:
    - sm_ca_resource_facesearchv2_level0_ir_<version>: Required for Level-0 and Level-1 search.
    - sm_ca_resource_facesearchv2_level1_ir_<version>: Required for Level-1 search.

    Only copy the resource files you need, no more. 
    So for example, under Windows, if you are using infrared images and only need to run level-0, 
    there should only be one resource file in the local folder: sm_ca_resource_facesearchv2_level0_ir_7.0.dll.

    @param image_sequence Must point to an existing array of @a image_sequence_size smImageHandle handles. 
                All images must be the same resolution.
                See smImageCreateFromPNG() and smImageCreateFromInfo() 
    @param image_sequence_size Length of the @a image_sequence array. 
                Sequence length must be between [1, SM_API_FACE_SEARCH_MAX_SEQUENCE_LENGTH].
    @param frame_period_ms Number of milliseconds between frames in @a image_sequence. Must be less than 100ms. If there is only 1 image in the sequence set this to 0.
    @param search_level The level to refine the search results.
    @param constraints Optional parameters used to constrain the search-space of 
                the algorithm and improve performance (recommended).
                May be <code>NULL</code> in which case defaults values are used.
                See smFaceSearchConstraintsInitialize().
    @param result_options Optional parameters use to specify the algorithm output.
                May be <code>NULL</code> in which case defaults values are used.
                See smFaceSearchResultOptionsInitialize().
    @param lens_params The lens parameters. May be <code>NULL</code> in which case a default lens field-of-view of 55 degrees is used.
                Errors in the field-of-view of the lens equate to systematic offsets in the 3D output measurements.
                See smCameraLensParamsFromHFOV().
    @param face_results Must point to an existing smFaceSearchResultList. 
            The contents of this structure are assigned by the routine.
            If no faces were found, the @ref smFaceSearchResultList::num_faces_found "num_faces_found" field is set to 0 and 
            the @ref smFaceSearchResultList::result "result" array is set to <code>NULL</code>.
    @return 
        - SM_API_OK if the function executed successfully. 
        - SM_API_FAIL_OPEN_FILE if no coefficient resources files are found in the local folder.
    @note 
    - If faces were found, when you are done with the list of results, you must call smFaceSearchResultListDestroy() to free internal resources.
    - @a face_results will not contain face texture images or face texture coordinate data. I.e. for each smFaceSearchResult, 
        smEngineFaceData::texture == <code>NULL</code>, and smFaceLandmark::ftc == {0,0,0}.
	
	@see
	- smFaceSearchLevel
    - smFaceSearchConstraintsInitialize()
    - smFaceSearchResultOptionsInitialize()
    - smCameraLensParamsFromHFOV()
	- smImageCreateFromPNG()
	- smImageCreateFromInfo() */
SM_API(smReturnCode) smFaceSearch(
    const smImageHandle image_sequence[],
    size_t image_sequence_size,
    unsigned int frame_period_ms,
    smFaceSearchLevel search_level,
    const smFaceSearchConstraints* constraints,
    const smFaceSearchResultOptions* result_options,
    const smCameraLensParams *lens_params,
    smFaceSearchResultList* face_results);

/*! @brief Frees memory allocated by a call to smFaceSearch()

    @param face_results must have been passed into a successful call to smFaceSearch() before being passed to this routine.
    @return @ref smReturnCode "SM_API_OK" if the results were destroyed successfully. */
SM_API(smReturnCode) smFaceSearchResultListDestroy(smFaceSearchResultList* face_results);

/*! @brief Determines minimum and maximum possible ranges for finding faces given an image size and
    lens parameters. This function applies to smFaceSearch as well as the head-tracking engines. The
    function smHTGetTrackingRangeBounds() is for convenience and internally uses this function.

    @param image_size Resolution of the image in pixels that will be used to search for faces.
    @param lens_params Lens coefficients for the image.
    @param min Must point to an existing float. Set to the minimum possible range in meters.
    @param max Must point to an existing float. Set to the maximum possible range in meters.

    @see
    - smCameraLensParamsFromHFOV()
    - smCameraLoadLensParamsFile()
    - smHTGetTrackingRanges()
    - smHTGetTrackingRangeBounds()

	@return @ref smReturnCode "SM_API_OK" if the values were read successfully. */
SM_API(smReturnCode) smFaceSearchGetRangeBounds(smSize2i image_size, smCameraLensParams lens_params, float *min, float *max);

#ifdef __cplusplus
}
#endif


#endif


