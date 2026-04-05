using UnityEngine;

namespace SwarmCore2D.Combat
{
    [System.Serializable]
    public class GlobalStats
    {
        public float might = 1f;       // daño
        public float area = 1f;        // tamaño
        public float speed = 1f;       // velocidad proyectiles
        public float duration = 1f;    // duración efectos
        public float cooldown = 1f;    // reducción cooldown
        public float amount = 0f;      // proyectiles extra
        public float luck = 0f;        // crit / efectos

        public float ModifyDamage(float baseDamage)
        {
            return baseDamage * might;
        }

        public float ModifySize(float baseSize)
        {
            return baseSize * area;
        }

        public float ModifySpeed(float baseSpeed)
        {
            return baseSpeed * speed;
        }

        public float ModifyDuration(float baseDuration)
        {
            return baseDuration * duration;
        }

        public float ModifyCooldown(float baseCooldown)
        {
            return baseCooldown / cooldown;
        }

        public int ModifyAmount(int baseAmount)
        {
            return baseAmount + Mathf.FloorToInt(amount);
        }
    }
}