using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Simulation.Rendering
{
    [Serializable]
    [MovedFrom("Unity.MARS")]
    public enum ContextViewType
    {
        Undefined = -1,
        SimulationView = 0,
        DeviceView,
        NormalSceneView,
        PrefabIsolation,
        GameView,
        OtherView,
    }

    [MovedFrom("Unity.MARS")]
    internal interface ICompositeView
    {
        ContextViewType ContextViewType { get; }

        Camera TargetCamera { get; }

        RenderTextureDescriptor CameraTargetDescriptor { get; }

        Color BackgroundColor { get; }

        bool ShowImageEffects { get; }

        bool BackgroundSceneActive { get; }

        bool DesaturateComposited { get; }

        bool UseXRay { get; }
    }
}
