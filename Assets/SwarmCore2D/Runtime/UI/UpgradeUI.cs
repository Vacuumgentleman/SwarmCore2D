using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class TierDefinition
{
    public string tierName  = "Common";
    [Range(0f, 1f)] public float weight = 0.4f;
    public float multiplier = 1.0f;
    public Sprite background;
}

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance;

    [Header("Buttons")]
    public UpgradeCardUI[] buttons;
    public UpgradeSystem system;

    [Header("Tiers")]
    public TierDefinition[] tiers;

    void Awake()
    {
        Instance = this;
    }

    // Called by Unity when the component is first added in the Inspector
    void Reset()
    {
        tiers = new TierDefinition[]
        {
            new TierDefinition { tierName = "Common",    weight = 0.40f, multiplier = 1.0f },
            new TierDefinition { tierName = "Uncommon",  weight = 0.30f, multiplier = 1.3f },
            new TierDefinition { tierName = "Rare",      weight = 0.20f, multiplier = 1.6f },
            new TierDefinition { tierName = "Epic",      weight = 0.08f, multiplier = 2.0f },
            new TierDefinition { tierName = "Legendary", weight = 0.02f, multiplier = 3.0f },
        };
    }

    public TierDefinition RollTier()
    {
        if (tiers == null || tiers.Length == 0) return null;

        float total = 0f;
        foreach (var t in tiers) total += t.weight;

        float roll       = Random.value * total;
        float cumulative = 0f;

        foreach (var t in tiers)
        {
            cumulative += t.weight;
            if (roll < cumulative) return t;
        }

        return tiers[0];
    }

    public void GenerateOptions()
    {
        var weaponController = FindFirstObjectByType<PlayerWeaponController>();
        int weaponCount = weaponController != null ? weaponController.WeaponCount : 1;

        var seen = new HashSet<(int weaponIndex, UpgradeData.UpgradeType type)>();
        const int maxAttempts = 50;

        for (int i = 0; i < buttons.Length; i++)
        {
            UpgradeData data          = null;
            int         resolvedIndex = -1;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int         candidateWeapon = Random.Range(0, weaponCount);
                WeaponStats stats           = weaponController != null ? weaponController.weapons[candidateWeapon] : null;
                UpgradeData candidate       = UpgradeDatabase.Instance.GetRandomForWeapon(stats);

                if (candidate == null) break;

                int key   = candidate.isGlobal ? -1 : candidateWeapon;
                var combo = (key, candidate.type);

                if (seen.Contains(combo)) continue;

                seen.Add(combo);
                data          = candidate;
                resolvedIndex = candidate.isGlobal ? -1 : candidateWeapon;
                break;
            }

            buttons[i].Setup(data, resolvedIndex);
        }

        SetupCardNavigation();
    }

    void SetupCardNavigation()
    {
        var activeButtons = new List<Button>();
        foreach (var card in buttons)
        {
            if (card == null || !card.gameObject.activeSelf) continue;
            var btn = card.GetComponent<Button>();
            if (btn != null) activeButtons.Add(btn);
        }

        for (int i = 0; i < activeButtons.Count; i++)
        {
            var nav = activeButtons[i].navigation;
            nav.mode         = Navigation.Mode.Explicit;
            nav.selectOnUp   = i > 0 ? activeButtons[i - 1] : activeButtons[activeButtons.Count - 1];
            nav.selectOnDown = i < activeButtons.Count - 1 ? activeButtons[i + 1] : activeButtons[0];
            nav.selectOnLeft  = null;
            nav.selectOnRight = null;
            activeButtons[i].navigation = nav;
        }

        if (activeButtons.Count > 0 && UIInputMode.Current == UIInputMode.Mode.Keyboard)
            EventSystem.current?.SetSelectedGameObject(activeButtons[0].gameObject);
    }
}
