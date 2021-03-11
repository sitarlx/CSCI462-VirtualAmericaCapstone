using Unity.MARS.Data;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Query
{
    /// <summary>
    /// A common list of trait names for consistency
    /// This is not an exhaustive list of all possible traits, but simply provides a reference of traits that can be
    /// easily renamed and universally agreed-upon. It is quite possible to use trait names that are not specified here.
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public static class TraitNames
    {
        public const string Pose = "pose";
        public const string Point = "point";
        public const string Bounds2D = "bounds2d";
        public const string Alignment = "alignment";
        public const string Geolocation = "geolocation";
        public const string HeightAboveFloor = "heightAboveFloor";
        public const string DisplayFlat = "displayFlat";
        public const string DisplaySpatial = "displaySpatial";
        public const string TrackingState = "trackingState";

        // Semantic tag traits
        public const string Plane = "plane";
        public const string Face = "face";
        public const string Floor = "floor";
        public const string Environment = "environment";
        public const string User = "user";
        public const string InView = "inView";
        public const string Marker = "marker";
        public const string MarkerId = "markerId";

        /// <summary>
        /// A tracked human body
        /// </summary>
        public const string Body = "body";
    }

    /// <summary>
    /// A common list of trait definitions for consistency
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public static class TraitDefinitions
    {
        public static readonly TraitDefinition Pose = new TraitDefinition(TraitNames.Pose, typeof(Pose));
        public static readonly TraitDefinition Point = new TraitDefinition(TraitNames.Point, typeof(Vector3));
        public static readonly TraitDefinition Bounds2D = new TraitDefinition(TraitNames.Bounds2D, typeof(Vector2));
        public static readonly TraitDefinition Alignment = new TraitDefinition(TraitNames.Alignment, typeof(int));
        public static readonly TraitDefinition GeoCoordinate = new TraitDefinition(TraitNames.Geolocation, typeof(Vector2));
        public static readonly TraitDefinition HeightAboveFloor = new TraitDefinition(TraitNames.HeightAboveFloor, typeof(float));

        // Semantic tag traits
        public static readonly TraitDefinition Plane = new TraitDefinition(TraitNames.Plane, typeof(bool));
        public static readonly TraitDefinition Face = new TraitDefinition(TraitNames.Face, typeof(bool));
        public static readonly TraitDefinition Floor = new TraitDefinition(TraitNames.Floor, typeof(bool));
        public static readonly TraitDefinition Environment = new TraitDefinition(TraitNames.Environment, typeof(bool));
        public static readonly TraitDefinition User = new TraitDefinition(TraitNames.User, typeof(bool));
        public static readonly TraitDefinition InView = new TraitDefinition(TraitNames.InView, typeof(bool));
        public static readonly TraitDefinition DisplayFlat = new TraitDefinition(TraitNames.DisplayFlat, typeof(bool));
        public static readonly TraitDefinition DisplaySpatial = new TraitDefinition(TraitNames.DisplaySpatial, typeof(bool));
        public static readonly TraitDefinition Marker = new TraitDefinition(TraitNames.Marker, typeof(bool));
        public static readonly TraitDefinition MarkerId = new TraitDefinition(TraitNames.MarkerId, typeof(string));
        public static readonly TraitDefinition TrackingState = new TraitDefinition(TraitNames.TrackingState, typeof(int));

        /// <summary>
        /// A tracked human body
        /// </summary>
        public static readonly TraitDefinition Body = new TraitDefinition(TraitNames.Body, typeof(bool));
    }
}
