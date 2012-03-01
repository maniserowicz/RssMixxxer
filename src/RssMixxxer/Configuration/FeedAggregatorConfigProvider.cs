using System.Configuration;

namespace RssMixxxer.Configuration
{
    public interface IFeedAggregatorConfigProvider
    {
        FeedAggregatorConfig ProvideConfig();
    }

    public class FeedAggregatorConfigProvider : IFeedAggregatorConfigProvider
    {
        public FeedAggregatorConfig ProvideConfig()
        {
            var appSettings = ConfigurationManager.AppSettings;

            return new FeedAggregatorConfig
                {
                    Title = appSettings["rssMixxxer.title"],
                    MaxItems = int.Parse(appSettings["rssMixxxer.title"]),
                    SourceFeeds = appSettings["rssMixxxer.src"].Split(';'),
                };
        }
    }
}