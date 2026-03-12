using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public enum AttackType
    {
        Melee,
        Ranged
    }

    [Header("Attack Type")]
    public AttackType attackType = AttackType.Melee;

    [Header("Attack")]
    public float damage = 4f;
    public float radius = 2.5f;
    public float cooldown = 0.7f;
    public GameObject attackVisualPrefab;

    [Header("Direction")]
    public bool attackUp = false;
    public bool attackDown = false;
    public bool attackLeft = true;
    public bool attackRight = true;

    [Header("Advanced Direction")]
    public bool allowDiagonals = true;
    [Tooltip("Número de proyectiles disparados por ataque")]
    public int projectileCount = 1;

    public enum AttackDirectionMode
    {
        Clockwise,
        Alternating
    }
    public AttackDirectionMode directionMode = AttackDirectionMode.Clockwise;

    [Header("Arc")]
    [Range(10, 360)]
    public float attackAngle = 180f;

    [Header("Visual")]
    public Sprite[] frames;
    public float frameRate = 12f;

    [Header("Knockback")]
    public float knockback = 2f;

    [Header("Ranged Settings (Solo si attackType == Ranged)")]
    [Tooltip("Distancia máxima del proyectil. 0 = infinita")]
    public float maxDistance = 0f;
    [Tooltip("Tiempo máximo de vida del proyectil")]
    public float maxLifetime = 5f;
    [Tooltip("Si el proyectil atraviesa enemigos")]
    public bool pierceEnemies = false;
    [Tooltip("Tamaño del proyectil")]
    public float projectileSize = 0.5f;
}