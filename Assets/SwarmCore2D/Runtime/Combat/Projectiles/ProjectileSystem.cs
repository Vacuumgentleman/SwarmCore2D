using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Combat
{
    public class ProjectileSystem
    {
        ProjectileState projectiles;
        EnemyHealthSystem healthSystem = new EnemyHealthSystem();
        SpatialHashGrid grid;

        int maxProjectiles = 2000;

        public ProjectileState State => projectiles;

        public ProjectileSystem(SpatialHashGrid grid)
        {
            projectiles = new ProjectileState();
            this.grid = grid;
        }

        public void Spawn(
            Vector2  pos,
            Vector2  dir,
            float    speed,
            float    damage,
            float    duration,
            float    maxRange,
            bool     pierce,
            float    size,
            Material material   = null,
            int      frameCount = 1,
            float    frameRate  = 8f)
        {
            if (projectiles.count >= maxProjectiles)
                return;

            projectiles.Spawn(
                pos,
                dir.normalized,
                speed,
                damage,
                duration <= 0f ? Mathf.Infinity : duration,
                maxRange <= 0f ? Mathf.Infinity : maxRange,
                pierce,
                size,
                material,
                frameCount,
                frameRate
            );
        }

        public void Update(SwarmState state)
        {
            float dt = SwarmTime.FixedDelta;
            int i = 0;

            while (i < projectiles.count)
            {
                Vector2 pos = projectiles.position[i];
                pos += projectiles.direction[i] * projectiles.speed[i] * dt;
                projectiles.position[i] = pos;
                projectiles.lifetime[i] -= dt;

                float dist = Vector2.Distance(projectiles.startPos[i], pos);

                if (projectiles.lifetime[i] <= 0f || dist >= projectiles.maxDistance[i])
                {
                    projectiles.Remove(i);
                    continue;
                }

                if (CheckHit(state, i))
                    continue;

                i++;
            }
        }

        bool CheckHit(SwarmState state, int p)
        {
            Vector2 pos = projectiles.position[p];
            var neighbors = grid.Query(pos);

            foreach (int id in neighbors)
            {
                if (!state.active[id])
                    continue;

                float dist = Vector2.Distance(pos, state.positions[id]);
                float size = projectiles.size[p];

                if (dist <= state.radius[id] + size)
                {
                    healthSystem.Damage(state, id, projectiles.damage[p]);

                    if (!projectiles.pierce[p])
                    {
                        projectiles.Remove(p);
                        return true;
                    }
                }
            }

            return false;
        }

        public void MeleeHit(SwarmState state, Vector2 center, float radius, float damage)
        {
            var neighbors = grid.Query(center);

            foreach (int id in neighbors)
            {
                if (!state.active[id])
                    continue;

                float dist = Vector2.Distance(center, state.positions[id]);

                if (dist <= state.radius[id] + radius)
                    healthSystem.Damage(state, id, damage);
            }
        }
    }
}