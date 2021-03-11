using System;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Data
{
    /// <summary>
    /// A unique identifier for trackable data in MARS - mirrors `UnityEngine.Experimental.XR.TrackableId`
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [MovedFrom("Unity.MARS")]
    public struct MarsTrackableId : IEquatable<MarsTrackableId>
    {
        const ulong k_CreationSubIdOne = ulong.MaxValue;

        static readonly MarsTrackableId k_InvalidId;
        static readonly ulong k_StringSubIdOne = 1ul;
        static ulong s_IdTwoCounter;

        [SerializeField]
        ulong m_SubId1;

        [SerializeField]
        ulong m_SubId2;

        /// <summary>
        /// Represents an invalid id.
        /// </summary>
        public static MarsTrackableId InvalidId { get { return k_InvalidId; } }

        /// <summary>
        /// The first subcomponent of the id.
        /// </summary>
        public ulong subId1 { get { return m_SubId1; } }

        /// <summary>
        /// The second subcomponent of the id.
        /// </summary>
        public ulong subId2 { get { return m_SubId2; } }

        /// <summary>
        /// Generates a nicely formatted version of the id
        /// </summary>
        /// <returns>A string unique to this id</returns>
        public override string ToString()
        {
            return string.Format("{0}-{1}", m_SubId1.ToString("X16", CultureInfo.InvariantCulture),
                m_SubId2.ToString("X16", CultureInfo.InvariantCulture));
        }

        public override int GetHashCode()
        {
            return m_SubId1.GetHashCode() ^ m_SubId2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is MarsTrackableId && Equals((MarsTrackableId) obj);
        }

        public bool Equals(MarsTrackableId other)
        {
            return m_SubId1 == other.m_SubId1 && m_SubId2 == other.m_SubId2;
        }

        public static bool operator ==(MarsTrackableId id1, MarsTrackableId id2)
        {
            return id1.m_SubId1 == id2.m_SubId1 && id1.m_SubId2 == id2.m_SubId2;
        }

        public static bool operator !=(MarsTrackableId id1, MarsTrackableId id2)
        {
            return id1.m_SubId1 != id2.m_SubId1 || id1.m_SubId2 != id2.m_SubId2;
        }

        public MarsTrackableId(ulong idOne, ulong idTwo)
        {
            m_SubId1 = idOne;
            m_SubId2 = idTwo;
        }

        public MarsTrackableId(string trackableKey)
        {
            // the idea with this is that since we won't ever reach this id with the negative counter,
            // then it's safe to use it as a complementary key to the string's hash code
            m_SubId1 = k_StringSubIdOne;
            m_SubId2 = (ulong) trackableKey.GetHashCode();
        }

        /// <summary>
        /// Create a new trackable identifier
        /// </summary>
        /// <returns>A new session-unique trackable ID</returns>
        public static MarsTrackableId Create()
        {
            return new MarsTrackableId(k_CreationSubIdOne, s_IdTwoCounter++);
        }
    }
}
