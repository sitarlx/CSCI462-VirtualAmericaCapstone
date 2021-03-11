using System;
using Unity.MARS;
using Unity.MARS.Attributes;
using Unity.MARS.Landmarks;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Landmarks
{
    [ComponentEditor(typeof(ICalculateLandmarks), true)]
    [MovedFrom("Unity.MARS")]
    public class CalculateLandmarksInspector : ComponentInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LandmarkControllerEditor.DrawAddLandmarkButton((ICalculateLandmarks)target);
        }

        public override bool HasDisplayProperties()
        {
            return true;
        }
    }
}
