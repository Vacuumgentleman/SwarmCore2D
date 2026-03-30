using UnityEngine;
using SwarmCore2D.Core;
using System.Collections.Generic;

namespace SwarmCore2D.Simulation
{
    public class EnemyContactDamageSystem
    {
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
            if (grid == null || player == null)
                return;

            List<int> nearby = grid.Query(playerPos);

            if (nearby == null || nearby.Count == 0)
                return;

            for (int n = 0; n < nearby.Count; n++)
            {
                int id = nearby[n];

                if (!state.active[id])
                    continue;

                Vector2 pos = state.positions[id];

                float radius = state.radius[id] + playerRadius;

                float dist =
                    Vector2.SqrMagnitude(playerPos - pos);

                if (dist < radius * radius)
                {
                    int type = state.enemyType[id];
                    var data = EnemyDatabase.Instance.Get(type);

                    if (data != null)
                        player.Damage(data.damage);

                    return;
                }
            }
        }
    }
}