using UnityEngine;

[System.Serializable]
public class PlayerStatsRuntime
{
    [Header("Global Multipliers")]
    public float damageMultiplier = 1f;
    public float cooldownMultiplier = 1f;
    public float areaMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float durationMultiplier = 1f;

    [Header("Extra Mechanics")]
    public int extraProjectiles = 0;
    public int pierceBonus = 0;
    public float critChance = 0f;
    public float critMultiplier = 2f;
    public float lifeSteal = 0f;

    public void Reset()
    {
        damageMultiplier = 1f;
        cooldownMultiplier = 1f;
        areaMultiplier = 1f;
        speedMultiplier = 1f;
        durationMultiplier = 1f;
        extraProjectiles = 0;
        pierceBonus = 0;
        critChance = 0f;
        critMultiplier = 2f;
        lifeSteal = 0f;
    }
}