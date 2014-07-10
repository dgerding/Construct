using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class MovieAPI
    {
        public static byte[] GetMovieFragment()
        {
            return ResourceAPI.GetByteResource("Utilities.part.mov");
        }


    }
}
