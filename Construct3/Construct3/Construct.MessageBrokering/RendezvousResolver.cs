using System;
using System.Collections.Generic;
using System.Linq;

namespace Construct.MessageBrokering
{
    public static class RendezvousResolver
    {
        public static List<Uri> ConvertUrisIfLocal(Uri[] theUris)
        {
            UriBuilder optimalUri = new UriBuilder();
            List<Uri> optimalServiceUris = new List<Uri>();
            foreach (Uri uri in theUris)
            {
                if (IsUriThisHost(uri))
                {
                    optimalUri.Scheme = Uri.UriSchemeNetPipe;
                    optimalUri.Host = "localhost";
                    optimalUri.Path = uri.AbsolutePath;
                    optimalServiceUris.Add(optimalUri.Uri);
                }
                else
                {
                    optimalServiceUris.Add(uri);
                }
            }

            return optimalServiceUris;
        }

        public static Uri ConvertUrisIfLocal(Uri theUri)
        {
            UriBuilder optimalUri = new UriBuilder();
            Uri optimalServiceUri = null;

            if (IsUriThisHost(theUri))
            {
                optimalUri.Scheme = Uri.UriSchemeNetPipe;
                optimalUri.Host = "localhost";
                optimalUri.Path = theUri.AbsolutePath;
                optimalServiceUri = optimalUri.Uri;

                return optimalServiceUri;
            }
            return theUri;
        }

        private static bool IsUriThisHost(Uri theUri)
        {
            if (theUri.Scheme == Uri.UriSchemeNetPipe)
            {
                return true;
            }
            else if (theUri.Scheme == Uri.UriSchemeHttp && theUri.IsLoopback)
            {
                return true;
            }

            return false;
        }
    }
}