using System;
using Unity.MARS.Authoring;
using UnityEditor;
using UnityEditor.MARS;
using UnityEngine;

namespace Unity.MARS
{
    [Serializable]
    [Obsolete("Unity.MARS.ObjectCreationButtonData has been deprecated. Use UnityEditor.MARS.ObjectCreationData instead (UnityUpgradable) -> UnityEditor.MARS.ObjectCreationData", false)]
    public abstract class ObjectCreationButtonData : ScriptableObject
    {
        public enum CreateInContext
        {
            Scene,
            SyntheticSimulation
        }

        [SerializeField]
        protected string m_ButtonName = "Name not set";

        [SerializeField]
        protected DarkLightIconPair m_Icon;

        [SerializeField]
        protected string m_Tooltip;

        [SerializeField]
        CreateInContext m_CreateInContext = CreateInContext.Scene;

        public CreateInContext CreateInContextSelection => m_CreateInContext;

        [Obsolete("ButtonName has been deprecated. Use ObjectName property instead (UnityUpgradable) -> ObjectName", false)]
        public string ButtonName => m_ButtonName;

        public GUIContent ObjectGUIContent { get; private set; }

        internal void UpdateObjectGUIContent()
        {
            ObjectGUIContent.image = m_Icon.Icon;
            ObjectGUIContent.text = m_ButtonName;
            ObjectGUIContent.tooltip = m_Tooltip;
        }

        [Obsolete("CreationButtonContent() has been deprecated.", false)]
        public GUIContent CreationButtonContent() { return ObjectGUIContent; }

        [Obsolete("CreateGameObject() has been deprecated, please use CreateGameObject(out GameObject  createdObject, Transform parentTransform) instead",false)]
        public abstract bool CreateGameObject();

        [Obsolete("GenerateInitialGameObject(string objName) has been deprecated, please use GenerateInitialGameObject(string objName, Transform parent)", false)]
        protected static GameObject GenerateInitialGameObject(string objName)
        {
            var go = new GameObject(GameObjectUtility.GetUniqueNameForSibling(null, objName));
            MarsWorldScaleModule.ScaleChildren(go.transform);

            foreach (var colorComponent in go.GetComponentsInChildren<IHasEditorColor>())
            {
                colorComponent.SetNewColor(true, true);
            }

            return go;
        }
    }
}
