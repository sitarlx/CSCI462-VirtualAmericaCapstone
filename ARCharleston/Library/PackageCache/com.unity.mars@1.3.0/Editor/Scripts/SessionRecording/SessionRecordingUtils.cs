using Unity.MARS.Data.Recorded;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Timeline;

namespace UnityEditor.MARS.Data.Recorded
{
    [MovedFrom("Unity.MARS.Recording")]
    public static class SessionRecordingUtils
    {
        public static SessionRecordingInfo CreateSessionRecordingAsset(string path)
        {
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();
            AssetDatabase.CreateAsset(timeline, path);
            var recordingInfo = SessionRecordingInfo.Create(timeline);
            AssetDatabase.AddObjectToAsset(recordingInfo, timeline);
            return recordingInfo;
        }
    }
}
