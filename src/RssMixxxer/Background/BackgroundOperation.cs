using System;
using System.Diagnostics;
using System.Threading;
using NLog;
using Timer = System.Timers.Timer;

namespace RssMixxxer.Background
{
    /// <summary>
    /// Non-reentrant
    /// </summary>
    public class BackgroundOperation : IDisposable
    {
        private readonly Action _action;
        private Timer _timer;
        private volatile int _counter;
        private object _syncRoot = new object();
        private volatile bool _isStarted;
        private volatile bool _isDisposed;

        public BackgroundOperation(Action action, TimeSpan interval)
        {
            _action = action;
            _timer = new Timer(interval.TotalMilliseconds)
                {
                    AutoReset = false,
                };
            _timer.Elapsed += (_, __) => Execute();
        }

        public void Start()
        {
            lock (_syncRoot)
            {
                if (_isStarted)
                {
                    throw new InvalidOperationException("Operation already started");
                }
                _isStarted = true;
            }

            Execute();
        }

        private void Execute()
        {
            ThreadPool.QueueUserWorkItem(_ =>
                {
                    if (_isDisposed)
                    {
                        return;
                    }

                    var stopwatch = Stopwatch.StartNew();

                    try
                    {
                        _counter++;

                        _log.Debug("Executing background operation, iteration counter: {0}", _counter);

                        _action();
                    }
                    catch (Exception exc)
                    {
                        _log.ErrorException("Unhandled error when executing backround operation", exc);
                    }
                    finally
                    {
                        _log.Debug("Background operation finished ({0})", stopwatch.Elapsed);

                        if (_isDisposed == false && _timer != null)
                        {
                            _log.Debug("Scheduling background operation to run in {0}", TimeSpan.FromMilliseconds(_timer.Interval).ToString());

                            _timer.Start();
                        }
                    }
                });
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("BackgroundOperation");
            }

            _isDisposed = true;

            _log.Debug("Disposing background operation");

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    }
}