using System;

namespace RssMixxxer
{
    public class LocalFeedInfo
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Etag { get; set; }
        public DateTime? LastFetch { get; set; }

        public string Content { get; set; }
    }
}