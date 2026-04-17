using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "SwarmCore2D/World/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public float maxHealth = 10f;
    public float speed = 3f;
    public float damage = 5f;
    public float radius = 0.6f;

    [Header("Rewards")]
    public float xpReward = 5f;

    [Header("Animation")]
    public int frameCount = 7;
    public float animSpeed = 8f;

    [Header("Rendering")]
    public Material material;
    [Tooltip("Tamaño visual en unidades de mundo. Independiente del radio de colisión.")]
    public float visualScale = 1f;

    [Header("Behavior")]
    public EnemyBehaviorType behavior;

    [Header("Drops")]
    public EnemyDropEntry[] dropTable;
}

public enum EnemyBehaviorType
{
    Chase
}