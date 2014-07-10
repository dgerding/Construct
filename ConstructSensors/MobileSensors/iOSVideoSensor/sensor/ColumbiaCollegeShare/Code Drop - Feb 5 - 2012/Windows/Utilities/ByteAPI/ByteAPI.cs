using System;
using System.Collections.Generic;

namespace Utilities
{
    public static class ByteAPI
    {
        public static byte[] AppendBytes(byte[] b1, byte[] b2)
        {
            return appendBytesInternal(b1, b2);
        }

        public static byte[] AppendBytes(byte[] b1, int size, byte[] b2)
        {
            return AppendBytes(b1, 0, size, b2);
        }

        public static byte[] AppendBytes(byte[] b1, int offset, int size, byte[] b2)
        {
            return appendBytesInternal(b1, offset, size, b2);
        }

        public static byte[] AppendBytes( IEnumerable<byte[]> byteArrayCollection)
        {
            return appendBytesInternal(byteArrayCollection);
        }

        public static byte[] ExtractBytes(byte[] bytes, int startIndex, int length)
        {
            return extractBytesInternal(bytes, startIndex, length);
        }

        public static byte[] ExtractBytes(byte[] bytes, int startIndex)
        {
            return extractBytesInternal(bytes, startIndex, bytes.Length - startIndex);
        }

        public static bool AreSameBytes(byte[] b1, byte[] b2)
        {
            return areSameBytesInternal(b1, b2);
        }

        public static bool AreSameBytes(byte[] b1, byte[] b2, out int firstIndexDifferent)
        {
            return areSameBytesInternal(b1, b2, out firstIndexDifferent);
        }

        public static byte[] CopyBytes(byte[] src)
        {
            return AppendBytes(src, 0, src.Length, null);
        }

        public static byte[] CopyBytes(byte[] src, int size)
        {
            return AppendBytes(src, 0, size, null);
        }

        public static byte[] CopyBytes(byte[] src, int offset, int size)
        {
            return AppendBytes(src, offset, size, null);
        }

        public static byte[] RemoveLeadingBytes(byte[] src, int size)
        {
            int newsize = src.Length - size;
            if (newsize == 0)
            {
                return new byte[0];
            }

            byte[] dest = new byte[newsize];
            Array.Copy(src, size, dest, 0, newsize);
            return dest;
        }

        private static bool areSameBytesInternal(byte[] b1, byte[] b2)
        {
            int dontCare;
            return areSameBytesInternal(b1, b2, out dontCare);
        }

        private static bool areSameBytesInternal(byte[] b1, byte[] b2, out int firstIndexDifferent)
        {
            firstIndexDifferent = -1;
            if ((b1 == null) || (b2 == null))
            {
                return false;
            }
            if (b1.Length != b2.Length)
            {
                return false;
            }
            for (int index = 0; index < b1.Length; index++)
            {
                if (b1[index] != b2[index])
                {
                    firstIndexDifferent = index;
                    return false;
                }
            }
            return true;
        }

        private static byte[] extractBytesInternal(byte[] bytes, int startIndex, int length)
        {
            if ((bytes == null) || (startIndex > bytes.Length) || ((startIndex + length) > bytes.Length))
            {
                throw new ApplicationException("failed to extract bytes");
            }

            byte[] outBytes = new byte[length];
            Array.Copy(bytes, startIndex, outBytes, 0, length);
            return outBytes;
        }

        private static byte[] appendBytesInternal(byte[] b1, byte[] b2)
        {
            if (b1 == null)
            {
                return b2;
            }
            if (b2 == null)
            {
                return b1;
            }
            int count = b1.Length + b2.Length;
            byte[] bytes = new byte[count];
            Array.Copy(b1, 0, bytes, 0, b1.Length);
            Array.Copy(b2, 0, bytes, b1.Length, b2.Length);
            return bytes;
        }

        private static byte[] appendBytesInternal(byte[] b1, int offset, int size, byte[] b2)
        {
            if ((b1 == null) || (size==0))
            {
                return b2;
            }

            byte[] bytes = null;
            if (b2 == null)
            {
                bytes = new byte[size];
                Array.Copy(b1, offset, bytes, 0, size);
            }
            else
            {
                bytes = new byte[b2.Length + size];
                Array.Copy(b2, 0, bytes, 0, b2.Length);
                Array.Copy(b1, offset, bytes, b2.Length, size);
            }
            return bytes;
        }

        private static byte[] appendBytesInternal(IEnumerable<byte[]> byteArrayCollection)
        {
            if ( byteArrayCollection == null )
            {
                return null;
            }

            int length = 0;
            foreach (byte[] bytes in byteArrayCollection)
            {
                if (( bytes == null ) || (bytes.Length == 0))
                {
                    continue;
                }
                length += bytes.Length;
            }
            byte[] combinedBytes = new byte[length];
            int index = 0;
            foreach (byte[] bytes in byteArrayCollection)
            {
                if ((bytes == null) || (bytes.Length == 0))
                {
                    continue;
                }
                bytes.CopyTo(combinedBytes, index);
                index += bytes.Length;
            }
            return combinedBytes;
        }

        public static byte[] GenerateRandomBytes(int numBytes)
        {
            Random rand = new Random();
            byte[] bytes = new byte[numBytes];
            for (int i = 0; i < numBytes; i++ )
            {
                bytes[i] = (byte) rand.Next(0, 255);
            }
            return bytes;
        }
    }
}
