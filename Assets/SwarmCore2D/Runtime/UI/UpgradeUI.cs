using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance;

    public UpgradeButton[] buttons;

    public UpgradeSystem system;

    void Awake()
    {
        Instance = this;
    }

    public void GenerateOptions()
    {
        var weaponController = FindFirstObjectByType<PlayerWeaponController>();
        int weaponCount = weaponController != null ? weaponController.WeaponCount : 1;

        var seen = new HashSet<(int weaponIndex, UpgradeData.UpgradeType type)>();
        const int maxAttempts = 50;

        for (int i = 0; i < buttons.Length; i++)
        {
            UpgradeData data        = null;
            int         resolvedIndex = -1;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int        candidateWeapon = Random.Range(0, weaponCount);
                WeaponStats stats          = weaponController != null ? weaponController.weapons[candidateWeapon] : null;
                UpgradeData candidate      = UpgradeDatabase.Instance.GetRandomForWeapon(stats);

                if (candidate == null) break;

                int  key        = candidate.isGlobal ? -1 : candidateWeapon;
                var  combo      = (key, candidate.type);

                if (seen.Contains(combo)) continue;

                seen.Add(combo);
                data          = candidate;
                resolvedIndex = candidate.isGlobal ? -1 : candidateWeapon;
                break;
            }

            buttons[i].Setup(data, resolvedIndex);
        }
    }
}
