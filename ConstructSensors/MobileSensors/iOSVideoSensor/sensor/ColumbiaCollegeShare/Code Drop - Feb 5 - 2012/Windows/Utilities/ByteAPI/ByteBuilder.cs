using System;
using System.Collections.Generic;

namespace Utilities
{
    public class ByteBuilder
    {
        private List<byte[]> buffers = new List<byte[]>();
        private int length;

        public int Length
        {
            get { return length; }
        }

        public void Append( byte[] bytes )
        {
            if (( bytes == null ) || (bytes.Length == 0) )
            {
                return;
            }

            buffers.Add(bytes);
            length += bytes.Length;
        }

        public void Append(byte[] bytes, int start, int size)
        {
            if ((bytes == null) || (bytes.Length == 0) || size <= 0)
            {
                return;
            }
            if ( start >= size )
            {
                throw new ArgumentOutOfRangeException("start index for append operation is past the end of the array");
            }

            byte[] newBytes = ByteAPI.CopyBytes(bytes, start, size);

            buffers.Add(newBytes);
            this.length += newBytes.Length;
        }



        public byte[] ToBytes()
        {
            byte[] bigBuffer = new byte[length];
            int start = 0;
            foreach ( byte[] buffer in buffers )
            {
                Array.Copy(buffer, 0, bigBuffer, start, buffer.Length);
                start += buffer.Length;
            }
            return bigBuffer;
        }
    }
}
