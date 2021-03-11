#if ARFOUNDATION_2_1_OR_NEWER
﻿using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.ARSubsystems;

namespace Unity.MARS.Data.ARFoundation
{
    [MovedFrom("Unity.MARS.Providers")]
    public static class TrackableIdExtensions
    {
        public static MarsTrackableId ToMarsId(this TrackableId id)
        {
            return new MarsTrackableId(id.subId1, id.subId2);
        }
    }
}
#endif
