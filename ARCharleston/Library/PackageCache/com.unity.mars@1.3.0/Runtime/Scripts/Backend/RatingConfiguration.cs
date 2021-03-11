using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Conditions
{
    /// <summary>
    /// Defines how a condition's match rating will be evaluated
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public struct RatingConfiguration
    {
        public const float MaxDeadZone = 0.99f;
        // if the center is 0 it can mess up the calculation and produce NaN errors, so prevent that
        public const float MinimumCenter = 0.001f;
        public const float MaximumCenter = 0.999f;

        // accessing the match config each time we call RateDataMatch turned out to be a significant portion
        // of the time spent in that function. instead of allowing setters, we make these readonly so we can
        // still get the speed advantage of having them be fields, while maintaining the clamping

        /// <summary>
        /// Defines the point in the 0-1 range which will receive the highest possible match rating
        /// </summary>
        public readonly float center;

        /// <summary>
        /// Defines a portion of the range around the center, inside which the data is considered a perfect match
        /// </summary>
        public readonly float deadZone;

        public RatingConfiguration(float deadZone, float center = 0.5f)
        {
            this.deadZone = Mathf.Clamp(deadZone, 0f, MaxDeadZone);
            this.center = Mathf.Clamp(center, MinimumCenter, MaximumCenter);
        }
    }
}
