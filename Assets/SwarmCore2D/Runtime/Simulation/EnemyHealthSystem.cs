using UnityEngine;

namespace SwarmCore2D.Simulation
{
    public class EnemyHealthSystem
    {
        public void Damage(SwarmState state, int id, float damage)
        {
            if (!state.active[id])
                return;

            state.health[id] -= damage;

            // activar flash rojo
            state.hitFlash[id] = 0.15f;

            if (state.health[id] <= 0f)
            {
                state.Deactivate(id);
            }
        }
    }
}