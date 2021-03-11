using System;
using Unity.MARS;
using Unity.MARS.Authoring;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS
{
    [MovedFrom("Unity.MARS")]
    [Obsolete("ObjectCreationButtonDataGenericPrefab has been deprecated. (UnityUpgradable) -> GenericPrefabObjectCreationData", false)]
    public class ObjectCreationButtonDataGenericPrefab : ObjectCreationButtonData
    {
#pragma warning disable 649
        [SerializeField]
        GameObject m_Prefab;
#pragma warning restore 649

        [Obsolete("CreateGameObject() has been deprecated, please use CreateGameObject(out GameObject  createdObject, Transform parentTransform) instead",false)]
        public override bool CreateGameObject()
        {
            if (m_Prefab == null)
                return false;

            MARSSession.EnsureRuntimeState();

            var objName = GameObjectUtility.GetUniqueNameForSibling(null, m_ButtonName);
            var go = Instantiate(m_Prefab);
            MarsWorldScaleModule.ScaleChildren(go.transform);

            foreach (var colorComponent in go.GetComponentsInChildren<IHasEditorColor>())
            {
                colorComponent.SetNewColor(true, true);
            }

            go.name = objName;
            Undo.RegisterCreatedObjectUndo(go, $"Create {objName}");
            Selection.activeGameObject = go;
            return true;
        }
    }
}
