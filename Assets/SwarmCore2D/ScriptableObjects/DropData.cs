using UnityEngine;

public enum DropType { Coin, Heal, DamageBoost, SpeedBoost, WeaponUnlock }

[System.Serializable]
public class EnemyDropEntry
{
    public DropData drop;
    [Range(0f, 1f)] public float chance = 0.3f;
    public int minCount = 1;
    public int maxCount = 1;
}

[CreateAssetMenu(fileName = "DropData", menuName = "SwarmCore2D/Combat/Drop")]
public class DropData : ScriptableObject
{
    [Header("Effect")]
    public DropType type;
    [Tooltip("Monedas a otorgar, vida recuperada, o multiplicador de buff")]
    public float value = 1f;
    [Tooltip("Duración del buff en segundos. 0 = instantáneo")]
    public float duration = 0f;

    [Header("Collection")]
    [Tooltip("Radio en unidades en el que el jugador recoge automáticamente")]
    public float collectRadius = 1.5f;

    [Header("Weapon Unlock")]
    [Tooltip("Arma a desbloquear al recoger (solo cuando type = WeaponUnlock)")]
    public WeaponStats weaponToUnlock;

    [Header("Visual")]
    public Material material;
    public int frameCount = 1;
    public float frameRate = 8f;
    public float visualScale = 0.5f;
    public Color tint = Color.white;
}
