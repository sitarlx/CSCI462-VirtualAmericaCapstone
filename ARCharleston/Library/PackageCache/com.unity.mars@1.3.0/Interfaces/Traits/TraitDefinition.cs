using System;
using Unity.MARS.Query;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Data
{
    [MovedFrom("Unity.MARS")]
    public class TraitDefinition : IEquatable<TraitDefinition>, IEquatable<TraitRequirement>
    {
        /// <summary>
        /// The name of the trait
        /// </summary>
        public readonly string TraitName;

        /// <summary>
        /// The type of the trait's value
        /// </summary>
        public readonly Type Type;

        public TraitDefinition(string traitName, Type type)
        {
            TraitName = traitName;
            Type = type;
        }

        public bool Equals(TraitDefinition other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(TraitName, other.TraitName) && Type == other.Type;
        }

        public bool Equals(TraitRequirement obj)
        {
            if (obj == null)
                return false;

            return obj.TraitName == TraitName && obj.Type == Type;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TraitName.GetHashCode() * 397) ^ Type.GetHashCode();
            }
        }

        public override string ToString() { return $"{TraitName} - {Type.Name}"; }
    }
}
