using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Simulation
{
    [MovedFrom("Unity.MARS.Simulation")]
    public abstract class SimulationEnvironmentModeSettings : ScriptableObject
    {
        public static readonly GUIContent[] NoEnvironmentSwitchingContents = {new GUIContent("No Environment Switching")};

        public abstract string EnvironmentModeName { get; }

        public abstract SimulationModeSelection DefaultSimulationMode { get; }

        public abstract bool IsFramingEnabled { get; }

        public abstract bool IsMovementEnabled { get; }

        public abstract bool HasEnvironmentSwitching { get; }

        public abstract GUIContent[] EnvironmentSwitchingContents { get; }

        public abstract int ActiveEnvironmentIndex { get; }

        public abstract void SetActiveEnvironment(int index);

        public abstract void FindEnvironmentCandidates();

        public abstract void OpenSimulationEnvironment();
    }
}
