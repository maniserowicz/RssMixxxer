using System;
using System.ComponentModel;

namespace RssMixxxer.Environment
{
    public static class ApplicationTime
    {
        // RSS aggregator logic requires DateTime.Now as opposed to DateTime.UtcNow
        // framework classes handle translation to UTC internally
        private static readonly Func<DateTime> _defaultLogic = () => DateTime.Now;

        private static Func<DateTime> _current = _defaultLogic;

        /// <summary>
        /// Returns current date/time, correct in application context
        /// </summary>
        /// <remarks>Normally this would return <see cref="DateTime.Now"/>, but can be changed to make testing easier.</remarks>
        public static DateTime Current
        {
            get { return _current(); }
        }

        /// <summary>
        /// Changes logic behind <see cref="Current"/>. Useful in scenarios where time needs to be defined upfront, like unit tests.
        /// </summary>
        /// <remarks>Be sure you know what you are doing when calling this method.</remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void _replaceCurrentTimeLogic(Func<DateTime> newCurrentTimeLogic)
        {
            _current = newCurrentTimeLogic;
        }

        /// <summary>
        /// Reverts current logic to the default logic. Useful in scenarios where unit test changed logic and should rever system to previous state.
        /// </summary>
        /// <remarks>Be sure you know what you are doing when calling this method.</remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void _revertToDefaultLogic()
        {
            _current = _defaultLogic;
        }
    }
}