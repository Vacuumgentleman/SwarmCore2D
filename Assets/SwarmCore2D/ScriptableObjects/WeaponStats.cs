using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "SwarmCore2D/Combat/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public enum AttackType
    {
        Melee,
        Projectile,
        Area,
        Orbit
    }

    public enum AttackDirectionMode
    {
        Clockwise,
        Alternating
    }

    [Header("Type")]
    public AttackType attackType = AttackType.Projectile;

    [Header("Base Stats")]
    public float damage = 4f;
    public float cooldown = 0.7f;
    public int amount = 1;

    [Header("Projectile")]
    public float projectileSpeed = 12f;
    public float projectileSize = 0.5f;
    public float maxRange = 0f;
    [Tooltip("Ángulo entre proyectiles extra del mismo disparo (grados). 0 = sin separación.")]
    public float spreadAngle = 0f;

    [Header("Area / Melee")]
    public float hitRadius = 2.5f;

    [Header("Duration")]
    public float effectDuration = 2f;

    [Header("Pierce")]
    public int pierceCount = 0;

    [Header("Knockback")]
    public float knockback = 2f;

    [Header("Direction")]
    public bool attackUp = false;
    public bool attackDown = false;
    public bool attackLeft = true;
    public bool attackRight = true;
    public bool allowDiagonals = true;
    public AttackDirectionMode directionMode = AttackDirectionMode.Clockwise;

    [Header("Arc")]
    [Range(10, 360)]
    public float attackAngle = 180f;

    [Header("Scaling Flags")]
    public bool scaledByMight = true;
    public bool scaledByArea = true;
    public bool scaledBySpeed = true;
    public bool scaledByDuration = true;
    public bool scaledByAmount = true;

    [Header("Upgrades")]
    [Tooltip("Upgrade types offered when this weapon is selected for an upgrade. Leave empty to allow all weapon-specific upgrades.")]
    public List<UpgradeData.UpgradeType> allowedWeaponUpgrades = new List<UpgradeData.UpgradeType>();

    [Header("Visual")]
    [Tooltip("Icono del arma para la UI de upgrades")]
    public Sprite weaponIcon;
    public GameObject attackVisualPrefab;
    public Sprite[] frames;
    public float frameRate = 12f;
}