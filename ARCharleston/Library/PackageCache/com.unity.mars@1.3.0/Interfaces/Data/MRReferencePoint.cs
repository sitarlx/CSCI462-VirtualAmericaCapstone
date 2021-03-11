using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Data
{
    /// <summary>
    /// Provides a template for tracked reference point data
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public struct MRReferencePoint : IMRTrackable, IEquatable<MRReferencePoint>
    {
        /// <summary>
        /// The id of this reference point as determined by the provider
        /// </summary>
        public MarsTrackableId id { get; set; }

        /// <summary>
        /// The pose of this reference point
        /// </summary>
        public Pose pose { get; set; }

        /// <summary>
        /// The tracking state of this tracked object
        /// </summary>
        public MARSTrackingState trackingState { get; set; }

        public MRReferencePoint(Pose pose, MARSTrackingState state = MARSTrackingState.Unknown) : this()
        {
            id = MarsTrackableId.Create();
            trackingState = state;
            this.pose = pose;
        }

        public override int GetHashCode() { return id.GetHashCode(); }

        public bool Equals(MRReferencePoint other) { return id.Equals(other.id); }
    }
}
