using System;
using UnityEngine;

namespace Unity.MARS.Companion.Core
{
    [Serializable]
    class DataUsageOrganization
    {
#pragma warning disable 649
        // ReSharper disable InconsistentNaming
        [SerializeField]
        long storageCap;

        [SerializeField]
        DataUsageProject[] projects;
        // ReSharper restore InconsistentNaming
#pragma warning restore 649

        public long StorageCap => storageCap;
        public DataUsageProject[] Projects => projects;
    }
}
