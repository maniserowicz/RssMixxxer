using System;
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

            dynamic read_view = GetLocalFeedInfoView(db);

            LocalFeedInfo feed = read_view.FindByUrl(url)
                ?? new LocalFeedInfo
                    {
                        Url = url,
                    };

            _log.Debug("Updating {0} feed '{1}'", feed.Id == 0 ? "new" : "existing", url);

            RemoteContentResponse remoteResponse = null;

            try
            {
                remoteResponse = _remoteData.ReadRemoteSource(feed);
            }
            catch(Exception exc)
            {
                OnReadingRemoteSourceError(db, feed, exc);
                throw;
            }

            if (remoteResponse.HasNewContent == false)
            {
                _log.Debug("Feed '{0}' does not have new content", url);

                return;
            }

            bool newFeed = feed.LastFetch == null;
            string previousContent = feed.Content ?? string.Empty;

            feed.Content = GetFeedStringContent(remoteResponse);
            feed.LastFetch = ApplicationTime.Current;
            feed.Etag = remoteResponse.Etag;

            if (newFeed)
            {
                _log.Info("Inserting new feed '{0}' to local cache", url);

                InsertNewFeed(db, feed, url);
            }
            else
            {
                _log.Info("Updating feed '{0}' in local cache", url);

                UpdateExistingFeed(db, feed, url);
            }

            if (previousContent.GetHashCode() == feed.Content.GetHashCode())
            {
                _log.Warn("Wasting resources: updating feed '{0}' with the same content it already had!", url);
            }
        }

        protected virtual void OnReadingRemoteSourceError(dynamic db, LocalFeedInfo feed, Exception exc)
        {
            
        }

        /// <summary>
        /// Inserts new feed into DB if not fetched before
        /// </summary>
        protected virtual void InsertNewFeed(dynamic db, LocalFeedInfo feed, string url)
        {
            db.LocalFeedInfo.Insert(feed);
        }

        protected virtual void UpdateExistingFeed(dynamic db, LocalFeedInfo feed, string url)
        {
            db.LocalFeedInfo.Update(feed);
        }

        protected virtual dynamic GetLocalFeedInfoView(dynamic db)
        {
            return db.LocalFeedInfo;
        }

        /// <summary>
        /// Gets whole feed content as string made of all it's items.
        /// </summary>
        protected virtual string GetFeedStringContent(RemoteContentResponse remoteResponse)
        {
            return remoteResponse.Content.GetRssString();
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    }
}