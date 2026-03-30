using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Enemy")]
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

    [Header("Behavior")]
    public EnemyBehaviorType behavior;
}

public enum EnemyBehaviorType
{
    Chase
}