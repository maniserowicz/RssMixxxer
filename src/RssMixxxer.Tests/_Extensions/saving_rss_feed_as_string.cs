using System.ServiceModel.Syndication;
using Xunit;

namespace RssMixxxer.Tests
{
    public class saving_rss_feed_as_string
    {
        [Fact]
        public void uses_utf8_encoding()
        {
            var feed = new SyndicationFeed();
            feed.Title = new TextSyndicationContent("some title");

            string rssString = feed.GetRssString();

            Assert.Contains("encoding=\"utf-8\"?>", rssString);
        }
    }
}