using UnityEngine;

[CreateAssetMenu(menuName = "SwarmCore2D/Prop")]
public class PropData : ScriptableObject
{
    public Mesh mesh;
    public Material material;

    public float spawnChance = 0.1f;
    public float minNoise = 0.5f;
    public float noiseScale = 0.1f;
    public Vector2 noiseOffset;

    public float minDistance = 1.0f;

    public Vector2 scaleRange = new Vector2(1f, 1f);

    [Header("Animation")]
    public bool animated = false;
    public Vector2 animSpeedRange = new Vector2(0.8f, 1.2f);
}