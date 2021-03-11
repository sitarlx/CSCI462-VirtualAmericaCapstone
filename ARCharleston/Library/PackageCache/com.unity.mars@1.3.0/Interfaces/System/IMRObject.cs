using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Query
{
    /// <summary>
    /// Interface for a representation of a single real-world thing
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public interface IMRObject
    {
        string name { get; set; }
    }
}
