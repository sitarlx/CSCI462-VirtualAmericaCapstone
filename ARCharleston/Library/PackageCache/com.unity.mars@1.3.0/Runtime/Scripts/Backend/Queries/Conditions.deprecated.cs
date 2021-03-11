using System;
using UnityEngine;

namespace Unity.MARS.Query
{
    [Obsolete("Conditions has been deprecated. Use ProxyConditions instead.", true)]
    public partial class Conditions
    {
        public int Count => default;

        public Conditions(Proxy target) { }

        public static Conditions FromGenericIMRObject<TComponentRootType>(TComponentRootType target) where TComponentRootType : Component, IMRObject
        {
            return default;
        }

        public static Conditions FromGameObject<TComponentRootType>(GameObject target) where TComponentRootType : Component, IMRObject
        {
            return default;
        }

        public Conditions(Condition condition) { }

        public Conditions(ICondition[] conditions) { }

        public bool TryGetType<T>(out T[] conditions)
        {
            conditions = default;
            return default;
        }

        public bool TryGetType(out object[] conditions)
        {
            conditions = default;
            return default;
        }

        public int GetTypeCount(out object[] conditions)
        {
            conditions = default;
            return default;
        }
    }
}
