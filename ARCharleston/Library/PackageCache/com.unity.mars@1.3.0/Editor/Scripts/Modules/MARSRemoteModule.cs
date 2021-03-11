using Unity.MARS.Providers;
using Unity.XRTools.ModuleLoader;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Simulation
{
    /// <summary>
    /// Module for managing and interfacing with a remote data connection in the editor
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public class MARSRemoteModule : IModuleBehaviorCallbacks, IUsesRemoteDataConnection
    {
        IProvidesRemoteDataConnection IFunctionalitySubscriber<IProvidesRemoteDataConnection>.provider { get; set; }

        bool m_HasRemoteModule;

        void IModule.LoadModule()
        {
            m_HasRemoteModule = this.HasProvider<IProvidesRemoteDataConnection>();
        }

        void IModule.UnloadModule()
        {
            RemoteDisconnect();
        }

        void IModuleBehaviorCallbacks.OnBehaviorAwake() { }

        void IModuleBehaviorCallbacks.OnBehaviorEnable() { }

        void IModuleBehaviorCallbacks.OnBehaviorStart() { }

        void IModuleBehaviorCallbacks.OnBehaviorUpdate()
        {
            RemoteUpdate();
        }

        void IModuleBehaviorCallbacks.OnBehaviorDisable()
        {
            RemoteDisconnect();
        }

        void IModuleBehaviorCallbacks.OnBehaviorDestroy() { }

        /// <summary>
        /// Is the module connected an editor remote
        /// </summary>
        public bool RemoteActive => m_HasRemoteModule && this.IsConnected();

        /// <summary>
        /// Connect the module to an editor remote
        /// </summary>
        public void RemoteConnect()
        {
            if (m_HasRemoteModule)
                this.ConnectRemote();
        }

        /// <summary>
        /// Disconnect from the editor remote
        /// </summary>
        public void RemoteDisconnect()
        {
            if (m_HasRemoteModule)
                this.DisconnectRemote();
        }

        /// <summary>
        /// Update the connection to the editor remote
        /// </summary>
        public void RemoteUpdate()
        {
            if (m_HasRemoteModule)
                this.UpdateRemote();
        }
    }
}
