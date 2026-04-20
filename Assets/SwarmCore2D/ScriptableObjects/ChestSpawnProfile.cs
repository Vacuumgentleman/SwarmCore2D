using UnityEngine;

[System.Serializable]
public struct ChestTierWeight
{
    public ChestData data;
    [Tooltip("Peso relativo de este tier (mayor = más probable)")]
    public float weight;
}

[CreateAssetMenu(fileName = "ChestSpawnProfile", menuName = "SwarmCore2D/World/Chest Spawn Profile")]
public class ChestSpawnProfile : ScriptableObject
{
    [Tooltip("Probabilidad de que un enemigo al morir spawne un cofre (0 = nunca, 1 = siempre)")]
    [Range(0f, 1f)]
    public float baseSpawnChance = 0.05f;

    [Tooltip("Tiers disponibles con sus pesos de aparición")]
    public ChestTierWeight[] tiers;
}
