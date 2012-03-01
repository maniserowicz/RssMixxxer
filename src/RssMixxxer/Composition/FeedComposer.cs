using System.Collections.Generic;
using System.ServiceModel.Syndication;
using RssMixxxer.Configuration;
using RssMixxxer.LocalCache;
using System.Linq;

namespace RssMixxxer.Composition
{
    public interface IFeedComposer
    {
        SyndicationFeed ComposeFeed();
    }

    public class FeedComposer : IFeedComposer
    {
        private readonly ILocalFeedsProvider _localFeedsProvider;
        private readonly IFeedMixer _feedMixer;
        private FeedAggregatorConfig _config;

        public FeedComposer(ILocalFeedsProvider localFeedsProvider, IFeedMixer feedMixer, IFeedAggregatorConfigProvider configProvider)
        {
            _localFeedsProvider = localFeedsProvider;
            _feedMixer = feedMixer;

            _config = configProvider.ProvideConfig();
        }

        public SyndicationFeed ComposeFeed()
        {
            List<LocalFeedInfo> allFeeds = _localFeedsProvider.Db().LocalFeedInfo.All()
                .ToList<LocalFeedInfo>();

            var feedsArray = allFeeds
                .Where(x => _config.SourceFeeds.Contains(x.Url))
                .Select(x => x.Content).ToArray();

            var items = _feedMixer.MixFeeds(feedsArray);

            var feed = new SyndicationFeed(items.Take(_config.MaxItems));
            feed.Title = new TextSyndicationContent(_config.Title);
            
            return feed;
        }
    }
}