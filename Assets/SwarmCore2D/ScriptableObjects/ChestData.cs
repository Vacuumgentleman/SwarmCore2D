using UnityEngine;

[CreateAssetMenu(fileName = "ChestData", menuName = "SwarmCore2D/World/Chest Data")]
public class ChestData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Nombre del tier (Wooden, Iron, Silver, Golden)")]
    public string tierName;

    [Header("Prefab")]
    [Tooltip("Prefab del cofre de este tier (debe tener el componente Chest de Cainos)")]
    public GameObject prefab;

    [Header("Interaction")]
    [Tooltip("Distancia máxima desde la que el jugador puede interactuar")]
    public float interactRadius = 1.5f;

    [Header("Loot")]
    [Tooltip("Tabla de drops que suelta este cofre al abrirse")]
    public EnemyDropEntry[] lootTable;
}
