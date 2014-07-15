using System.Configuration;
using System.Linq;

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
                MaxItems = int.Parse(appSettings["rssMixxxer.maxItems"]),
                SourceFeeds = appSettings["rssMixxxer.src"]
                    .Split(';')
                    .Select(x => x.Trim())
                    .Where(x => string.IsNullOrWhiteSpace(x) == false)
                    .ToArray(),
                SyncInterval_Seconds = int.Parse(appSettings["rssMixxxer.interval_seconds"]),
            };
        }
    }
}