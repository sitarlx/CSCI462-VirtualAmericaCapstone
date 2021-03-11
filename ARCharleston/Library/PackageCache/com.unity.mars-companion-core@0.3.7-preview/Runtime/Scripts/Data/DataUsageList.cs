using System;
using UnityEngine;

namespace Unity.MARS.Companion.Core
{
    [Serializable]
    class DataUsageList
    {
#pragma warning disable 649
        // ReSharper disable InconsistentNaming
        [SerializeField]
        DataUsageOrganization[] organizations;

        [SerializeField]
        string[] errors;
        // ReSharper restore InconsistentNaming
#pragma warning restore 649

        public DataUsageOrganization[] Organizations => organizations;
        public string[] Errors => errors;
    }
}
