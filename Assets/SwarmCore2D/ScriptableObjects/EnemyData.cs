using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public float maxHealth = 10f;
    public float speed = 3f;
    public float damage = 5f;
    public float radius = 0.6f;

    [Header("Rendering")]
    public Sprite[] frames;
    public float animSpeed = 8f;

    [Header("Behavior")]
    public EnemyBehaviorType behavior;
}

public enum EnemyBehaviorType
{
    Chase,
    // luego: Orbit, Flee, Ranged...
}