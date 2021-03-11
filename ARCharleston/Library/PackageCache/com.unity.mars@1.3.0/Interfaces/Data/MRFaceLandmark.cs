using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Data
{
    /// <summary>
    /// Enumerates the types of tracked face landmarks
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public enum MRFaceLandmark
    {
        LeftEye,
        RightEye,
        LeftEyebrow,
        RightEyebrow,
        NoseBridge,
        NoseTip,
        Mouth,
        UpperLip,
        LowerLip,
        LeftEar,
        RightEar,
        Chin
    }

    [MovedFrom("Unity.MARS")]
    public class MRFaceLandmarkComparer : IEqualityComparer<MRFaceLandmark>
    {
        public bool Equals(MRFaceLandmark x, MRFaceLandmark y)
        {
            return x == y;
        }

        public int GetHashCode(MRFaceLandmark obj)
        {
            return (int)obj;
        }
    }
}
