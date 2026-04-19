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
        Alternating,
        Volley
    }

    [Header("Type")]
    [Tooltip("Tipo de ataque: Melee (arco cuerpo a cuerpo), Projectile (balas), Area (AoE alrededor del jugador), Orbit (objetos en órbita)")]
    public AttackType attackType = AttackType.Projectile;

    [Header("Base Stats")]
    [Tooltip("Daño base por impacto")]
    public float damage = 4f;
    [Tooltip("Segundos entre ataques")]
    public float cooldown = 0.7f;
    [Tooltip("Número de proyectiles/golpes por ataque")]
    public int amount = 1;

    [Header("Projectile")]
    [Tooltip("Velocidad de movimiento del proyectil (unidades/seg)")]
    public float projectileSpeed = 12f;
    [Tooltip("Radio de colisión de cada proyectil")]
    public float projectileSize = 0.5f;
    [Tooltip("Distancia máxima antes de desaparecer (0 = ilimitado)")]
    public float maxRange = 0f;
    [Tooltip("Ángulo entre proyectiles extra del mismo disparo (grados). 0 = sin separación.")]
    public float spreadAngle = 0f;

    [Header("Area / Melee")]
    [Tooltip("Radio del arco de melee o zona de área")]
    public float hitRadius = 2.5f;

    [Header("Duration")]
    [Tooltip("Duración del proyectil o de efectos temporales")]
    public float effectDuration = 2f;

    [Header("Pierce")]
    [Tooltip("Enemigos que puede atravesar antes de desaparecer (0 = destruye en primer impacto)")]
    public int pierceCount = 0;

    [Header("Knockback")]
    [Tooltip("Fuerza aplicada al enemigo al impactar")]
    public float knockback = 2f;

    [Header("Direction")]
    [Tooltip("Habilita ataques hacia arriba")]
    public bool attackUp = false;
    [Tooltip("Habilita ataques hacia abajo")]
    public bool attackDown = false;
    [Tooltip("Habilita ataques hacia la izquierda")]
    public bool attackLeft = true;
    [Tooltip("Habilita ataques hacia la derecha")]
    public bool attackRight = true;
    [Tooltip("Permite direcciones diagonales (NE, SE, SW, NO)")]
    public bool allowDiagonals = true;
    [Tooltip("Clockwise: siempre empieza por la dirección 0, distribuye round-robin. Alternating: desplaza el inicio cada ráfaga para cobertura uniforme. Volley: todos los proyectiles van a la misma dirección, rotando una dirección cada ataque.")]
    public AttackDirectionMode directionMode = AttackDirectionMode.Clockwise;

    [Header("Arc")]
    [Tooltip("Amplitud del arco en grados para ataques melee")]
    [Range(10, 360)]
    public float attackAngle = 180f;

    [Header("Scaling Flags")]
    [Tooltip("El daño escala con el stat Might del jugador")]
    public bool scaledByMight = true;
    [Tooltip("El radio escala con el stat Area del jugador")]
    public bool scaledByArea = true;
    [Tooltip("El cooldown escala con el stat Speed del jugador")]
    public bool scaledBySpeed = true;
    [Tooltip("La duración escala con el stat Duration del jugador")]
    public bool scaledByDuration = true;
    [Tooltip("La cantidad de proyectiles escala con el stat Amount del jugador")]
    public bool scaledByAmount = true;

    [Header("Upgrades")]
    [Tooltip("Upgrade types offered when this weapon is selected for an upgrade. Leave empty to allow all weapon-specific upgrades.")]
    public List<UpgradeData.UpgradeType> allowedWeaponUpgrades = new List<UpgradeData.UpgradeType>();

    [Header("Visual")]
    [Tooltip("Icono del arma para la UI de upgrades")]
    public Sprite weaponIcon;

    [Header("Attack Visual")]
    [Tooltip("Material con SwarmSpriteInstanced shader. Enable GPU Instancing debe estar ON.")]
    public Material attackVisualMaterial;
    [Tooltip("Número de fotogramas en el sprite sheet de animación")]
    public int attackVisualFrameCount = 1;
    [Tooltip("Velocidad de reproducción de la animación en fotogramas por segundo")]
    public float attackVisualFrameRate = 12f;
    [Tooltip("Escala visual del sprite de ataque (0 usa 1 como fallback para assets existentes)")]
    public float attackVisualScale = 1f;
    [Tooltip("Si está activo, el sprite del proyectil rota para alinearse con su dirección de movimiento")]
    public bool rotateProjectile = true;
}