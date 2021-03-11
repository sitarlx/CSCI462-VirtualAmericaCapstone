using Unity.MARS.Attributes;
using Unity.MARS.Data;
using Unity.MARS.Query;
using Unity.XRTools.Utils;
using UnityEngine;

namespace Unity.MARS.Actions
{
    [RequireComponent(typeof(Proxy))]
    [ComponentTooltip("Syncs the GameObject's position with the real world object and aligns the long side to the X-axis.")]
    [MonoBehaviourComponentMenu(typeof(SetAlignedPoseAction), "Action/Set Aligned Pose")]
    public class SetAlignedPoseAction : TransformAction, IMatchAcquireHandler, IMatchUpdateHandler, IRequiresTraits
    {
        [SerializeField]
        [Tooltip("When enabled, movement of the matched data, such as surface-resizing, will be followed")]
        public bool FollowMatchUpdates = true;

        static readonly TraitRequirement[] k_RequiredTraits =
        {
            TraitDefinitions.Pose,
            new TraitRequirement(TraitDefinitions.Bounds2D, false)
        };

        static readonly Quaternion k_RotateQuarter = Quaternion.Euler(0.0f, 90.0f, 0.0f);

        public void OnMatchAcquire(QueryResult queryResult)
        {
            UpdatePosition(queryResult);
        }

        public void OnMatchUpdate(QueryResult queryResult)
        {
            if (FollowMatchUpdates)
            {
                UpdatePosition(queryResult);
            }
        }

        void UpdatePosition(QueryResult queryResult)
        {
            if (queryResult.TryGetTrait(TraitNames.Pose, out Pose newPose))
            {
                if (queryResult.TryGetTrait(TraitNames.Bounds2D, out Vector2 bounds))
                {
                    if (bounds.x < bounds.y)
                        newPose.rotation *= k_RotateQuarter;
                }
                transform.SetWorldPose(newPose);
            }
        }

        public TraitRequirement[] GetRequiredTraits()
        {
            return k_RequiredTraits;
        }
    }
}
