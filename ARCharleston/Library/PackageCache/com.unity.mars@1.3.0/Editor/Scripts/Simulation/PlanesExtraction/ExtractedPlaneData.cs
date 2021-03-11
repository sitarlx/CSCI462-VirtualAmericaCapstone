using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Data.Synthetic
{
    [MovedFrom("Unity.MARS")]
    public struct ExtractedPlaneData
    {
        public List<Vector3> vertices;
        public Pose pose;
        public Vector3 center;
        public Vector2 extents;
    }
}
