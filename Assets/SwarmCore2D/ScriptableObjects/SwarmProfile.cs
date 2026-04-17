using UnityEngine;

namespace SwarmCore2D.ScriptableObjects
{
    /// <summary>
    /// Core simulation configuration for SwarmCore2D.
    /// Single source of truth for tick rate and entity capacity.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SwarmProfile",
        menuName = "SwarmCore2D/Configuration/Swarm Profile"
    )]
    public class SwarmProfile : ScriptableObject
    {
        [Header("Simulation")]

        [Tooltip("Ticks por segundo. Afecta FixedTickBehaviour y la simulación de entidades.")]
        public int tickRate = 60;

        [Tooltip("Máximo de entidades simultáneas. Aumentar consume más memoria.")]
        public int maxEntities = 2048;
    }
}
