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

            DeterministicRNG rng = new DeterministicRNG((uint)SwarmTime.Tick);

            Vector2 offset = rng.Direction() * 10f;

            Vector2 spawnPos = playerPos + offset;

            world.Spawn(spawnPos, 1);
        }
    }
}