using Unity.MARS;
using Unity.MARS.Authoring;
using UnityEngine;

namespace UnityEditor.MARS
{
    public class GenericPrefabObjectCreationData : ObjectCreationData
    {
#pragma warning disable 649
        [SerializeField]
        GameObject m_Prefab;
#pragma warning restore 649

        public override bool CreateGameObject(out GameObject createdObj, Transform parentTransform)
        {
            if (m_Prefab == null)
            {
                createdObj  = null;
                return false;
            }

            MARSSession.EnsureRuntimeState();

            var objName = GameObjectUtility.GetUniqueNameForSibling(null, m_ObjectName);
            createdObj = Instantiate(m_Prefab);
            MarsWorldScaleModule.ScaleChildren(createdObj.transform);

            foreach (var colorComponent in createdObj.GetComponentsInChildren<IHasEditorColor>())
            {
                colorComponent.SetNewColor(true, true);
            }

            createdObj.name = objName;
            Undo.RegisterCreatedObjectUndo(createdObj, $"Create {objName}");
            Selection.activeGameObject = createdObj;
            return true;
        }
    }
}
