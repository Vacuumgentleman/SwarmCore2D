using UnityEngine;

namespace SwarmCore2D.Combat
{
    public struct DamageData
    {
        public float amount;

        public float critChance;
        public float critMultiplier;

        public float knockback;

        public int sourceWeaponId;

        public DamageData(
            float amount,
            float critChance = 0f,
            float critMultiplier = 1.5f,
            float knockback = 0f,
            int sourceWeaponId = -1)
        {
            this.amount = amount;
            this.critChance = critChance;
            this.critMultiplier = critMultiplier;
            this.knockback = knockback;
            this.sourceWeaponId = sourceWeaponId;
        }

        public float GetFinalDamage()
        {
            float final = amount;

            if (critChance > 0f && Random.value < critChance)
            {
                final *= critMultiplier;
            }

            return final;
        }
    }
}