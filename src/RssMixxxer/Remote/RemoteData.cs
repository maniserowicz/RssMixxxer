using System;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using NLog;
using System.Linq;

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

                _log.Debug("Trying to read remote source '{0}' (etag: {1}, last fetch: {2})", feedInfo.Url, feedInfo.Etag, feedInfo.LastFetch);

                using (var response = (HttpWebResponse) request.GetResponseEx())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string etag = response.Headers[HttpResponseHeader.ETag];

                        using (var reader = XmlReader.Create(response.GetResponseStream()))
                        {
                            var feed = SyndicationFeed.Load(reader);

                            _log.Info("Returning new content for remote source '{0}' with {1} items", feedInfo.Url, feed.Items.Count());

                            return new RemoteContentResponse(true, etag, feed);
                        }
                    }
                    else
                    {
                        _log.Debug("No new content for remote source '{0}', response status is {1}", feedInfo.Url, response.StatusCode);

                        return RemoteContentResponse.NotModified;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.ErrorException(string.Format("Failed to read remote source '{0}'", feedInfo.Url), exc);

                throw new CannotReachRemoteSourceException(feedInfo.Url, exc);
            }
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    }
}