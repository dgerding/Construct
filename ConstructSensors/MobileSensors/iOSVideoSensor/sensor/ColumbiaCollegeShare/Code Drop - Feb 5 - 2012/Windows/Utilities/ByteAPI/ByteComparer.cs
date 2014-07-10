using System.Collections.Generic;

namespace Utilities
{
    // determines if two byte arrays have equivalent contents
    public class ByteComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            return ByteAPI.AreSameBytes(x, y);
        }

        public int GetHashCode(byte[] obj)
        {
            if ( obj == null )
            {
                return 0;
            }

            return obj.Length;
        }
    }
}
