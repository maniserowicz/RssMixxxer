using RssMixxxer.Composition;
using RssMixxxer.LocalCache;
using RssMixxxer.Remote;
using RssMixxxer.Tests.LocalCache;
using Xunit;

namespace RssMixxxer.Tests.x_Integration
{
    public class composing_feed_from_content_read_from_remote_sources
    {
        private TestFeedsProvider _feedsProvider;
        private HttpRequestFactory _httpRequestFactory;
        private RemoteData _remoteData;
        private SingleFeedUpdate _singleFeedUpdate;
        private FeedMixer _feedMixer;

        public composing_feed_from_content_read_from_remote_sources()
        {
            _feedsProvider = new TestFeedsProvider();
            _httpRequestFactory = new HttpRequestFactory();
            _remoteData = new RemoteData(_httpRequestFactory);
            _singleFeedUpdate = new SingleFeedUpdate(_remoteData, _feedsProvider);
            _feedMixer = new FeedMixer();
        }

        [Fact]
        public void correctly_mixes_data_from_remote_source___covers_issue_with_UTF8_BOM_screwing_up_xml_parsing()
        {
            var url = "http://feeds.feedburner.com/maciejaniserowicz";

            _singleFeedUpdate.UpdateFeed(url);

            var feedInfo = _feedsProvider.Db().LocalFeedInfo.All()
                .ToList();
            string feedContent = feedInfo[0].Content;

            Assert.DoesNotThrow(
                () => _feedMixer.MixFeeds(new[] { feedContent })
            );
        }
    }
}