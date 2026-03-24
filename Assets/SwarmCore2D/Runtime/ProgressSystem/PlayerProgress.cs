using UnityEngine;
using System;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance;

    [Header("Level")]
    public int level = 1;

    [Header("XP")]
    public float currentXP = 0f;
    public float xpToNext = 10f;

    public event Action OnXPChanged;
    public event Action OnLevelUp;

    void Awake()
    {
        Instance = this;
    }

    public void AddXP(float amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNext)
        {
            currentXP -= xpToNext;
            LevelUp();
        }

        OnXPChanged?.Invoke();
    }

    void LevelUp()
    {
        level++;

        // curva de dificultad (puedes cambiarla luego)
        xpToNext *= 1.5f;

        OnLevelUp?.Invoke();
    }

    public float GetXPPercent()
    {
        return currentXP / xpToNext;
    }
}