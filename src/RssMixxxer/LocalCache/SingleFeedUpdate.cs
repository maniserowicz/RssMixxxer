using NLog;
using RssMixxxer.Environment;
using RssMixxxer.Remote;

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

            _log.Debug("Updating {0} feed '{1}'", feed.Id == 0 ? "new" : "existing", url);

            var remoteResponse = _remoteData.ReadRemoteSource(feed);

            if (remoteResponse.HasNewContent == false)
            {
                _log.Debug("Feed '{0}' does not have new content", url);

                return;
            }

            bool newFeed = feed.LastFetch == null;

            feed.Content = remoteResponse.Content.GetRssString();
            feed.LastFetch = ApplicationTime.Current;
            feed.Etag = remoteResponse.Etag;

            if (newFeed)
            {
                _log.Info("Inserting new feed '{0}' to local cache", url);

                db.LocalFeedInfo.Insert(feed);
            }
            else
            {
                _log.Info("Updating feed '{0}' in local cache", url);

                db.LocalFeedInfo.Update(feed);
            }
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    }
}