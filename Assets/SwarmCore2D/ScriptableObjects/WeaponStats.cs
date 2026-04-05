using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public enum AttackType
    {
        Melee,
        Ranged
    }

    [Header("Type")]
    public AttackType attackType = AttackType.Melee;

    // =========================
    // BASE STATS (VS STYLE)
    // =========================

    [Header("Base Stats")]
    public float damage = 4f;
    public float cooldown = 0.7f;

    [Tooltip("Cantidad de proyectiles por disparo")]
    public int amount = 1;

    [Tooltip("Duración del ataque (vida del proyectil o efecto)")]
    public float duration = 2f;

    [Tooltip("Velocidad del proyectil")]
    public float speed = 12f;

    [Tooltip("Área / tamaño del hitbox")]
    public float area = 1f;

    [Tooltip("Cuántos enemigos puede atravesar")]
    public int pierce = 1;

    [Tooltip("Tiempo entre impactos al mismo enemigo")]
    public float hitCooldown = 0.2f;

    // =========================
    // MELEE
    // =========================

    [Header("Melee")]
    public float radius = 2.5f;

    // =========================
    // RANGED
    // =========================

    [Header("Ranged")]
    [Tooltip("Distancia máxima (0 = infinito)")]
    public float maxDistance = 0f;

    [Tooltip("Tiempo máximo de vida")]
    public float maxLifetime = 5f;

    [Tooltip("Tamaño del proyectil")]
    public float projectileSize = 0.5f;

    [Tooltip("Si atraviesa enemigos infinitamente")]
    public bool pierceEnemies = false;

    // =========================
    // DIRECCIÓN
    // =========================

    [Header("Direction")]
    public bool attackUp = false;
    public bool attackDown = false;
    public bool attackLeft = true;
    public bool attackRight = true;

    public bool allowDiagonals = true;

    public enum AttackDirectionMode
    {
        Clockwise,
        Alternating
    }

    public AttackDirectionMode directionMode = AttackDirectionMode.Clockwise;

    [Header("Arc")]
    [Range(10, 360)]
    public float attackAngle = 180f;

    // =========================
    // EFECTOS
    // =========================

    [Header("Effects")]
    public float knockback = 2f;

    // =========================
    // VISUAL
    // =========================

    [Header("Visual")]
    public GameObject attackVisualPrefab;
    public Sprite[] frames;
    public float frameRate = 12f;
}