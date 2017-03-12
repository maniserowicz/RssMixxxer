using System;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using NLog;
using System.Linq;
using RssMixxxer.Configuration;

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
        private readonly IFeedAggregatorConfigProvider _configProvider;
        private readonly IRemoteContentPreProcessor _contentPreProcessor;

        public RemoteData(IHttpRequestFactory httpRequestFactory, IFeedAggregatorConfigProvider configProvider, IRemoteContentPreProcessor contentPreProcessor)
        {
            _httpRequestFactory = httpRequestFactory;
            _configProvider = configProvider;
            _contentPreProcessor = contentPreProcessor;
        }

        public RemoteContentResponse ReadRemoteSource(LocalFeedInfo feedInfo)
        {
            _log.Debug("Trying to read remote source '{0}' (etag: {1}, last fetch: {2})", feedInfo.Url, feedInfo.Etag, feedInfo.LastFetch);

            try
            {
                if (feedInfo.LastFetch.HasValue && _configProvider.ProvideConfig().PrefetchHeadRequest)
                {
                    var head_request = _httpRequestFactory.CreateRequest(feedInfo.Url);
                    head_request.Method = "HEAD";
                    using (var head_response = (HttpWebResponse)head_request.GetResponseEx())
                    {
                        DateTime last_modified = head_response.LastModified.ToUniversalTime();
                        DateTime last_fetch = feedInfo.LastFetch.Value.ToUniversalTime();
                        if (last_modified < last_fetch)
                        {
                            _log.Debug("HEAD request indicates that there is no new content available (last_modified={0}, last_fetch={1}), returning NotModified.", last_modified, last_fetch);
                            return RemoteContentResponse.NotModified;
                        }
                    }
                }

                var get_request = _httpRequestFactory.CreateRequest(feedInfo.Url);
                if (feedInfo.LastFetch != null)
                {
                    get_request.IfModifiedSince = feedInfo.LastFetch.Value;
                }

                if (string.IsNullOrWhiteSpace(feedInfo.Etag) == false)
                {
                    get_request.Headers[HttpRequestHeader.IfNoneMatch] = feedInfo.Etag;
                }

                using (var response = (HttpWebResponse) get_request.GetResponseEx())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string etag = response.Headers[HttpResponseHeader.ETag];

                        Stream responseStream = response.GetResponseStream();
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            string responseText = streamReader.ReadToEnd();

                            responseText = _contentPreProcessor.PreProcess(responseText);

                            using (var stringReader = new StringReader(responseText))
                            {
                                using (var reader = XmlReader.Create(stringReader))
                                {
                                    var feed = SyndicationFeed.Load(reader);

                                    _log.Info("Returning new content for remote source '{0}' with {1} items", feedInfo.Url, feed.Items.Count());

                                    return new RemoteContentResponse(true, etag, feed);
                                }
                            }
                        }
                    }
                    else
                    {
                        string message = string.Format("No new content for remote source '{0}', response status is {1}", feedInfo.Url, response.StatusCode);

                        if (response.StatusCode == HttpStatusCode.NotModified)
                        {
                            _log.Debug(message);
                        }
                        else
                        {
                            _log.Warn(message);
                        }

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