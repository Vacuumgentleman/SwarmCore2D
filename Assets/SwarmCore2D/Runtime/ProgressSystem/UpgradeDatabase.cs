using System.Collections.Generic;
using UnityEngine;

public class UpgradeDatabase : MonoBehaviour
{
    public static UpgradeDatabase Instance;

    public UpgradeData[] upgrades;

    void Awake()
    {
        Instance = this;
    }

    public UpgradeData GetRandom()
    {
        if (upgrades == null || upgrades.Length == 0)
            return null;

        int i = Random.Range(0, upgrades.Length);
        return upgrades[i];
    }

    public UpgradeData GetRandomForWeapon(WeaponStats weapon)
    {
        if (upgrades == null || upgrades.Length == 0)
            return null;

        var pool = new List<UpgradeData>();
        foreach (var u in upgrades)
        {
            if (u == null) continue;

            if (u.isGlobal)
            {
                pool.Add(u);
                continue;
            }

            // Weapon-specific: include if no filter defined or this type is allowed
            if (weapon == null
                || weapon.allowedWeaponUpgrades.Count == 0
                || weapon.allowedWeaponUpgrades.Contains(u.type))
            {
                pool.Add(u);
            }
        }

        if (pool.Count == 0)
            return GetRandom();

        return pool[Random.Range(0, pool.Count)];
    }
}