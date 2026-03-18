using UnityEngine;

[System.Serializable]
public class PropData
{
    public string name;

    public Material material;
    public Vector2 noiseOffset;

    [Header("Layer")]
    public int layerIndex; 

    [Header("Spawn")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;

    public float noiseScale = 0.1f;
    public float minNoise = 0.5f;

    [Header("Spacing")]
    public float minDistance = 1.5f;

    [Header("Scale")]
    public Vector2 scaleRange = new Vector2(1f, 1.5f);
}