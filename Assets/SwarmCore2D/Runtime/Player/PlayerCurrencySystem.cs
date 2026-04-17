using UnityEngine;
using System;

public class PlayerCurrencySystem : MonoBehaviour
{
    public static PlayerCurrencySystem Instance;

    public int coins;

    public event Action OnCoinsChanged;

    void Awake()
    {
        Instance = this;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke();
    }
}
