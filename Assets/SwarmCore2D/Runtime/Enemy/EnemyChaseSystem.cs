using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.ScriptableObjects;

namespace SwarmCore2D.Simulation
{
    public class EnemyChaseSystem
    {
        SwarmDifficultyProfile profile;

        public EnemyChaseSystem(SwarmDifficultyProfile profile = null)
        {
            this.profile = profile;
        }

        public void Update(SwarmState state, Vector2 playerPos)
        {
            float speedMult = profile != null
                ? profile.GetSpeedMultiplier(SwarmTime.Time)
                : 1f;

            int count = state.activeCount;

            for (int a = 0; a < count; a++)
            {
                int i = state.activeList[a];

                int type = state.enemyType[i];

                var data = EnemyDatabase.Instance.Get(type);

                if (data == null)
                    continue;

                if (data.behavior != EnemyBehaviorType.Chase)
                    continue;

                Vector2 pos = state.positions[i];

                Vector2 dir = playerPos - pos;
                dir = SwarmMath.SafeNormalize(dir);

                state.velocities[i] = dir * data.speed * state.speedMultiplier[i] * speedMult;
                state.positions[i] += state.velocities[i] * SwarmTime.FixedDelta;

                state.facingLeft[i] = dir.x < 0f;

                float t = Time.time + state.animOffset[i];
                state.frame[i] = (int)(t * data.animSpeed) % Mathf.Max(1, data.frameCount);
            }
        }
    }
}
