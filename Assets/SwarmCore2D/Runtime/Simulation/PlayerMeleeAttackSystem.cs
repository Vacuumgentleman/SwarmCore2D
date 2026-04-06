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

        public void Attack(
            Vector2 origin,
            Vector2 dir,
            float hitRadius,
            float attackAngle,
            float damage,
            float knockback)
        {
            int count = state.activeCount;
            float radiusSq = hitRadius * hitRadius;
            float halfArc = attackAngle * 0.5f;

            for (int a = 0; a < count; a++)
            {
                int i = state.activeList[a];

                if (!state.active[i])
                    continue;

                Vector2 toEnemy = state.positions[i] - origin;
                float distSq = toEnemy.sqrMagnitude;

                if (distSq > radiusSq)
                    continue;

                if (attackAngle < 360f)
                {
                    float angle = Vector2.Angle(dir, toEnemy);
                    if (angle > halfArc)
                        continue;
                }

                healthSystem.Damage(state, i, damage);

                if (knockback > 0f && distSq > 0f)
                    state.positions[i] += toEnemy.normalized * knockback;
            }
        }
    }
}