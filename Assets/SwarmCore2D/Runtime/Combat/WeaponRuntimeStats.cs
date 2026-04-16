using UnityEngine;

[System.Serializable]
public class WeaponRuntimeStats
{
    public float damage;
    public float cooldown;
    public int amount;

    public float projectileSpeed;
    public float projectileSize;
    public float maxRange;
    public float spreadAngle;

    public float hitRadius;
    public float effectDuration;
    public int pierceCount;

    public float knockback;
    public float attackAngle;

    public bool attackUp;
    public bool attackDown;
    public bool attackLeft;
    public bool attackRight;
    public bool allowDiagonals;

    public WeaponStats.AttackType attackType;
    public WeaponStats.AttackDirectionMode directionMode;

    public Sprite[] frames;
    public float frameRate;
    public GameObject attackVisualPrefab;

    public void LoadFrom(WeaponStats baseStats)
    {
        damage = baseStats.damage;
        cooldown = baseStats.cooldown;
        amount = baseStats.amount;

        projectileSpeed = baseStats.projectileSpeed;
        projectileSize = baseStats.projectileSize;
        maxRange = baseStats.maxRange;
        spreadAngle = baseStats.spreadAngle;

        hitRadius = baseStats.hitRadius;
        effectDuration = baseStats.effectDuration;
        pierceCount = baseStats.pierceCount;

        knockback = baseStats.knockback;
        attackAngle = baseStats.attackAngle;

        attackUp = baseStats.attackUp;
        attackDown = baseStats.attackDown;
        attackLeft = baseStats.attackLeft;
        attackRight = baseStats.attackRight;
        allowDiagonals = baseStats.allowDiagonals;

        attackType = baseStats.attackType;
        directionMode = baseStats.directionMode;

        frames = baseStats.frames;
        frameRate = baseStats.frameRate;
        attackVisualPrefab = baseStats.attackVisualPrefab;
    }
}