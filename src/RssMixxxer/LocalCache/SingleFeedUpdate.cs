using RssMixxxer.Environment;
using RssMixxxer.Remote;
using RssMixxxer._Extensions;

namespace RssMixxxer.LocalCache
{
    public interface ISingleFeedUpdate
    {
        void UpdateFeed(string url);
    }

    public class SingleFeedUpdate : ISingleFeedUpdate
    {
        private readonly IRemoteData _remoteData;
        private readonly ILocalFeedsProvider _localFeedsProvider;

        public SingleFeedUpdate(IRemoteData remoteData, ILocalFeedsProvider localFeedsProvider)
        {
            _remoteData = remoteData;
            _localFeedsProvider = localFeedsProvider;
        }

        public void UpdateFeed(string url)
        {
            var db = _localFeedsProvider.Db();

            LocalFeedInfo feed = db.LocalFeedInfo.FindByUrl(url)
                ?? new LocalFeedInfo()
                    {
                        Url = url,
                    };

            var remoteResponse = _remoteData.ReadRemoteSource(feed);

            if (remoteResponse.HasNewContent)
            {
                bool newFeed = feed.LastFetch == null;

                feed.Content = remoteResponse.Content.GetRssString();
                feed.LastFetch = ApplicationTime.Current;
                feed.Etag = remoteResponse.Etag;

                if (newFeed)
                {
                    db.LocalFeedInfo.Insert(feed);
                }
                else
                {
                    db.LocalFeedInfo.Update(feed);
                }
            }
        }
    }
}