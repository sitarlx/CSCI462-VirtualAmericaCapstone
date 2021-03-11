using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Data
{
    /// <summary>
    /// Enumerates the types of tracked body landmarks
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public enum MRBodyLandmark
    {
        Head,
        Body
    }

    [MovedFrom("Unity.MARS")]
    public class MRBodyLandmarkComparer : IEqualityComparer<MRBodyLandmark>
    {
        public bool Equals(MRBodyLandmark x, MRBodyLandmark y)
        {
            return x == y;
        }

        public int GetHashCode(MRBodyLandmark obj)
        {
            return (int)obj;
        }
    }
}
