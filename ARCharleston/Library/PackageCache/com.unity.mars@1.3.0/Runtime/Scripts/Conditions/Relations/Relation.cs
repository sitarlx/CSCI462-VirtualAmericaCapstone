using Unity.MARS.Authoring;
using Unity.MARS.Data;
using Unity.MARS.Query;
using UnityEngine;

namespace Unity.MARS
{
    public abstract class Relation : RelationBase, IRelation
    {
        public string child1TraitName
        {
            get
            {
                var traits = GetRequiredTraits();
                if (traits != null && traits.Length == 2)
                    return traits[0].TraitName;

                LogRequiredTraitsError();
                return "";
            }
        }

        public string child2TraitName
        {
            get
            {
                var traits = GetRequiredTraits();
                if (traits != null && traits.Length == 2)
                    return traits[1].TraitName;

                LogRequiredTraitsError();
                return "";
            }
        }

        public abstract TraitRequirement[] GetRequiredTraits();

        void LogRequiredTraitsError()
        {
            const string requiredTraitsFieldName = IRequiresTraitsMethods.StaticRequiredTraitsFieldName;
            Debug.LogError(
                $"{GetType().Name}.GetRequiredTraits() must return exactly two TraitRequirements - the first " +
                "defines the trait for the Relation's first child, and the second defines the trait for the second child. " +
                $"Your implementation of GetRequiredTraits() must return the field {requiredTraitsFieldName}. " +
                "ConditionsAnalyzer should check that this field contains exactly two TraitRequirements.");
        }
    }

    public abstract class Relation<T> : Relation, IRelation<T>, ICreateFromDataPair
    {
        public abstract float RateDataMatch(ref T child1Data, ref T child2Data);
        public virtual void OptimizeForData(TraitDataSnapshot child1Data, TraitDataSnapshot child2Data) { }
        public virtual void ConformToData(TraitDataSnapshot child1Data, TraitDataSnapshot child2Data) { }
        public virtual string FormatDataString(TraitDataSnapshot child1Data, TraitDataSnapshot child2Data) { return $"{child1Data} {child2Data}"; }
    }
}
