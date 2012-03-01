using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using FakeItEasy;
using Xunit;
using RssMixxxer.Environment;
using RssMixxxer.LocalCache;
using RssMixxxer.Remote;
using RssMixxxer._Extensions;

namespace RssMixxxer.Tests.LocalCache
{
    public class updating_single_feed
    {
        private SingleFeedUpdate _singleFeedUpdate;
        private TestFeedsProvider _feedsProvider;
        private IRemoteData _remoteData;

        public updating_single_feed()
        {
            _feedsProvider = new TestFeedsProvider();

            _remoteData = A.Fake<IRemoteData>();
            _singleFeedUpdate = new SingleFeedUpdate(_remoteData, 
_feedsProvider);
        }

        private void execute(string url)
        {
            _singleFeedUpdate.UpdateFeed(url);
        }

        private SyndicationFeed prepare_fake_feed()
        {
            var items = new List<SyndicationItem>();

            for (int i = 0; i < 10; i++)
            {
                var item = new SyndicationItem();
                item.Content = new TextSyndicationContent(Guid.NewGuid().ToString());
                items.Add(item);
            }

            return new SyndicationFeed(items);
        }

        [Fact]
        public void inserts_new_content_for_noncached_feed()
        {
            string url = "http://new.feed/rss";

            var newFeed = prepare_fake_feed();

            A.CallTo(
                () => _remoteData.ReadRemoteSource(
                    A<LocalFeedInfo>.That.Matches(x => x.Url == url && x.LastFetch == null))
                ).Returns(new RemoteContentResponse(true, "new-etag", newFeed));

            execute(url);

            List<LocalFeedInfo> fromDb = _feedsProvider.Db().LocalFeedInfo.All()
                .ToList<LocalFeedInfo>();

            Assert.Equal(1, fromDb.Count);

            Assert.Equal(newFeed.GetRssString(), fromDb[0].Content);
        }

        [Fact]
        public void updates_cache_with_new_feed_content()
        {
            var now = new DateTime(2012, 2, 20);
            ApplicationTime._replaceCurrentTimeLogic(() => now);

            var cached = new LocalFeedInfo()
            {
                Url = "http://cached.feed/rss",
                Content = "cached-content",
                Etag = "cached-etag",
                LastFetch = new DateTime(2012, 2, 3)
            };

            _feedsProvider.Db().LocalFeedInfo.Insert(cached);

            var newFeed = prepare_fake_feed();

            A.CallTo(
                () => _remoteData.ReadRemoteSource(
                    A<LocalFeedInfo>.That.Matches(x => x.Url == cached.Url && x.Etag == cached.Etag && x.LastFetch == cached.LastFetch)
                )
            )
            .Returns(new RemoteContentResponse(true, "new-etag", newFeed));

            execute(cached.Url);

            List<LocalFeedInfo> fromDb = _feedsProvider.Db().LocalFeedInfo.All().ToList<LocalFeedInfo>();

            Assert.Equal(1, fromDb.Count);

            Assert.Equal(cached.Url, fromDb[0].Url);
            Assert.Equal(newFeed.GetRssString(), fromDb[0].Content);
            Assert.Equal("new-etag", fromDb[0].Etag);
            Assert.Equal(now, fromDb[0].LastFetch);
        }

        [Fact]
        public void does_not_update_feed_that_was_not_modified()
        {
            var cached = new LocalFeedInfo()
                {
                    Url = "http://cached.feed/rss",
                    Content = "cached-content",
                    Etag = "cached-etag",
                    LastFetch = new DateTime(2012, 2, 3)
                };

            _feedsProvider.Db().LocalFeedInfo.Insert(cached);

            A.CallTo(
                () => _remoteData.ReadRemoteSource(
                    A<LocalFeedInfo>.That.Matches(x => x.Url == cached.Url && x.Etag == cached.Etag && x.LastFetch == cached.LastFetch)
                )
            )
            .Returns(RemoteContentResponse.NotModified);

            execute(cached.Url);

            List<LocalFeedInfo> fromDb = _feedsProvider.Db().LocalFeedInfo.All().ToList<LocalFeedInfo>();

            Assert.Equal(1, fromDb.Count);

            Assert.Equal(cached.Url, fromDb[0].Url);
            Assert.Equal(cached.Content, fromDb[0].Content);
            Assert.Equal(cached.Etag, fromDb[0].Etag);
            Assert.Equal(cached.LastFetch, fromDb[0].LastFetch);
        }
    }
}