using System;
using System.Linq;
using System.Net;
using FakeItEasy;
using Xunit;
using RssMixxxer.Remote;

namespace RssMixxxer.Tests.Remote
{
    public class reading_feed_from_remote_source
    {
        private string _validUri = "http://feeds.feedburner.com/maciejaniserowicz";

        private RemoteData _remoteData;
        private IHttpRequestFactory HttpRequestFactory;
        private string _incorrectUri;

        public reading_feed_from_remote_source()
        {
            HttpRequestFactory = A.Fake<IHttpRequestFactory>();

            _remoteData = new RemoteData(HttpRequestFactory);
            _incorrectUri = "http://incorrect.uri";
        }

        private RemoteContentResponse execute(LocalFeedInfo feedInfo)
        {
            return _remoteData.ReadRemoteSource(feedInfo);
        }

        [Fact]
        public void creates_feed_from_given_uri()
        {
            configure_webrequest();

            var response = execute(new LocalFeedInfo { Url = _validUri });

            Assert.Equal(15, response.Content.Items.Count());
        }

        [Fact]
        public void throws_when_given_incorrect_uri()
        {
            configure_webrequest();

            var exc = Assert.Throws<CannotReachRemoteSourceException>(
                () => execute(new LocalFeedInfo { Url = _incorrectUri })
            );

            Assert.Equal(_incorrectUri, exc.Uri);
        }

        [Fact]
        public void respects_last_fetch_date_to_avoid_downloading_already_cached_content()
        {
            var rc = new RequestContainer();
            configure_webrequest(_incorrectUri, rc);

            var lastFetchTime = new DateTime(2012, 2, 14);
            ignore_exceptions(
                () => execute(new LocalFeedInfo() { LastFetch = lastFetchTime })
            );
            
            Assert.Equal(lastFetchTime, rc.Request.IfModifiedSince);
        }

        [Fact]
        public void respects_etag_to_avoid_downloading_already_cached_content()
        {
            var rc = new RequestContainer();
            configure_webrequest(_incorrectUri, rc);

            var etag = Guid.NewGuid().ToString();
            ignore_exceptions(
                () => execute(new LocalFeedInfo() { Etag = etag })
            );
            
            Assert.Equal(etag, rc.Request.Headers[HttpRequestHeader.IfNoneMatch]);
        }

        [Fact]
        public void handles_feeds_with_xml_surrounded_by_whitespace()
        {
            _validUri = "http://makoweczki.pl/feed/";

            configure_webrequest();

            var response = execute(new LocalFeedInfo { Url = _validUri });

            Assert.Equal(10, response.Content.Items.Count());
        }

        private void ignore_exceptions(Action action)
        {
            try
            {
                action();
            }
            catch { }
        }

        private class RequestContainer
        {
            public HttpWebRequest Request { get; set; }
        }

        private void configure_webrequest(string uri = null, RequestContainer rc = null)
        {
            A.CallTo(() => HttpRequestFactory.CreateRequest(null))
                .WithAnyArguments()
                .ReturnsLazily(
                    x =>
                        {
                            uri = uri ?? x.GetArgument<string>(0);
                            var request = new HttpRequestFactory().CreateRequest(uri);
                            if (rc != null)
                            {
                                rc.Request = request;
                            }
                            return request;
                        });
        }
    }
}