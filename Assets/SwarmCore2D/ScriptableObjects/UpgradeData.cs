using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;

    public enum UpgradeType
    {
        // Weapon specific
        Damage,
        AttackSpeed,
        ProjectileAmount,
        Size,
        Knockback,
        AddDirectionRandom,
        Pierce,

        // Global
        GlobalDamage,
        GlobalAttackSpeed,
        GlobalProjectile,
        GlobalArea,
        GlobalSpeed,
        GlobalDuration,
        GlobalPierce,
        GlobalCritChance,
        GlobalLifeSteal,

        // Player
        Heal,
        MaxHealth
    }

    public UpgradeType type;

    [Tooltip("Indice del arma a la que aplica este upgrade. -1 = global")]
    public int weaponIndex = -1;

    public float value = 1f;
}