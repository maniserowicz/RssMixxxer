using System.ServiceModel.Syndication;
using FakeItEasy;
using RssMixxxer.Tests.Configuration;
using RssMixxxer.Tests.LocalCache;
using Xunit;
using RssMixxxer.Composition;
using System.Linq;

namespace RssMixxxer.Tests.Composition
{
    public class composing_result_feed
    {
        private TestFeedsProvider _feedsProvider;
        private IFeedMixer _feedMixer;
        private TestConfigurationProvider _configurationProvider;
        private FeedComposer _composer;

        public composing_result_feed()
        {
            _feedsProvider = new TestFeedsProvider();
            _feedMixer = A.Fake<IFeedMixer>();
            _configurationProvider = new TestConfigurationProvider();

            _composer = new FeedComposer(_feedsProvider, _feedMixer, _configurationProvider);
        }

        private SyndicationFeed execute()
        {
            return _composer.ComposeFeed();
        }

        [Fact]
        public void sets_configured_title_in_result_feed()
        {
            var result = execute();

            Assert.Equal(_configurationProvider.ProvideConfig().Title, result.Title.Text);
        }

        private void add_feed_to_db(string url, string content)
        {
            _feedsProvider.Db().LocalFeedInfo.Insert(Url: url, Content: content);
        }

        [Fact]
        public void returns_feed_mixed_from_cached()
        {
            var config = _configurationProvider.ProvideConfig();

            var feeds = Resource.read_feeds().ToList();

            add_feed_to_db(config.SourceFeeds[0], feeds[0]);
            add_feed_to_db(config.SourceFeeds[1], feeds[1]);

            var mixedItems = new[]
                {
                    new SyndicationItem(), new SyndicationItem(),
                };

            A.CallTo(
                () => _feedMixer.MixFeeds(
                    A<string[]>.That.IsSameSequenceAs(feeds)
                )
            ).Returns(mixedItems);

            var result = execute();

            Assert.Equal(mixedItems, result.Items.ToArray());
        }

        [Fact]
        public void uses_only_configured_cached_feeds_for_mixing()
        {
            var feedUrl = "http://some.feed/rss";

            var feeds = Resource.read_feeds().ToList();

            var config = _configurationProvider.ProvideConfig();

            add_feed_to_db(feedUrl, feeds[0]);
            add_feed_to_db(config.SourceFeeds[1], feeds[1]);

            var mixedItems = new[]
                {
                    new SyndicationItem(), new SyndicationItem(),
                };

            config.SourceFeeds = new[] {feedUrl};

            A.CallTo(
                () => _feedMixer.MixFeeds(
                    A<string[]>.That.IsSameSequenceAs(new[] {feeds[0]})
                )
            ).Returns(mixedItems);

            var result = execute();

            Assert.Equal(mixedItems, result.Items.ToArray());
        }

        [Fact]
        public void returns_configured_number_of_items_in_result_feed()
        {
            _configurationProvider.ProvideConfig().MaxItems = 3;

            var mixedItems = new[]
                {
                    new SyndicationItem(), new SyndicationItem(),
                    new SyndicationItem(), new SyndicationItem(),
                    new SyndicationItem(), new SyndicationItem(),
                };

            A.CallTo(
                () => _feedMixer.MixFeeds(null)
            ).WithAnyArguments()
            .Returns(mixedItems);

            var result = execute();

            Assert.Equal(3, result.Items.Count());
        }
    }
}