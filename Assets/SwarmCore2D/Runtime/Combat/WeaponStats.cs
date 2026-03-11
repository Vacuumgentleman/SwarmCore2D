using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Weapon Stats")]

public class WeaponStats : ScriptableObject
{
    [Header("Attack")]
    public float damage = 4f;
    public float radius = 2.5f;
    public float cooldown = 0.7f;
    public GameObject attackVisualPrefab;
    [Header("Direction")]
    public bool alternateLeftRight = true;

    public bool attackUp = false;
    public bool attackDown = false;
    public bool attackLeft = true;
    public bool attackRight = true;

    [Header("Arc")]
    [Range(10,360)]
    public float attackAngle = 180f;

    [Header("Visual")]
    public Sprite[] frames;
    public float frameRate = 12f;

    [Header("Knockback")]
    public float knockback = 2f;
}