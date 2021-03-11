using System;
using Unity.MARS.Data;
using Unity.MARS.Query;
using UnityEngine;

namespace Unity.MARS
{
    public abstract class Condition : ConditionBase, ICondition
    {
        public string traitName
        {
            get
            {
                var traits = GetRequiredTraits();
                if (traits != null && traits.Length > 0)
                    return traits[0].TraitName;

                const string requiredTraitsFieldName = IRequiresTraitsMethods.StaticRequiredTraitsFieldName;
                Debug.LogError(
                    $"{GetType().Name}.GetRequiredTraits() must return at least one TraitRequirement. The first one defines " +
                     "the trait this Condition tests against, and any after that define an additional requirement for the simple presence of a trait." +
                    $" Your implementation of GetRequiredTraits() must return the field {requiredTraitsFieldName}.");

                return "";
            }
        }

        public abstract TraitRequirement[] GetRequiredTraits();
    }

    public abstract class Condition<T> : Condition, ICondition<T>
    {
        public abstract float RateDataMatch(ref T data);
    }
}
