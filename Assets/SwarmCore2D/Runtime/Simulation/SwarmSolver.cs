using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    /// <summary>
    /// Resolves swarm interactions such as separation and pressure.
    /// Prevents entity overlap and stabilizes the swarm.
    /// </summary>
    public class SwarmSolver
    {
        int iterations;

        public SwarmSolver(int iterations = SwarmConstants.SolverIterations)
        {
            this.iterations = iterations;
        }

        public void Solve(SwarmState state)
        {
            var positions = state.positions;
            var radius = state.radius;
            var active = state.active;

            int capacity = state.Capacity;

            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = 0; i < capacity; i++)
                {
                    if (!active[i])
                        continue;

                    for (int j = i + 1; j < capacity; j++)
                    {
                        if (!active[j])
                            continue;

                        Vector2 delta = positions[j] - positions[i];

                        float distSq = delta.x * delta.x + delta.y * delta.y;

                        float minDist = radius[i] + radius[j];

                        if (distSq <= 0f || distSq >= minDist * minDist)
                            continue;

                        float dist = Mathf.Sqrt(distSq);

                        Vector2 normal = delta / dist;

                        float penetration = minDist - dist;

                        Vector2 correction = normal * (penetration * 0.5f);

                        positions[i] -= correction;
                        positions[j] += correction;
                    }
                }
            }
        }
    }
}