using Unity.MARS.Simulation;
using UnityEngine;

namespace Unity.MARS
{
    [DisallowMultipleComponent]
    [SelectionBase]
    public abstract class MARSEntity : MonoBehaviour, ISimulatable { }
}
