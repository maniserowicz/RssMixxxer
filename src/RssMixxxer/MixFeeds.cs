using System;
using NLog;
using RssMixxxer.Background;
using RssMixxxer.Configuration;
using RssMixxxer.LocalCache;

namespace RssMixxxer
{
    /// <summary>
    /// Entry point for <see cref="RssMixxxer"/>.
    /// </summary>
    public class MixFeeds : IDisposable
    {
        private static bool _isRunning;
        private static object _syncRoot = new object();

        private readonly IFeedAggregatorConfigProvider _configProvider;
        private readonly ISingleFeedUpdate _singleFeedUpdate;
        private BackgroundOperation _backgroundOperation;

        public MixFeeds(
            IFeedAggregatorConfigProvider configProvider
            , ISingleFeedUpdate singleFeedUpdate
        )
        {
            _configProvider = configProvider;
            _singleFeedUpdate = singleFeedUpdate;
        }

        /// <summary>
        /// Initializes <see cref="RssMixxxer"/>'s background maintenance.
        /// </summary>
        public void ForMyNeeds()
        {
            lock (_syncRoot)
            {
                if (_isRunning)
                {
                    _log.Warn("Failed attempt to start mixing feeds - another instance is already running.");
                    return;
                }

                _backgroundOperation = new BackgroundOperation(() =>
                    {
                        var config = _configProvider.ProvideConfig();
                        var sourceFeeds = config.SourceFeeds;

                        _log.Debug("Synchronizing {0} feeds", sourceFeeds.Length);

                        foreach (var src in sourceFeeds)
                        {
                            try
                            {
                                _singleFeedUpdate.UpdateFeed(src);
                            }
                            catch (Exception exc)
                            {
                                _log.ErrorException(string.Format("Error occured when reading feed '{0}'", src), exc);
                            }
                        }
                    }, TimeSpan.FromSeconds(_configProvider.ProvideConfig().SyncInterval_Seconds));

                _backgroundOperation.Start();

                _isRunning = true;
            }
        }

        public void Dispose()
        {
            if (_backgroundOperation != null)
            {
                _log.Debug("Disposing feed mixxxer");

                _backgroundOperation.Dispose();
                _backgroundOperation = null;
            }
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    }
}