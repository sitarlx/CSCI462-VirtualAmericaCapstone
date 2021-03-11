using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Authoring
{
    [Serializable]
    [MovedFrom("Unity.MARS")]
    public enum AxisEnum
    {
        None = -1,
        XUp = 0,
        XDown,
        YUp,
        YDown,
        ZUp,
        ZDown,
        Custom
    }

    [MovedFrom("Unity.MARS")]
    public class PlacementOverride : MonoBehaviour
    {
        /// <summary>
        /// Optional boolean value that can override a value or have no effect
        /// </summary>
        public enum BooleanOverride
        {
            None, True, False
        }

        [SerializeField]
        BooleanOverride m_SnapToPivotOverride = BooleanOverride.None;

        [SerializeField]
        BooleanOverride m_OrientToSurfaceOverride = BooleanOverride.None;

        [SerializeField]
        AxisEnum m_AxisOverride = AxisEnum.YUp;

        [SerializeField]
        Vector3 m_CustomAxisOverride = Vector3.up;

        [SerializeField]
        [HideInInspector]
        Vector3 m_CustomAxisOverrideNormalized = Vector3.up;

        public bool useSnapToPivotOverride { get { return m_SnapToPivotOverride != BooleanOverride.None; } }

        /// <summary>
        /// If not set to None, objects placed on this target will always use the specified setting for snapping to the pivot
        /// </summary>
        public bool snapToPivotOverride { get { return m_SnapToPivotOverride == BooleanOverride.True; } }

        public bool useOrientToSurfaceOverride { get { return m_OrientToSurfaceOverride != BooleanOverride.None; } }

        /// <summary>
        /// If not set to None, objects placed on this target will always use the specified setting for orienting to the surface
        /// </summary>
        public bool orientToSurfaceOverride { get { return m_OrientToSurfaceOverride == BooleanOverride.True; } }

        public bool useAxisOverride { get { return m_AxisOverride != AxisEnum.None; } }

        public AxisEnum axisOverride { get { return m_AxisOverride; } }

        /// <summary>
        /// If there is an axis override, then this will contain that value, otherwise it will be null
        /// </summary>
        public Vector3 axisOverrideVector
        {
            get
            {
                var axis = Vector3.zero;
                switch (m_AxisOverride)
                {
                    case AxisEnum.XUp:
                        axis = Vector3.right;
                        break;
                    case AxisEnum.XDown:
                        axis = Vector3.left;
                        break;
                    case AxisEnum.YUp:
                        axis = Vector3.up;
                        break;
                    case AxisEnum.YDown:
                        axis = Vector3.down;
                        break;
                    case AxisEnum.ZUp:
                        axis = Vector3.forward;
                        break;
                    case AxisEnum.ZDown:
                        axis = Vector3.back;
                        break;
                    case AxisEnum.Custom:
                        axis = m_CustomAxisOverrideNormalized;
                        break;
                }

                return axis;
            }
        }

        void OnValidate()
        {
            if (m_CustomAxisOverride == Vector3.zero)
            {
                m_CustomAxisOverride = Vector3.up;
                m_CustomAxisOverrideNormalized = Vector3.up;
                Debug.LogWarning("Custom Axis Override cannot be Zero! Must specify a direction.");
            }
            else
                m_CustomAxisOverrideNormalized = Vector3.Normalize(m_CustomAxisOverride);
        }
    }
}
