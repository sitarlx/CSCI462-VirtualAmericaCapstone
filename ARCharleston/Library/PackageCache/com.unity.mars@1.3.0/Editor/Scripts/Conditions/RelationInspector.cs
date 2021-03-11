using System.Collections.Generic;
using Unity.MARS;
using Unity.MARS.Query;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS
{
    [MovedFrom("Unity.MARS")]
    public abstract class RelationInspector : ConditionBaseInspector
    {
        static readonly GUIContent k_Child1Content = new GUIContent("Child 1", "Specifies the first child object of this relation");
        static readonly GUIContent k_Child2Content = new GUIContent("Child 2", "Specifies the second child object of this relation");

        protected SerializedProperty m_Child1Property;
        protected SerializedProperty m_Child2Property;

        protected RelationBase relation { get; private set; }
        protected ProxyGroup proxyGroup { get; private set; }
        protected Transform child1Transform { get; private set; }
        protected Transform child2Transform { get; private set; }

        protected virtual GUIContent child1Content { get { return k_Child1Content; } }
        protected virtual GUIContent child2Content { get { return k_Child2Content; } }

        // Local method use only -- created here to reduce garbage collection. Collections must be cleared before use
        protected static readonly List<Proxy> k_ChildObjects = new List<Proxy>();
        protected static readonly List<GUIContent> k_ChildNames = new List<GUIContent>();

        public override void OnEnable()
        {
            relation = target as RelationBase;
            proxyGroup = relation.proxyGroup;
            m_Child1Property = serializedObject.FindProperty("m_Child1");
            m_Child2Property = serializedObject.FindProperty("m_Child2");

            base.OnEnable();
        }

        protected void RefreshChildren()
        {
            k_ChildObjects.Clear();
            k_ChildNames.Clear();

            proxyGroup.GetChildList(k_ChildObjects);
            foreach (var currentChild in k_ChildObjects)
            {
                if (currentChild == null)
                    continue;

                var name = string.Format(
                    ProxyGroupInspector.childNameString, currentChild.transform.GetSiblingIndex(), currentChild.name);
                k_ChildNames.Add(new GUIContent(name));
            }
        }

        protected bool DrawChildOptions()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                RefreshChildren();

                var child1 = (Proxy) m_Child1Property.objectReferenceValue;
                var child2 = (Proxy) m_Child2Property.objectReferenceValue;
                if (child1 == null || child2 == null)
                    EditorGUIUtils.Warning(
                        "This relation has an invalid child reference, so it will not apply to the set");

                var child1Index = k_ChildObjects.IndexOf(child1);
                var child2Index = k_ChildObjects.IndexOf(child2);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUIUtils.DrawCheckboxFillerRect();
                    var newIndex = EditorGUILayout.Popup(child1Content, child1Index, k_ChildNames.ToArray());
                    if (newIndex == child2Index)
                        child2Index = child1Index;

                    child1Index = newIndex;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUIUtils.DrawCheckboxFillerRect();
                    var newIndex = EditorGUILayout.Popup(child2Content, child2Index, k_ChildNames.ToArray());
                    if (newIndex == child1Index)
                        child1Index = child2Index;

                    child2Index = newIndex;
                }

                m_Child1Property.objectReferenceValue = (child1Index >= 0) ? k_ChildObjects[child1Index] : null;
                m_Child2Property.objectReferenceValue = (child2Index >= 0) ? k_ChildObjects[child2Index] : null;
                child1Transform = relation.child1Transform;
                child2Transform = relation.child2Transform;

                return check.changed;
            }
        }

        protected override void OnConditionInspectorGUI()
        {
            relation.EnsureChildClients();
            serializedObject.Update();

            if(DrawChildOptions())
                serializedObject.ApplyModifiedProperties();
        }

        protected override void OnConditionSceneGUI()
        {
            child1Transform = relation.child1Transform;
            child2Transform = relation.child2Transform;
        }
    }
}
