using Microsoft.Xna.Framework.Graphics;
using sm.eod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;


namespace SMFramework
{
	//	Super-fast converter from ImageOutputData object to an XNA Texture2D.
	public class ImageOutputDataConverter
	{
		unsafe public Texture2D Convert(GraphicsDevice targetGraphicsDevice, ImageOutputData imageOutputData)
		{
			Texture2D result = new Texture2D(targetGraphicsDevice, (int)imageOutputData.Width(), (int)imageOutputData.Height());

			D3DFORMAT textureFormat;
			switch (imageOutputData.ImageFormat())
			{
					//	Values pulled from sm_api_imagetypes.h, in FaceAPI documentation

				case (12): // 32-bit ARGB
					textureFormat = D3DFORMAT.A8R8G8B8;
					break;

				case (21): // 24-bit RGB
					textureFormat = D3DFORMAT.R8G8B8;
					break;

				default:
					throw new Exception("Image type must be RGB/ARGB 8-bits per channel.");
			}

			result.SetData((void *)0, imageOutputData.RawData().ToPointer(), (int)imageOutputData.Width(), (int)imageOutputData.Height(), imageOutputData.BytesPerLine(), textureFormat);

			return result;
		}
	}





	//	Skimmed down from:
	//	http://www.gamasutra.com/view/news/127515/Opinion_Copying_Pixels_From_A_Pointer_To_An_XNA_Texture2D.php
	// ... https://github.com/sq/Fracture/blob/master/Squared/RenderLib/Evil.cs

    public enum D3DFORMAT : uint {
        UNKNOWN              =  0,

        R8G8B8               = 20,
        A8R8G8B8             = 21,
    }

    [Flags]
    enum D3DX_FILTER : uint {
        NONE                 = 0x00000001
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RECT {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    static class TextureUtils {

        [DllImport("d3dx9_41.dll")]
        [SuppressUnmanagedCodeSecurity]
        private static unsafe extern int D3DXLoadSurfaceFromMemory (
            void* pDestSurface,
            void* pDestPalette,
            RECT* pDestRect,
            void* pSrcMemory,
            D3DFORMAT srcFormat,
            uint srcPitch,
            void* pSrcPalette,
            RECT* pSrcRect,
            D3DX_FILTER filter,
            uint colorKey
        );

        /// <summary>
        /// Copies pixels from an address in memory into a mip level of Texture2D, converting them from one format to another if necessary.
        /// </summary>
        /// <param name="texture">The texture to copy to.</param>
        /// <param name="level">The index into the texture's mip levels.</param>
        /// <param name="pData">The address of the pixel data.</param>
        /// <param name="width">The width of the pixel data (in pixels).</param>
        /// <param name="height">The height of the pixel data (in pixels).</param>
        /// <param name="pitch">The number of bytes occupied by a single row of the pixel data (including padding at the end of rows).</param>
        /// <param name="pixelFormat">The format of the pixel data.</param>
        public static unsafe void SetData (
            this Texture2D texture, void* pSurface, void* pData, 
            int width, int height, uint pitch, 
            D3DFORMAT pixelFormat
        ) {
            var rect = new RECT {
                Top = 0,
                Left = 0,
                Right = width,
                Bottom = height
            };

            SetData(texture, pSurface, pData, ref rect, pitch, ref rect, pixelFormat);
        }

        /// <summary>
        /// Copies pixels from an address in memory into a mip level of Texture2D, converting them from one format to another if necessary.
        /// </summary>
        /// <param name="texture">The texture to copy to.</param>
        /// <param name="level">The index into the texture's mip levels.</param>
        /// <param name="pData">The address of the pixel data.</param>
        /// <param name="destRect">The destination rectangle.</param>
        /// <param name="pitch">The number of bytes occupied by a single row of the pixel data (including padding at the end of rows).</param>
        /// <param name="sourceRect">The source rectangle.</param>
        /// <param name="pixelFormat">The format of the pixel data.</param>
        public static unsafe void SetData (
            this Texture2D texture, void* pSurface, void* pData,
            ref RECT destRect, uint pitch, ref RECT sourceRect,
            D3DFORMAT pixelFormat
        ) {
            fixed (RECT* pDestRect = &destRect)
            fixed (RECT* pSourceRect = &sourceRect) {
                var rv = D3DXLoadSurfaceFromMemory(pSurface, null, pDestRect, pData, pixelFormat, pitch, null, pSourceRect, D3DX_FILTER.NONE, 0);
                if (rv != 0)
                    throw new COMException("D3DXLoadSurfaceFromMemory failed", rv);
            }
        }
    }
}
