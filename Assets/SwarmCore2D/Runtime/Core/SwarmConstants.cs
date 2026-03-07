using UnityEngine;

namespace SwarmCore2D.Core
{
    /// <summary>
    /// Global constants for SwarmCore 2D.
    /// Centralized configuration to avoid magic numbers.
    /// </summary>
    public static class SwarmConstants
    {
        // ==============================
        // Simulation Limits
        // ==============================

        public const int MaxEntities = 2048;
        public const int DefaultInitialCapacity = 512;

        // ==============================
        // Simulation Timing
        // ==============================

        public const int FixedTickRate = 60;
        public const float FixedDeltaTime = 1f / FixedTickRate;

        // Prevent spiral of death
        public const int MaxTicksPerFrame = 4;

        // ==============================
        // Solver
        // ==============================

        public const int SolverIterations = 2;
        public const float PositionEpsilon = 0.0001f;

        // ==============================
        // Spatial Grid
        // ==============================

        public const int MaxGridCells = 4096;
        public const float DefaultCellSize = 1.5f;

        // ==============================
        // Safety
        // ==============================

        public const float MaxVelocity = 50f;
        public const float MaxForce = 100f;
    }
}