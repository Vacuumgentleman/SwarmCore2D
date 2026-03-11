using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class EnemySeparationSystem
    {
        float separationRadius = 0.6f;
        float separationForce = 2f;

        public void Update(SwarmState state)
        {
            int cap = state.Capacity;

            float radiusSq = separationRadius * separationRadius;

            for (int i = 0; i < cap; i++)
            {
                if (!state.active[i])
                    continue;

                if (state.type[i] != 1)
                    continue;

                Vector2 posA = state.positions[i];

                Vector2 push = Vector2.zero;

                for (int j = 0; j < cap; j++)
                {
                    if (i == j)
                        continue;

                    if (!state.active[j])
                        continue;

                    Vector2 posB = state.positions[j];

                    Vector2 diff = posA - posB;

                    float distSq = diff.sqrMagnitude;

                    if (distSq > radiusSq || distSq == 0f)
                        continue;

                    float dist = Mathf.Sqrt(distSq);

                    push += diff / dist;
                }

                state.positions[i] += push * separationForce * SwarmTime.FixedDelta;
            }
        }
    }
}