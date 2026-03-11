using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class PlayerMeleeAttackSystem
    {
        SwarmState state;

        EnemyHealthSystem healthSystem;

        public PlayerMeleeAttackSystem(SwarmState state)
        {
            this.state = state;
            healthSystem = new EnemyHealthSystem();
        }

        public void Attack(Vector2 origin, WeaponStats weapon, Vector2 dir)
        {
            int count = state.activeCount;

            float radiusSq = weapon.radius * weapon.radius;
            float halfArc = weapon.attackAngle * 0.5f;

            for (int a = 0; a < count; a++)
            {
                int i = state.activeList[a];

                if (state.type[i] != 1)
                    continue;

                Vector2 enemyPos = state.positions[i];

                Vector2 toEnemy = enemyPos - origin;

                float distSq = toEnemy.sqrMagnitude;

                if (distSq > radiusSq)
                    continue;

                float angle = Vector2.Angle(dir, toEnemy);

                if (angle > halfArc)
                    continue;

                healthSystem.Damage(state, i, weapon.damage);

                Vector2 push = toEnemy.normalized * weapon.knockback;

                state.positions[i] += push;
            }
        }
    }
}