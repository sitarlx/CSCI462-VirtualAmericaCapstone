using System;
using Unity.MARS.Settings;
using UnityEngine;

namespace Unity.MARS.Query
{
    /// <summary>
    /// Contains data that is used by queries and the objects that make queries alike
    /// </summary>
    [Serializable]
    public struct CommonQueryData
    {
        [Tooltip("When enabled, this query will time-out at a different rate than the app default.")]
        public bool overrideTimeout;

        [Tooltip("Sets how long this query should stay active before failing from lack of data. " +
            "A negative value indicates no timeout.")]
        public float timeOut;

        [Tooltip("When enabled, this query will go back to looking for another match if its data is lost")]
        public bool reacquireOnLoss;

        [Tooltip("The interval (in seconds) on which to check for updates to this query's match")]
        public float updateMatchInterval;

        [Tooltip("The importance level of the query")]
        public MarsEntityPriority priority;

        /// <summary>
        /// Gets the actual timeout value, whether it's been set here or is using the default
        /// </summary>
        public float currentTimeOut
        {
            get
            {
                return overrideTimeout ? timeOut : MARSCore.instance.defaultEntityTimeout;
            }
        }

        public override string ToString()
        {
            const string str = "Re-acquire on Loss: {0}, Timeout: {1}, Match Interval: {2}, Priority: {3}";

            return string.Format(str, reacquireOnLoss, overrideTimeout ? "default" : timeOut.ToString(), updateMatchInterval, priority);
        }
    }
}
