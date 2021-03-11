using Unity.MARS.Query;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Data.Reasoning
{
    [CreateAssetMenu(menuName = "MARS/Geo Duplication ReasoningAPI")]
    [MovedFrom("Unity.MARS")]
    public class GeoDuplicationReasoningAPI : ScriptableObject, IReasoningAPI, IProvidesTraits<Vector2>,
        IRequiresTraits<Vector2>, IRequiresTraits<Pose>
    {
        const float k_ProcessSceneTime = 10f;
        static readonly TraitRequirement[] k_RequiredTraits = { TraitDefinitions.GeoCoordinate };
        static readonly TraitDefinition[] k_ProvidedTraits = new TraitDefinition[0];

        static Vector2 s_CurrentCoordinate;

        public float processSceneInterval => k_ProcessSceneTime;

        public TraitDefinition[] GetProvidedTraits() { return new TraitDefinition[0]; }

        public TraitRequirement[] GetRequiredTraits() { return k_RequiredTraits; }

        public void Setup() { ProcessScene(); }

        public void TearDown() { }

        public void ProcessScene()
        {
            // the immediate environment's geolocation trait should be set by the geo module if available
            if (!this.TryGetTraitValue((int) ReservedDataIDs.ImmediateEnvironment,
                TraitNames.Geolocation, out s_CurrentCoordinate))
                return;

            // duplicate geo location trait value to everything with a pose as a temporary solution to allowing geo queries
            this.ForEachContextIdWithTrait<Pose>(TraitNames.Pose, id =>
            {
                // this would be much more efficient if we had a way to batch-update a single trait's values,
                // but it shouldn't be a performance problem for this use case
                this.AddOrUpdateTrait(id, TraitNames.Geolocation, s_CurrentCoordinate);
            });
        }

        public void UpdateData() { }
    }
}
