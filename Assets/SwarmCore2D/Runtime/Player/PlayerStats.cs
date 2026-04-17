using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int kills;

    public PlayerStatsRuntime stats = new PlayerStatsRuntime();

    public event Action OnKillsChanged;

    [System.Serializable]
    public class ActiveBuff
    {
        public DropType type;
        public float originalValue;
        public float remainingTime;
    }

    List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        TickBuffs();
    }

    public void AddKill()
    {
        kills++;
        OnKillsChanged?.Invoke();
    }

    public void AddBuff(DropType type, float value, float duration)
    {
        if (duration <= 0f) return;

        float original = ApplyBuff(type, value);
        activeBuffs.Add(new ActiveBuff { type = type, originalValue = original, remainingTime = duration });
    }

    float ApplyBuff(DropType type, float value)
    {
        switch (type)
        {
            case DropType.DamageBoost:
                float prev = stats.damageMultiplier;
                stats.damageMultiplier *= value;
                return prev;
            case DropType.SpeedBoost:
                float prevS = stats.speedMultiplier;
                stats.speedMultiplier *= value;
                return prevS;
        }
        return 0f;
    }

    void TickBuffs()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            activeBuffs[i].remainingTime -= Time.deltaTime;
            if (activeBuffs[i].remainingTime <= 0f)
            {
                RevertBuff(activeBuffs[i]);
                activeBuffs.RemoveAt(i);
            }
        }
    }

    void RevertBuff(ActiveBuff buff)
    {
        switch (buff.type)
        {
            case DropType.DamageBoost: stats.damageMultiplier = buff.originalValue; break;
            case DropType.SpeedBoost:  stats.speedMultiplier  = buff.originalValue; break;
        }
    }
}