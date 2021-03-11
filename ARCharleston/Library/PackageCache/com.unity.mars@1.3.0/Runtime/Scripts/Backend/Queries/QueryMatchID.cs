using System;

namespace Unity.MARS.Query
{
    /// <summary>
    /// Identifies a query and an instance of a match for this query
    /// </summary>
    public struct QueryMatchID : IEquatable<QueryMatchID>
    {
        static int s_IDCounter = 1;

        public static QueryMatchID NullQuery = new QueryMatchID(0,0);
        /// <summary>
        /// Identifies the query itself
        /// </summary>
        readonly int m_ID;

        /// <summary>
        /// Identifies an instance of a query match. This is distinct from the ID of the data that fulfills the query.
        /// </summary>
        readonly int m_MatchID;

        /// <summary>
        /// Identifies the query itself
        /// </summary>
        public int queryID { get { return m_ID; } }

        public int matchID { get { return m_MatchID; } }

        public static QueryMatchID Generate()
        {
            return new QueryMatchID(s_IDCounter++);
        }

        internal static void ResetCounter()
        {
            s_IDCounter = 1;
        }

        public QueryMatchID NextMatch()
        {
            return new QueryMatchID(m_ID, m_MatchID + 1);
        }

        QueryMatchID(int id)
        {
            m_ID = id;
            m_MatchID = 0;
        }

        internal QueryMatchID(int id, int matchID)
        {
            m_ID = id;
            m_MatchID = matchID;
        }

        /// <summary>
        /// Checks whether this ID refers to the same query as another ID
        /// </summary>
        /// <param name="otherID">QueryMatchID to compare against</param>
        /// <returns>True if both IDs refer to the same query, false otherwise</returns>
        public bool SameQuery(QueryMatchID otherID)
        {
            return m_ID == otherID.m_ID;
        }

        /// <summary>
        /// Checks whether this query ID has actually been initialized
        /// </summary>
        /// <returns></returns>
        public bool IsNullQuery()
        {
            return m_ID == 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return m_ID * 397 + m_MatchID;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is QueryMatchID))
                return false;

            return Equals((QueryMatchID)obj);
        }

        public bool Equals(QueryMatchID id)
        {
            return m_ID == id.m_ID && m_MatchID == id.m_MatchID;
        }

        public override string ToString()
        {
            return string.Format("ID:{0}, Match:{1}", m_ID, m_MatchID);
        }
    }
}
