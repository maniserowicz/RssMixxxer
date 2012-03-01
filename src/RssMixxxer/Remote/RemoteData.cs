using System;
using System.Globalization;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;

namespace RssMixxxer.Remote
{
    public interface IRemoteData
    {
        /// <exception cref="CannotReachRemoteSourceException"></exception>
        RemoteContentResponse ReadRemoteSource(LocalFeedInfo feedInfo);
    }

    public class RemoteData : IRemoteData
    {
        private readonly IHttpRequestFactory _httpRequestFactory;

        public RemoteData(IHttpRequestFactory httpRequestFactory)
        {
            _httpRequestFactory = httpRequestFactory;
        }

        public RemoteContentResponse ReadRemoteSource(LocalFeedInfo feedInfo)
        {
            try
            {
                var request = _httpRequestFactory.CreateRequest(feedInfo.Url);
                if (feedInfo.LastFetch != null)
                {
                    request.IfModifiedSince = feedInfo.LastFetch.Value;
                }

                if (string.IsNullOrWhiteSpace(feedInfo.Etag) == false)
                {
                    request.Headers[HttpRequestHeader.IfNoneMatch] = feedInfo.Etag;
                }

                using (var response = request.GetResponseEx())
                {
                    string etag = response.Headers[HttpResponseHeader.ETag];

                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        var feed = SyndicationFeed.Load(reader);

                        return new RemoteContentResponse(true, etag, feed);
                    }
                }
            }
            catch (Exception exc)
            {
                throw new CannotReachRemoteSourceException(feedInfo.Url, exc);
            }
        }
    }
}