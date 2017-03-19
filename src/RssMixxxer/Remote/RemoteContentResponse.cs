using System.Net;
using System.ServiceModel.Syndication;

namespace RssMixxxer.Remote
{
    public class RemoteContentResponse
    {
        public static RemoteContentResponse NotModified(HttpStatusCode statusCode)
        {
            return new RemoteContentResponse
            {
                HasNewContent = false,
                StatusCode = statusCode
            };
        }

        public bool HasNewContent { get; private set; }

        public string Etag { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public SyndicationFeed Content { get; private set; }

        private RemoteContentResponse()
        {
        }

        public RemoteContentResponse(bool hasNewContent, string etag, SyndicationFeed content, HttpStatusCode statusCode)
        {
            HasNewContent = hasNewContent;
            Etag = etag;
            Content = content;
            StatusCode = statusCode;
        }
    }
}