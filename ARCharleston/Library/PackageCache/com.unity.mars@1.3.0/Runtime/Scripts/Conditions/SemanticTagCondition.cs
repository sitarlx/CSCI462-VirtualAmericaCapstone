using Unity.MARS.Attributes;
using Unity.MARS.Authoring;
using Unity.MARS.Data;
using Unity.MARS.Query;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Conditions
{
    /// <summary>
    /// Represents a situation that depends on the existence or lack of a certain trait
    /// </summary>
    [ComponentTooltip("Requires the object to have or lack the specified trait.")]
    [MonoBehaviourComponentMenu(typeof(SemanticTagCondition), "Condition/Semantic Tag")]
    [CreateFromDataOptions(0, true)]
    [MovedFrom("Unity.MARS")]
    public class SemanticTagCondition : Condition<bool>, ISemanticTagCondition, ICreateFromData
    {
        [Delayed]
        [SerializeField]
        [Tooltip("Sets the name of the trait that must be present or not")]
        string m_TraitName;

        [SerializeField]
        [Tooltip("Whether to require a semantic tag to be present or be excluded")]
        SemanticTagMatchRule m_MatchRule;

        readonly TraitRequirement[] m_RequiredTraits = new TraitRequirement[1];

        public SemanticTagMatchRule matchRule
        {
            get { return m_MatchRule; }
            set { m_MatchRule = value; }
        }

        public void SetTraitName(string newName)
        {
            m_TraitName = newName;
            SetTraitRequirement();
        }

        void SetTraitRequirement()
        {
            // if we have a condition that is excluding a tag, it can't be a required trait,
            // or else it will filter itself out and never match.
            var required = m_MatchRule != SemanticTagMatchRule.Exclude;
            m_RequiredTraits[0] = new TraitRequirement(m_TraitName, typeof(bool), required);
        }

        public override TraitRequirement[] GetRequiredTraits()
        {
            var requirement = m_RequiredTraits[0];
            if (requirement == null || !string.Equals(requirement.TraitName, m_TraitName))
                SetTraitRequirement();

            return m_RequiredTraits;
        }

        // tag conditions have binary pass / fail answers
        public override float RateDataMatch(ref bool data)
        {
            // if this is an exclusive tag, we want to add only failures to the ratings dict
            return data ? 1f : 0f;
        }

        public void OptimizeForData(TraitDataSnapshot data)
        {
            if (m_TraitName != null && data.TryGetTrait(m_TraitName, out bool value))
                m_MatchRule = value ? SemanticTagMatchRule.Match : SemanticTagMatchRule.Exclude;
            else
                enabled = false;
        }

        public void IncludeData(TraitDataSnapshot data)
        {
            if (m_TraitName != null && data.TryGetTrait(m_TraitName, out bool value))
                m_MatchRule = value ? SemanticTagMatchRule.Match : SemanticTagMatchRule.Exclude;
            else
                m_MatchRule = SemanticTagMatchRule.Exclude;
        }

        public string FormatDataString(TraitDataSnapshot data)
        {
            if (m_TraitName != null && data.TryGetTrait(m_TraitName, out bool value))
                return $"Tagged {(value ? "" : "not ")}{m_TraitName}";

            return  $"No {m_TraitName} tag";
        }

        public float GetConditionRatingForData(TraitDataSnapshot data)
        {
            if (m_TraitName != null && data.TryGetTrait(m_TraitName, out bool _))
                return m_MatchRule == SemanticTagMatchRule.Exclude ? 0f : 1f;

            return m_MatchRule == SemanticTagMatchRule.Exclude ? 1f : 0f;
        }
    }
}
