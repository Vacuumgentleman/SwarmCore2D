using UnityEngine;
using SwarmCore2D.Core;
using System.Collections.Generic;

namespace SwarmCore2D.Simulation
{
    public class EnemyContactDamageSystem
    {
        float damage = 5f;

        float playerRadius = 0.6f;

        SpatialHashGrid grid;

        public EnemyContactDamageSystem(SpatialHashGrid grid)
        {
            this.grid = grid;
        }

        public void Update(
            SwarmState state,
            Vector2 playerPos,
            PlayerHealth player)
        {
            if (grid == null)
                return;

            if (player == null)
                return;

            List<int> nearby = grid.Query(playerPos);

            if (nearby == null || nearby.Count == 0)
                return;

            int count = nearby.Count;

            for (int n = 0; n < count; n++)
            {
                int id = nearby[n];

                if (id < 0 || id >= state.Capacity)
                    continue;

                if (!state.active[id])
                    continue;

                if (state.type[id] != 1)
                    continue;

                Vector2 pos = state.positions[id];

                float radius = state.radius[id] + playerRadius;

                float dist =
                    Vector2.SqrMagnitude(playerPos - pos);

                if (dist < radius * radius)
                {
                    player.Damage(damage);
                    return;
                }
            }
        }
    }
}