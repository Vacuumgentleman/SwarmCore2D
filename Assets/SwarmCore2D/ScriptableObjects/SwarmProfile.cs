using UnityEngine;

namespace SwarmCore2D.ScriptableObjects
{
    /// <summary>
    /// Core simulation configuration for SwarmCore2D.
    /// Controls swarm behaviour, physics limits and simulation parameters.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SwarmProfile",
        menuName = "SwarmCore2D/Swarm Profile"
    )]
    public class SwarmProfile : ScriptableObject
    {
        [Header("Simulation")]

        [Tooltip("Simulation tick rate.")]
        public int tickRate = 60;

        [Tooltip("Maximum number of swarm entities.")]
        public int maxEntities = 1024;

        [Header("Movement")]

        [Tooltip("Maximum allowed velocity.")]
        public float maxVelocity = 20f;

        [Tooltip("Drag applied each tick.")]
        public float drag = 0.0f;

        [Header("Collision")]

        [Tooltip("Base radius for swarm entities.")]
        public float defaultRadius = 0.4f;

        [Tooltip("Solver iterations per tick.")]
        public int solverIterations = 2;

        [Header("Spatial Grid")]

        [Tooltip("Spatial grid cell size.")]
        public float cellSize = 1.5f;

        [Tooltip("Maximum entities per cell.")]
        public int maxCellCapacity = 64;
    }
}