using Unity.MARS.Attributes;
using Unity.MARS.Query;
using UnityEngine;

namespace Unity.MARS.Conditions
{
    /// <summary>
    /// Represents a situation that depends on the existence of a plane
    /// </summary>
    [DisallowMultipleComponent]
    [ComponentTooltip("Requires the object to be a body.")]
    [MonoBehaviourComponentMenu(typeof(IsBodyCondition), "Condition/Trait/Body")]
    public class IsBodyCondition : SimpleTagCondition
    {
        static readonly TraitRequirement[] k_RequiredTraits = { TraitDefinitions.Body };

        /// <summary>
        /// Get the TraitRequirements that are required by this condition
        /// </summary>
        /// <returns>The required traits</returns>
        public override TraitRequirement[] GetRequiredTraits() { return k_RequiredTraits; }
    }
}
