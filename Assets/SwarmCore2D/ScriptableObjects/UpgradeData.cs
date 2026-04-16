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

    [Tooltip("Si es true, aplica a todos los stats globales del jugador. Si es false, aplica al arma en weaponIndex.")]
    public bool isGlobal = false;

    [Tooltip("Indice del arma a la que aplica este upgrade. Solo relevante si isGlobal es false.")]
    public int weaponIndex = 0;

    public float value = 1f;
}