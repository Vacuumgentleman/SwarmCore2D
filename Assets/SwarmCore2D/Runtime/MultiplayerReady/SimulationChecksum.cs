using UnityEngine;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.MultiplayerReady
{
    /// <summary>
    /// Generates deterministic checksum of simulation state.
    /// Used for lockstep validation and debugging.
    /// </summary>
    public static class SimulationChecksum
    {
        public static uint Compute(SwarmState state)
        {
            uint hash = 2166136261;

            int capacity = state.Capacity;

            var positions = state.positions;
            var velocities = state.velocities;
            var active = state.active;

            for (int i = 0; i < capacity; i++)
            {
                if (!active[i])
                    continue;

                HashFloat(ref hash, positions[i].x);
                HashFloat(ref hash, positions[i].y);

                HashFloat(ref hash, velocities[i].x);
                HashFloat(ref hash, velocities[i].y);
            }

            return hash;
        }

        static void HashFloat(ref uint hash, float value)
        {
            uint bits = (uint)System.BitConverter.SingleToInt32Bits(value);

            hash ^= bits;
            hash *= 16777619;
        }
    }
}