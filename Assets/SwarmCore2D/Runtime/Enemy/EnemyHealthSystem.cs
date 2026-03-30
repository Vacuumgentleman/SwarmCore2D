using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class EnemyHealthSystem
    {
        public void Damage(SwarmState state, int id, float damage)
        {
            if (!state.active[id])
                return;

            state.health[id] -= damage;

            state.hitFlash[id] = 0.15f;

            if (state.health[id] <= 0f)
            {
                OnEnemyKilled(state, id);
                state.Deactivate(id);
            }
        }

        void OnEnemyKilled(SwarmState state, int id)
        {
            GiveXP(state, id);
            AddKill();
        }

        void GiveXP(SwarmState state, int id)
        {
            if (PlayerProgress.Instance == null)
                return;

            int type = state.enemyType[id];

            if (EnemyDatabase.Instance == null)
                return;

            EnemyData data = EnemyDatabase.Instance.Get(type);

            if (data == null)
                return;

            PlayerProgress.Instance.AddXP(data.xpReward);
        }

        void AddKill()
        {
            if (PlayerStats.Instance == null)
                return;

            PlayerStats.Instance.AddKill();
        }
    }
}