using UnityEngine;

namespace SwarmCore2D.Simulation
{
    public class EnemyContactDamageSystem
    {
        float damage = 5f;

        public void Update(SwarmState state, Vector2 playerPos, PlayerHealth player)
        {
            int count = state.activeCount;

            for (int a = 0; a < count; a++)
            {
                int i = state.activeList[a];

                if (state.type[i] != 1)
                    continue;

                Vector2 pos = state.positions[i];

                float radius = state.radius[i] + 0.6f;

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