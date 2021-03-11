using System;
using UnityEditor.MARS.Simulation;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS
{
    [CreateAssetMenu(fileName = "Templates", menuName = "MARS/Template Collection")]
    [MovedFrom("Unity.MARS")]
    public class TemplateCollection : ScriptableObject
    {
#pragma warning disable 649
        [SerializeField]
        TemplateData[] m_Templates;
#pragma warning restore 649

        public TemplateData[] templates { get { return m_Templates; } }
    }

    [Serializable]
    [MovedFrom("Unity.MARS")]
    public struct TemplateData
    {
        public string name;
        public SceneAsset scene;
        public Texture2D icon;
        public EnvironmentMode environmentMode;
    }
}
