#if ARFOUNDATION_2_1_OR_NEWER
﻿using Unity.XRTools.Utils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Unity.MARS.Data.ARFoundation
{
    [MovedFrom("Unity.MARS.Providers")]
    public static class ARTrackedImageExtensions
    {
        public static MRMarker ToMRMarker(this ARTrackedImage trackedImage)
        {
            var mrMarker = new MRMarker
            {
                id = trackedImage.trackableId.ToMarsId(),
                pose = trackedImage.transform.GetWorldPose(),
                markerId = trackedImage.referenceImage.guid,
                extents = trackedImage.extents,
                trackingState = ArfTrackingStateToMars(trackedImage.trackingState)
            };

            return mrMarker;
        }

        // instead of just casting, explicitly specify conversion in case the definition of ARF's enum changes
        static MARSTrackingState ArfTrackingStateToMars(TrackingState state)
        {
            switch (state)
            {
                case TrackingState.None: return MARSTrackingState.Unknown;
                case TrackingState.Limited: return MARSTrackingState.Limited;
                case TrackingState.Tracking: return MARSTrackingState.Tracking;
                default: return MARSTrackingState.Unknown;
            }
        }
    }
}
#endif
