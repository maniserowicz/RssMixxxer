using System.ServiceModel.Syndication;
using Xunit;

namespace RssMixxxer.Tests
{
    public class saving_rss_feed_as_string
    {
        private static string execute()
        {
            var feed = new SyndicationFeed();
            feed.Title = new TextSyndicationContent("some title");

            return feed.GetRssString();
        }

        [Fact]
        public void uses_utf8_encoding()
        {
            var rssString = execute();

            Assert.Contains("encoding=\"utf-8\"?>", rssString);
        }

        [Fact]
        public void cuts_off_UTF8_BOM()
        {
            var rssString = execute();

            Assert.Equal('<', rssString[0]);
        }
    }
}