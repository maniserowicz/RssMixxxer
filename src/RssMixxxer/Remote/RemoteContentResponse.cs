using System.ServiceModel.Syndication;

namespace RssMixxxer.Remote
{
    public class RemoteContentResponse
    {
        public static readonly RemoteContentResponse NotModified = new RemoteContentResponse
            {
                HasNewContent = false,
            };

        public bool HasNewContent { get; private set; }

        public string Etag { get; private set; }
        public SyndicationFeed Content { get; private set; }

        private RemoteContentResponse()
        {
        }

        public RemoteContentResponse(bool hasNewContent, string etag, SyndicationFeed content)
        {
            HasNewContent = hasNewContent;
            Etag = etag;
            Content = content;
        }
    }
}