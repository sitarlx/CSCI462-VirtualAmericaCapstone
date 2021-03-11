using System;
using Unity.MARS.Attributes;
using Unity.MARS.Query;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Actions
{
    [Serializable]
    [Event(typeof(MatchResultEvent))]
    [MovedFrom("Unity.MARS")]
    public class MatchResultEvent : UnityEvent<QueryResult> { }

    [Serializable]
    [Event(typeof(MatchArgsEvent))]
    [MovedFrom("Unity.MARS")]
    public class MatchArgsEvent : UnityEvent<QueryArgs> { }

    [RequireComponent(typeof(Proxy))]
    [MonoBehaviourComponentMenu(typeof(MatchAction), "Action/Match Action")]
    [MovedFrom("Unity.MARS")]
    public class MatchAction : MonoBehaviour, IMatchAcquireHandler, IMatchUpdateHandler, IMatchLossHandler,
        IMatchTimeoutHandler
    {
        [SerializeField]
        MatchResultEvent m_OnMatchAcquire = new MatchResultEvent();
        [SerializeField]
        MatchResultEvent m_OnMatchUpdate = new MatchResultEvent();
        [SerializeField]
        MatchResultEvent m_OnMatchLoss = new MatchResultEvent();
        [SerializeField]
        MatchArgsEvent m_OnMatchTimeout = new MatchArgsEvent();

        public void OnMatchAcquire(QueryResult queryResult)
        {
            m_OnMatchAcquire.Invoke(queryResult);
        }

        public void OnMatchUpdate(QueryResult queryResult)
        {
            m_OnMatchUpdate.Invoke(queryResult);
        }

        public void OnMatchLoss(QueryResult queryResult)
        {
            m_OnMatchLoss.Invoke(queryResult);
        }

        public void OnMatchTimeout(QueryArgs queryArgs)
        {
            m_OnMatchTimeout.Invoke(queryArgs);
        }
    }
}
