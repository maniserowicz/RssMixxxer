using System.Collections.Generic;
using System.ServiceModel.Syndication;
using Xunit;
using RssMixxxer.Composition;
using System.Linq;

namespace RssMixxxer.Tests.Composition
{
    public class mixing_multiple_feeds_into_one
    {
        private FeedMixer _mixer;

        public mixing_multiple_feeds_into_one()
        {
            _mixer = new FeedMixer();
        }

        private IEnumerable<SyndicationItem> execute()
        {
            return _mixer.MixFeeds(Resource.read_feeds().ToArray());
        }

        [Fact]
        public void mixes_multiple_feeds_sorting_by_date_descending()
        {
            var result = execute().ToList();

            foreach (var feed2item in result.Take(4))
            {
                Assert.Equal("gutek", feed2item.Authors.Single().Email);
            }

            Assert.Equal("procent", result.Skip(4).First().Authors.Single().Email);
        }

        [Fact]
        public void returs_all_items()
        {
            var result = execute();

            Assert.Equal(35, result.Count());
        }
    }
}