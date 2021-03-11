using System;
using Unity.ListViewFramework;
using UnityEngine;

namespace Unity.MARS.Companion.Core
{
    [Serializable]
    class DataUsageProject : IListViewItemData<string>
    {
        const string k_Template = "DataUsageListViewItem";

#pragma warning disable 649
        // ReSharper disable InconsistentNaming
        [SerializeField]
        string name;

        [SerializeField]
        string id;

        [SerializeField]
        long storageUsage;
        // ReSharper restore InconsistentNaming
#pragma warning restore 649

        public string index => id;
        public bool selected => false;
        public string template => k_Template;
        public string Name => name;
        public long StorageUsage => storageUsage;
        public long SizeAllowance { get; set; }
    }
}
