using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    /// <summary>
    /// Integrates velocity into position using a fixed deterministic timestep.
    /// </summary>
    public class SwarmIntegrator
    {
        float drag;

        public SwarmIntegrator(float drag = 0f)
        {
            this.drag = drag;
        }

        public void Step(SwarmState state)
        {
            float dt = SwarmTime.FixedDelta;

            var positions = state.positions;
            var velocities = state.velocities;
            var active = state.active;

            int capacity = state.Capacity;

            for (int i = 0; i < capacity; i++)
            {
                if (!active[i])
                    continue;

                Vector2 v = velocities[i];

                if (drag > 0f)
                    v *= (1f - drag * dt);

                positions[i] += v * dt;

                velocities[i] = v;
            }
        }
    }
}