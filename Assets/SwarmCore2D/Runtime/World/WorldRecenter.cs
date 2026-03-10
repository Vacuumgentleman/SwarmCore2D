using UnityEngine;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.World
{
    /// <summary>
    /// Keeps the world centered around origin
    /// to avoid floating point precision problems.
    /// </summary>
    public class WorldRecenter
    {
        const float RECENTER_DISTANCE = 500f;

        public bool ShouldRecenter(Vector2 playerPosition)
        {
            return playerPosition.magnitude > RECENTER_DISTANCE;
        }

        public void Apply(SwarmState state, Vector2 playerPosition)
        {
            if (state == null || state.positions == null)
                return;

            Vector2 offset = playerPosition;

            int count = state.positions.Length;

            for (int i = 0; i < count; i++)
            {
                state.positions[i] -= offset;
            }
        }
    }
}