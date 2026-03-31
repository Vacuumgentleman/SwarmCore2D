using UnityEngine;

[System.Serializable]
public class WeaponRuntimeStats
{
    public float damage;
    public float radius;
    public float cooldown;

    public int projectileCount;

    public bool attackUp;
    public bool attackDown;
    public bool attackLeft;
    public bool attackRight;

    public bool allowDiagonals;

    public float attackAngle;
    public float knockback;

    public float maxDistance;
    public float maxLifetime;
    public bool pierceEnemies;
    public float projectileSize;

    public WeaponStats.AttackType attackType;
    public WeaponStats.AttackDirectionMode directionMode;

    public Sprite[] frames;
    public float frameRate;

    public GameObject attackVisualPrefab;

    public void LoadFrom(WeaponStats baseStats)
    {
        damage = baseStats.damage;
        radius = baseStats.radius;
        cooldown = baseStats.cooldown;

        projectileCount = baseStats.projectileCount;

        attackUp = baseStats.attackUp;
        attackDown = baseStats.attackDown;
        attackLeft = baseStats.attackLeft;
        attackRight = baseStats.attackRight;

        allowDiagonals = baseStats.allowDiagonals;

        attackAngle = baseStats.attackAngle;
        knockback = baseStats.knockback;

        maxDistance = baseStats.maxDistance;
        maxLifetime = baseStats.maxLifetime;
        pierceEnemies = baseStats.pierceEnemies;
        projectileSize = baseStats.projectileSize;

        attackType = baseStats.attackType;
        directionMode = baseStats.directionMode;

        frames = baseStats.frames;
        frameRate = baseStats.frameRate;

        attackVisualPrefab = baseStats.attackVisualPrefab;
    }
}