using System;
using Unity.MARS.Data;
using Unity.MARS.Data.Recorded;
using Unity.MARS.Providers;
using Unity.XRTools.ModuleLoader;
using Unity.XRTools.Utils;
using UnityEngine;

namespace Unity.MARS.Recording.Providers
{
    [ProviderSelectionOptions(ProviderPriorities.RecordedProviderPriority, disallowAutoCreation:true)]
    public class RecordedCameraPoseProvider : MonoBehaviour, IProvidesCameraPose, ISteppableRecordedDataProvider
    {
#pragma warning disable 67
        public event Action<Pose> poseUpdated;
        public event Action<MRCameraTrackingState> trackingStateChanged;
#pragma warning restore 67

        void IFunctionalityProvider.LoadProvider() { }

        void IFunctionalityProvider.ConnectSubscriber(object obj)
        {
            this.TryConnectSubscriber<IProvidesCameraPose>(obj);
        }

        void IFunctionalityProvider.UnloadProvider() { }

        public Pose GetCameraPose() { return transform.GetLocalPose(); }

        public void ClearData() { }

        public void StepRecordedData()
        {
            if (poseUpdated != null)
                poseUpdated(GetCameraPose());
        }
    }
}
