using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Linq;
using System.Xml;
using NLog;

namespace RssMixxxer.Composition
{
    public interface IFeedMixer
    {
        IEnumerable<SyndicationItem> MixFeeds(string[] feedContents);
    }

    public class FeedMixer : IFeedMixer
    {
        public IEnumerable<SyndicationItem> MixFeeds(string[] feedContents)
        {
            var feeds = feedContents.Select(x =>
                {
                    using (var stringReader = new StringReader(x))
                    {
                        using (var xmlReader = XmlReader.Create(stringReader))
                        {
                            return SyndicationFeed.Load(xmlReader);
                        }
                    }
                });

            var sortedItems = feeds.SelectMany(x => x.Items)
                .OrderByDescending(x => x.PublishDate)
                .ToList();

            _log.Debug("Mixed {0} feeds producing {1} items", sortedItems.Count);

            return sortedItems;
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    }
}