using UnityEngine;

[CreateAssetMenu(menuName = "SwarmCore2D/Prop")]
public class PropData : ScriptableObject
{
    [Header("Rendering")]
    public Mesh mesh;
    public Material material;

    [Header("Sorting")]
    public int baseSorting = 0;

    [Header("Spawn")]
    public float spawnChance = 0.1f;
    public float minNoise = 0.5f;
    public float noiseScale = 0.1f;
    public Vector2 noiseOffset;

    [Header("Placement")]
    public float minDistance = 1.0f;

    [Header("Scale")]
    public Vector2 scaleRange = new Vector2(1f, 1f);
}