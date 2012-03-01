using System;

namespace RssMixxxer.Remote
{
    public class CannotReachRemoteSourceException : Exception
    {
        public string Uri { get; private set; }

        public CannotReachRemoteSourceException(string uri, Exception inner)
            : base(string.Format("Cannot reach remote source: '{0}'", uri), inner)
        {
            Uri = uri;
        }
    }
}