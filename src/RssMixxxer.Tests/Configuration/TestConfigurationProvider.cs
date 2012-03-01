using RssMixxxer.Configuration;

namespace RssMixxxer.Tests.Configuration
{
    public class TestConfigurationProvider : IFeedAggregatorConfigProvider
    {
        private FeedAggregatorConfig _feedAggregatorConfig;

        public TestConfigurationProvider()
        {
            _feedAggregatorConfig = new FeedAggregatorConfig
                {
                    Title = "rss feed aggregator test",
                    MaxItems = 13,
                    SourceFeeds = new[]
                        {
                            "http://source.feed/rss"
                            , "http://source2.feed/rss"
                        }
                };
        }

        public FeedAggregatorConfig ProvideConfig()
        {
            return _feedAggregatorConfig;
        }
    }
}