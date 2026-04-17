using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.ScriptableObjects;

namespace SwarmCore2D.Simulation
{
    public class EnemySpawnerSystem
    {
        float spawnTimer;
        SwarmDifficultyProfile profile;

        public EnemySpawnerSystem(SwarmDifficultyProfile profile = null)
        {
            this.profile = profile;
        }

        public void Update(SwarmWorld world, Vector2 playerPos)
        {
            spawnTimer += SwarmTime.FixedDelta;

            SpawnPhase phase = profile != null ? profile.GetActivePhase(SwarmTime.Time) : null;
            float interval = (phase != null && phase.spawnInterval > 0f) ? phase.spawnInterval : 2f;

            if (spawnTimer < interval)
                return;

            spawnTimer = 0f;

            EnemyData data = null;
            int enemyIndex = 0;

            if (phase != null && phase.enemyPool != null && phase.enemyPool.Length > 0)
            {
                DeterministicRNG rng = new DeterministicRNG((uint)SwarmTime.Tick);
                int idx = rng.Range(0, phase.enemyPool.Length);
                data = phase.enemyPool[idx];
                if (data != null && EnemyDatabase.Instance != null)
                    enemyIndex = EnemyDatabase.Instance.IndexOf(data);
            }
            else if (EnemyDatabase.Instance != null
                     && EnemyDatabase.Instance.enemies != null
                     && EnemyDatabase.Instance.enemies.Length > 0)
            {
                DeterministicRNG rng = new DeterministicRNG((uint)SwarmTime.Tick);
                enemyIndex = rng.Range(0, EnemyDatabase.Instance.enemies.Length);
                data = EnemyDatabase.Instance.Get(enemyIndex);
            }

            if (data == null)
                return;

            DeterministicRNG posRng = new DeterministicRNG((uint)(SwarmTime.Tick + 1));
            Vector2 offset = posRng.Direction() * 10f;
            Vector2 spawnPos = playerPos + offset;

            SwarmEntity entity = world.Spawn(spawnPos);

            int id = entity.id;
            if (id < 0)
                return;

            var state = world.state;
            float elapsed = SwarmTime.Time;

            state.enemyType[id] = enemyIndex;
            state.health[id] = data.maxHealth * (profile != null ? profile.GetHealthMultiplier(elapsed) : 1f);
            state.radius[id] = data.radius;
            state.mass[id] = 1f;
            state.speedMultiplier[id] = 1f;

            state.velocities[id] = Vector2.zero;
            state.hitFlash[id] = 0f;
            state.frame[id] = 0;
            state.facingLeft[id] = false;
        }
    }
}
