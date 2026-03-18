using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Biome")]
public class BiomeData : ScriptableObject
{
    [Header("General")]
    public string biomeName;

    [Header("Ground")]
    public Material groundMaterial;

    [Header("Noise")]
    public float noiseScale = 0.05f;
    public float thresholdMin = 0f;
    public float thresholdMax = 1f;

    [Header("Props")]
    public PropData[] props;
}