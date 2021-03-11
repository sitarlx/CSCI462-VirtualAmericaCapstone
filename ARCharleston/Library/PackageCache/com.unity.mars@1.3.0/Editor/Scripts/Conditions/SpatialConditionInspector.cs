using Unity.MARS;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS
{
    [MovedFrom("Unity.MARS")]
    public abstract class SpatialConditionInspector : ConditionBaseInspector
    {
        protected Condition condition { get; private set; }

        public override void OnEnable()
        {
            condition = (Condition)target;
            base.OnEnable();
        }
    }
}
