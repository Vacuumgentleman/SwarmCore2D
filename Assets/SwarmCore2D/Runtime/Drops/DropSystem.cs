using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Drops
{
    public class DropSystem : MonoBehaviour
    {
        public static DropSystem Instance;

        DropPool pool = new DropPool();
        public DropPool Pool => pool;

        void Awake()
        {
            Instance = this;
        }

        public void SpawnDrops(Vector2 position, EnemyData enemyData, uint rngSeed)
        {
            if (enemyData?.dropTable == null) return;
            SpawnFromTable(position, enemyData.dropTable, rngSeed);
        }

        public void SpawnFromTable(Vector2 position, EnemyDropEntry[] table, uint rngSeed)
        {
            if (table == null) return;

            var rng = new DeterministicRNG(rngSeed);

            foreach (var entry in table)
            {
                if (entry.drop == null) continue;

                int count = rng.Range(entry.minCount, entry.maxCount + 1);
                for (int i = 0; i < count; i++)
                {
                    if (rng.NextFloat() <= entry.chance)
                        pool.Spawn(position + rng.Direction() * 0.4f, entry.drop);
                }
            }
        }

        public void UpdateDrops(Vector2 playerPos, PlayerHealth health)
        {
            float dt = Time.deltaTime;

            for (int a = pool.activeCount - 1; a >= 0; a--)
            {
                int id = pool.activeList[a];

                pool.animTimer[id] += dt;
                int fc = Mathf.Max(1, pool.data[id].frameCount);
                pool.frame[id] = (int)(pool.animTimer[id] * pool.data[id].frameRate) % fc;

                float dist = Vector2.Distance(pool.positions[id], playerPos);
                if (dist <= pool.data[id].collectRadius)
                    Collect(id, health);
            }
        }

        void Collect(int id, PlayerHealth health)
        {
            var d = pool.data[id];
            pool.Despawn(id);

            switch (d.type)
            {
                case DropType.Coin:
                    PlayerCurrencySystem.Instance?.AddCoins((int)d.value);
                    break;
                case DropType.Heal:
                    health?.Heal(d.value);
                    break;
                case DropType.DamageBoost:
                    PlayerStats.Instance?.AddBuff(DropType.DamageBoost, d.value, d.duration);
                    break;
                case DropType.SpeedBoost:
                    PlayerStats.Instance?.AddBuff(DropType.SpeedBoost, d.value, d.duration);
                    break;
                case DropType.WeaponUnlock:
                    if (d.weaponToUnlock != null)
                        PlayerWeaponController.Instance?.AddWeapon(d.weaponToUnlock);
                    break;
            }
        }
    }
}
