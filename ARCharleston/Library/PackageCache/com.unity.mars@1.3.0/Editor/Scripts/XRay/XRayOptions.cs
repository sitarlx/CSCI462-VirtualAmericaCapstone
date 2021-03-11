using Unity.MARS;
using Unity.MARS.Settings;
using Unity.XRTools.Utils;
using UnityEditor.XRTools.Utils;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Simulation.Rendering
{
    [ScriptableSettingsPath(MARSCore.UserSettingsFolder)]
    [MovedFrom("Unity.MARS")]
    public class XRayOptions : EditorScriptableSettings<XRayOptions>
    {
        [SerializeField]
        [Tooltip("Flip the Depth direction that the X-Ray clips the model in URP.")]
        bool m_FlipXRayDirection;

        public bool FlipXRayDirection
        {
            get { return m_FlipXRayDirection; }
            set
            {
                m_FlipXRayDirection = value;
                XRayModule.SetXRayDirectionKeyword();
            }
        }
    }
}
