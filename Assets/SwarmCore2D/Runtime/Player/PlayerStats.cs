using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int kills;

    public event Action OnKillsChanged;

    void Awake()
    {
        Instance = this;
    }

    public void AddKill()
    {
        kills++;
        OnKillsChanged?.Invoke();
    }
}