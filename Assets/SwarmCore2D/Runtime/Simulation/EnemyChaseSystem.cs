using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class EnemyChaseSystem
    {
        float speed = 3f;
        float animSpeed = 8f;

        public void Update(SwarmState state, Vector2 playerPos)
        {
            int cap = state.Capacity;

            for (int i = 0; i < cap; i++)
            {
                if (!state.active[i])
                    continue;

                if (state.type[i] != 1)
                    continue;

                Vector2 pos = state.positions[i];

                Vector2 dir = playerPos - pos;

                dir = SwarmMath.SafeNormalize(dir);

                state.velocities[i] = dir * speed;
                state.positions[i] += state.velocities[i] * SwarmTime.FixedDelta;

                // flip
                state.facingLeft[i] = dir.x < 0f;

                // animación con desfase
                float t = Time.time + state.animOffset[i];
                state.frame[i] = (int)(t * animSpeed) % 7;
            }
        }
    }
}