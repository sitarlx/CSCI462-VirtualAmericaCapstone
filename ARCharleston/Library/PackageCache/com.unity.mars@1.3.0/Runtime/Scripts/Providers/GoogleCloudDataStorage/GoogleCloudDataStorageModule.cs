using System;
using Unity.MARS.Data;
using Unity.MARS.Settings;
using Unity.XRTools.ModuleLoader;
using Unity.XRTools.Utils;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Providers.Google
{
    // Ensure earliest possible load order so that this deprecated module does not override others trying to provide
    // the same functionality
    [ModuleOrder(int.MinValue)]
    [ScriptableSettingsPath(MARSCore.UserSettingsFolder)]
    [MovedFrom("Unity.MARS")]
    [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
    public class GoogleCloudDataStorageModule : ScriptableSettings<GoogleCloudDataStorageModule>,
        IModuleBehaviorCallbacks, IProvidesCloudDataStorage
    {
#pragma warning disable 649, 414
        [SerializeField]
        bool m_SyncAPIKey = true;

        [SerializeField]
        string m_ProjectIdentifier;

        [SerializeField]
        string m_APIKey;

        [SerializeField]
        bool m_OfflineMode;

        [SerializeField]
        float m_TestingDelay;
#pragma warning restore 649, 414

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void SetAPIKey(string key) { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public string GetAPIKey() { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void SetProjectIdentifier(string id) { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public string GetProjectIdentifier() { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void CloudSaveAsync(string key, string serializedObject, Action<bool, long, string> callback = null, ProgressCallback progress = null) { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void CloudSaveAsync(string key, byte[] bytesObject, Action<bool, long, string> callback = null, ProgressCallback progress = null) { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void CloudLoadAsync(string key, Action<bool, long, string> callback, ProgressCallback progress = null) { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void CloudLoadAsync(string key, Action<bool, long, byte[]> callback, ProgressCallback progress = null) { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void CloudLoadTextureAsync(string key, LoadTextureCallback callback, ProgressCallback progress = null) { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void LoadLocalTextureAsync(string path, LoadTextureCallback callback, ProgressCallback progress = null) { throw new NotImplementedException(); }

        void IModule.LoadModule() { }
        void IModule.UnloadModule() { }
        void IModuleBehaviorCallbacks.OnBehaviorAwake() { }
        void IModuleBehaviorCallbacks.OnBehaviorEnable() { }
        void IModuleBehaviorCallbacks.OnBehaviorStart() { }
        void IModuleBehaviorCallbacks.OnBehaviorUpdate() { }
        void IModuleBehaviorCallbacks.OnBehaviorDisable() { }
        void IModuleBehaviorCallbacks.OnBehaviorDestroy() { }
        void IFunctionalityProvider.LoadProvider() { }
        void IFunctionalityProvider.ConnectSubscriber(object obj) { this.TryConnectSubscriber<IProvidesCloudDataStorage>(obj); }
        void IFunctionalityProvider.UnloadProvider() { }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public bool IsConnected() { throw new NotImplementedException(); }

        [Obsolete(IUsesCloudDataStorageMethods.ObsoleteMessage)]
        public void AuthenticatedGetRequest(string uri, string authToken, Action<bool, string> callback) { throw new NotImplementedException(); }
    }
}
