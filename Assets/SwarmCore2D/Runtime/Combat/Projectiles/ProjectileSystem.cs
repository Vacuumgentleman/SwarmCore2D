using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Simulation;
using SwarmCore2D.Spatial;

namespace SwarmCore2D.Combat
{
    public class ProjectileSystem
    {
        ProjectileState projectiles;

        EnemyHealthSystem healthSystem = new EnemyHealthSystem();

        SpatialHashGrid grid;

        // ✅ POOL LIMIT (ANTI EXPLOSIÓN)
        int maxProjectiles = 2000;

        public ProjectileState State => projectiles;

        public ProjectileSystem(SpatialHashGrid grid)
        {
            projectiles = new ProjectileState();
            this.grid = grid;
        }

        public void Spawn(
            Vector2 pos,
            Vector2 dir,
            WeaponStats weapon)
        {
            // ✅ LIMITADOR
            if (projectiles.count >= maxProjectiles)
                return;

            projectiles.Spawn(
                pos,
                dir.normalized,
                weapon.speed,
                weapon.damage,
                weapon.duration,
                weapon.maxDistance <= 0 ? Mathf.Infinity : weapon.maxDistance,
                weapon.pierceEnemies,
                weapon.projectileSize
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

                float dist = Vector2.Distance(
                    projectiles.startPos[i],
                    pos
                );

                if (projectiles.lifetime[i] <= 0f ||
                    dist >= projectiles.maxDistance[i])
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

                float dist = Vector2.Distance(
                    pos,
                    state.positions[id]
                );

                float size = projectiles.size[p];

                if (dist <= state.radius[id] + size)
                {
                    // ✅ CORREGIDO
                    healthSystem.Damage(
                        state,
                        id,
                        projectiles.damage[p]
                    );

                    if (!projectiles.pierce[p])
                    {
                        projectiles.Remove(p);
                        return true;
                    }
                }
            }

            return false;
        }

        // ✅ MELEE REAL (NO PROYECTIL)
        public void MeleeHit(
            SwarmState state,
            Vector2 center,
            float radius,
            float damage
        )
        {
            var neighbors = grid.Query(center);

            foreach (int id in neighbors)
            {
                if (!state.active[id])
                    continue;

                float dist = Vector2.Distance(center, state.positions[id]);

                if (dist <= state.radius[id] + radius)
                {
                    healthSystem.Damage(state, id, damage);
                }
            }
        }
    }
}