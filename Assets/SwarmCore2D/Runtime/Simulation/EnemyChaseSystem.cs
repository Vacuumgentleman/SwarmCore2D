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
            int count = state.activeCount;

            for (int a = 0; a < count; a++)
            {
                int i = state.activeList[a];

                if (state.type[i] != 1)
                    continue;

                Vector2 pos = state.positions[i];

                Vector2 dir = playerPos - pos;

                dir = SwarmMath.SafeNormalize(dir);

                state.velocities[i] = dir * speed;
                state.positions[i] += state.velocities[i] * SwarmTime.FixedDelta;

                state.facingLeft[i] = dir.x < 0f;

                float t = Time.time + state.animOffset[i];
                state.frame[i] = (int)(t * animSpeed) % 7;
            }
        }
    }
}