using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class EnemySpawnerSystem
    {
        float spawnTimer;

        public void Update(SwarmWorld world, Vector2 playerPos)
        {
            spawnTimer += SwarmTime.FixedDelta;

            if (spawnTimer < 2f)
                return;

            spawnTimer = 0f;

            if (EnemyDatabase.Instance == null || EnemyDatabase.Instance.enemies == null || EnemyDatabase.Instance.enemies.Length == 0)
                return;

            DeterministicRNG rng = new DeterministicRNG((uint)SwarmTime.Tick);

            Vector2 offset = rng.Direction() * 10f;
            Vector2 spawnPos = playerPos + offset;

            int enemyIndex = rng.Range(0, EnemyDatabase.Instance.enemies.Length);

            EnemyData data = EnemyDatabase.Instance.Get(enemyIndex);

            SwarmEntity entity = world.Spawn(spawnPos);

            int id = entity.id;

            if (id < 0 || data == null)
                return;

            var state = world.state;

            state.enemyType[id] = enemyIndex;

            state.health[id] = data.maxHealth;
            state.radius[id] = data.radius;
            state.mass[id] = 1f;

            state.velocities[id] = Vector2.zero;
            state.hitFlash[id] = 0f;

            state.frame[id] = 0;
            state.facingLeft[id] = false;
        }
    }
}