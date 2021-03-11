using Unity.XRTools.Utils;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Settings
{
    [ScriptableSettingsPath(MARSCore.UserSettingsFolder)]
    [MovedFrom("Unity.MARS")]
    public class MarsDebugSettings : ScriptableSettings<MarsDebugSettings>
    {
#pragma warning disable 649
        [SerializeField]
        bool m_SceneModuleLogging;

        [SerializeField]
        bool m_GeolocationModuleLogging;

        [SerializeField]
        bool m_QuerySimulationModuleLogging;

        [SerializeField]
        bool m_SimObjectsManagerLogging;

        [SerializeField]
        bool m_SimPlaneFindingLogging;

        [SerializeField]
        bool m_AllowInteractionTargetSelection;

        [SerializeField]
        bool m_SimDiscoveryPointCloudDebug;

        [SerializeField]
        bool m_SimDiscoveryPlaneVerticesDebug;

        [SerializeField]
        bool m_SimDiscoveryPlaneExtentsDebug;

        [SerializeField]
        bool m_SimDiscoveryPlaneCenterDebug;

        [SerializeField]
        bool m_SimDiscoveryVoxelsDebug;

        [SerializeField]
        bool m_SimDiscoveryImageMarkerDebug;
#pragma warning restore 649

        [SerializeField]
        float m_SimDiscoveryPointCloudRayGizmoTime = 0.5f;

        public static bool SceneModuleLogging => instance.m_SceneModuleLogging;

        public static bool GeoLocationModuleLogging => instance.m_GeolocationModuleLogging;

        public static bool QuerySimulationModuleLogging => instance.m_QuerySimulationModuleLogging;

        public static bool SimObjectsManagerLogging => instance.m_SimObjectsManagerLogging;

        public static bool SimPlaneFindingLogging => instance.m_SimPlaneFindingLogging;

        public static bool SimDiscoveryPointCloudDebug => instance.m_SimDiscoveryPointCloudDebug;
        public static float SimDiscoveryPointCloudRayGizmoTime => instance.m_SimDiscoveryPointCloudRayGizmoTime;
        public static bool SimDiscoveryModulePlaneVerticesDebug => instance.m_SimDiscoveryPlaneVerticesDebug;
        public static bool SimDiscoveryModulePlaneExtentsDebug => instance.m_SimDiscoveryPlaneExtentsDebug;
        public static bool SimDiscoveryModulePlaneCenterDebug => instance.m_SimDiscoveryPlaneCenterDebug;
        public static bool SimDiscoveryVoxelsDebug => instance.m_SimDiscoveryVoxelsDebug;
        public static bool SimDiscoveryImageMarkerDebug => instance.m_SimDiscoveryImageMarkerDebug;

        public static bool allowInteractionTargetSelection => instance.m_AllowInteractionTargetSelection;
    }
}
