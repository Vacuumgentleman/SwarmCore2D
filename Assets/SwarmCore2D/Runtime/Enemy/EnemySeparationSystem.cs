using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class EnemySeparationSystem
    {
        float separationRadius = 0.6f;
        float separationForce = 2f;

        SpatialHashGrid grid;

        public EnemySeparationSystem()
        {
            grid = new SpatialHashGrid(1.2f);
        }

        public void Update(SwarmState state)
        {
            grid.Clear();

            int count = state.activeCount;

            for (int a = 0; a < count; a++)
            {
                int i = state.activeList[a];

                grid.Add(i, state.positions[i]);
            }

            float radiusSq = separationRadius * separationRadius;

            for (int a = 0; a < count; a++)
            {
                int i = state.activeList[a];

                Vector2 posA = state.positions[i];

                Vector2 push = Vector2.zero;

                var neighbors = grid.Query(posA);

                for (int n = 0; n < neighbors.Count; n++)
                {
                    int j = neighbors[n];

                    if (i == j)
                        continue;

                    Vector2 posB = state.positions[j];

                    Vector2 diff = posA - posB;

                    float distSq = diff.sqrMagnitude;

                    if (distSq > radiusSq || distSq == 0)
                        continue;

                    float dist = Mathf.Sqrt(distSq);

                    push += diff / dist;
                }

                state.positions[i] += push * separationForce * SwarmTime.FixedDelta;
            }
        }
    }
}