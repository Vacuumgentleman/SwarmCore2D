using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    static readonly Dictionary<(UpgradeData, int), int> upgradeLevels = new();

    [Header("Texts")]
    public TMP_Text lvlText;    // "LVL X"       — wire to Upgrades Text A
    public TMP_Text tierText;   // tier name      — wire to Tier Text A
    public TMP_Text nameText;   // upgrade name   — wire to Upgrades Type Text A
    public TMP_Text statBefore;
    public TMP_Text statAfter;

    [Header("Icon")]
    public Image upgradeIcon;   // wire to Image Weapon Button A

    [Header("Background")]
    public Image cardBackground;

    UpgradeData data;
    int         resolvedWeaponIndex;
    float       effectiveValue;

    void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn == null) return;

        if (cardBackground != null)
        {
            btn.targetGraphic = cardBackground;
        }
        else
        {
            // Reuse an existing Image if present (e.g., default Button Image), or add one.
            var img = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
            img.color = Color.clear;
            img.raycastTarget = true;
            btn.targetGraphic = img;
        }
    }

    public void Setup(UpgradeData upgrade, int weaponIndexOverride = -1)
    {
        data = upgrade;

        if (upgrade == null) { gameObject.SetActive(false); return; }
        gameObject.SetActive(true);

        resolvedWeaponIndex = (!upgrade.isGlobal && weaponIndexOverride >= 0)
            ? weaponIndexOverride
            : upgrade.weaponIndex;

        TierDefinition tier = UpgradeUI.Instance != null ? UpgradeUI.Instance.RollTier() : null;
        effectiveValue = upgrade.value * (tier != null ? tier.multiplier : 1f);

        int trackingIndex = upgrade.isGlobal ? -1 : resolvedWeaponIndex;
        int currentLevel  = GetLevel(upgrade, trackingIndex);

        if (lvlText != null)
            lvlText.text = $"LVL {currentLevel + 1}";

        if (tierText != null)
            tierText.text = tier != null ? tier.tierName : string.Empty;

        if (nameText != null)
            nameText.text = upgrade.upgradeName;

        if (upgradeIcon != null)
        {
            upgradeIcon.sprite  = upgrade.icon;
            upgradeIcon.enabled = upgrade.icon != null;
        }

        if (cardBackground != null && tier?.background != null)
            cardBackground.sprite = tier.background;

        UpdateStatDisplay();
    }

    void UpdateStatDisplay()
    {
        float before = GetCurrentValue();
        float after  = ComputeAfterValue(before, effectiveValue);

        if (statBefore != null) statBefore.text = FormatValue(before, data.type);
        if (statAfter  != null) statAfter.text  = FormatValue(after,  data.type);
    }

    float GetCurrentValue()
    {
        var global = PlayerStats.Instance?.stats;
        var wc     = PlayerWeaponController.Instance;
        var rt     = wc?.GetRuntime(resolvedWeaponIndex);

        switch (data.type)
        {
            case UpgradeData.UpgradeType.GlobalDamage:      return global?.damageMultiplier  ?? 1f;
            case UpgradeData.UpgradeType.GlobalAttackSpeed: return global?.cooldownMultiplier ?? 1f;
            case UpgradeData.UpgradeType.GlobalArea:        return global?.areaMultiplier     ?? 1f;
            case UpgradeData.UpgradeType.GlobalSpeed:       return global?.speedMultiplier    ?? 1f;
            case UpgradeData.UpgradeType.GlobalDuration:    return global?.durationMultiplier ?? 1f;
            case UpgradeData.UpgradeType.GlobalProjectile:  return global?.extraProjectiles   ?? 0f;
            case UpgradeData.UpgradeType.GlobalPierce:      return global?.pierceBonus        ?? 0f;
            case UpgradeData.UpgradeType.GlobalCritChance:  return global?.critChance         ?? 0f;
            case UpgradeData.UpgradeType.GlobalLifeSteal:   return global?.lifeSteal          ?? 0f;
            case UpgradeData.UpgradeType.Damage:            return rt?.damage      ?? 0f;
            case UpgradeData.UpgradeType.AttackSpeed:       return rt?.cooldown    ?? 0f;
            case UpgradeData.UpgradeType.ProjectileAmount:  return rt?.amount      ?? 0f;
            case UpgradeData.UpgradeType.Pierce:            return rt?.pierceCount ?? 0f;
            case UpgradeData.UpgradeType.Size:              return rt?.hitRadius   ?? 0f;
            case UpgradeData.UpgradeType.Knockback:         return rt?.knockback   ?? 0f;
            case UpgradeData.UpgradeType.MaxHealth:         return FindFirstObjectByType<PlayerHealth>()?.maxHealth ?? 0f;
            default:                                        return effectiveValue;
        }
    }

    float ComputeAfterValue(float before, float v)
    {
        switch (data.type)
        {
            case UpgradeData.UpgradeType.AttackSpeed:
            case UpgradeData.UpgradeType.GlobalAttackSpeed:
                return before * (1f - v);
            default:
                return before + v;
        }
    }

    string FormatValue(float val, UpgradeData.UpgradeType type)
    {
        switch (type)
        {
            case UpgradeData.UpgradeType.GlobalDamage:
            case UpgradeData.UpgradeType.GlobalAttackSpeed:
            case UpgradeData.UpgradeType.GlobalArea:
            case UpgradeData.UpgradeType.GlobalSpeed:
            case UpgradeData.UpgradeType.GlobalDuration:
            case UpgradeData.UpgradeType.GlobalCritChance:
            case UpgradeData.UpgradeType.GlobalLifeSteal:
                return $"{val * 100f:0}%";
            case UpgradeData.UpgradeType.GlobalProjectile:
            case UpgradeData.UpgradeType.GlobalPierce:
            case UpgradeData.UpgradeType.ProjectileAmount:
            case UpgradeData.UpgradeType.Pierce:
                return $"{val:0}";
            case UpgradeData.UpgradeType.AttackSpeed:
                return $"{val:0.0}s";
            case UpgradeData.UpgradeType.Heal:
            case UpgradeData.UpgradeType.MaxHealth:
                return $"{val:0} HP";
            default:
                return $"{val:0.0}";
        }
    }

    public void OnClick()
    {
        if (data == null) return;
        int trackingIndex = data.isGlobal ? -1 : resolvedWeaponIndex;
        IncrementLevel(data, trackingIndex);
        ApplyUpgrade();
        FindFirstObjectByType<UpgradeSystem>()?.CloseSelection();
    }

    void ApplyUpgrade()
    {
        var wc     = PlayerWeaponController.Instance;
        var player = FindFirstObjectByType<PlayerHealth>();
        var global = PlayerStats.Instance?.stats;
        float v    = effectiveValue;

        switch (data.type)
        {
            case UpgradeData.UpgradeType.Damage:
                ApplyToWeapon(wc, r => r.damage += v); break;
            case UpgradeData.UpgradeType.AttackSpeed:
                ApplyToWeapon(wc, r => r.cooldown *= (1f - v)); break;
            case UpgradeData.UpgradeType.ProjectileAmount:
                ApplyToWeapon(wc, r => r.amount += Mathf.RoundToInt(v)); break;
            case UpgradeData.UpgradeType.Size:
                ApplyToWeapon(wc, r => { r.hitRadius += v; r.projectileSize += v; }); break;
            case UpgradeData.UpgradeType.Knockback:
                ApplyToWeapon(wc, r => r.knockback += v); break;
            case UpgradeData.UpgradeType.Pierce:
                ApplyToWeapon(wc, r => r.pierceCount += Mathf.RoundToInt(v)); break;
            case UpgradeData.UpgradeType.AddDirectionRandom:
                if (wc != null)
                {
                    var rt = wc.GetRuntime(resolvedWeaponIndex);
                    if (rt != null) { AddRandomDirection(rt); wc.RebuildDirections(resolvedWeaponIndex); }
                }
                break;

            case UpgradeData.UpgradeType.GlobalDamage:
                if (global != null) global.damageMultiplier   += v; break;
            case UpgradeData.UpgradeType.GlobalAttackSpeed:
                if (global != null) global.cooldownMultiplier *= (1f - v); break;
            case UpgradeData.UpgradeType.GlobalProjectile:
                if (global != null) global.extraProjectiles   += Mathf.RoundToInt(v); break;
            case UpgradeData.UpgradeType.GlobalArea:
                if (global != null) global.areaMultiplier     += v; break;
            case UpgradeData.UpgradeType.GlobalSpeed:
                if (global != null) global.speedMultiplier    += v; break;
            case UpgradeData.UpgradeType.GlobalDuration:
                if (global != null) global.durationMultiplier += v; break;
            case UpgradeData.UpgradeType.GlobalPierce:
                if (global != null) global.pierceBonus        += Mathf.RoundToInt(v); break;
            case UpgradeData.UpgradeType.GlobalCritChance:
                if (global != null) global.critChance         += v; break;
            case UpgradeData.UpgradeType.GlobalLifeSteal:
                if (global != null) global.lifeSteal          += v; break;

            case UpgradeData.UpgradeType.MaxHealth:
                player?.AddMaxHealth(v); break;
            case UpgradeData.UpgradeType.Heal:
                player?.Heal(v); break;
        }
    }

    void ApplyToWeapon(PlayerWeaponController wc, System.Action<WeaponRuntimeStats> action)
    {
        if (wc == null) return;

        if (data.isGlobal)
        {
            for (int i = 0; i < wc.WeaponCount; i++)
            {
                var r = wc.GetRuntime(i);
                if (r != null) action(r);
            }
        }
        else
        {
            var r = wc.GetRuntime(resolvedWeaponIndex);
            if (r != null) action(r);
        }
    }

    void AddRandomDirection(WeaponRuntimeStats runtime)
    {
        var possible = new List<System.Action>();

        if (!runtime.attackUp)    possible.Add(() => runtime.attackUp    = true);
        if (!runtime.attackDown)  possible.Add(() => runtime.attackDown  = true);
        if (!runtime.attackLeft)  possible.Add(() => runtime.attackLeft  = true);
        if (!runtime.attackRight) possible.Add(() => runtime.attackRight = true);

        if (possible.Count > 0)
            possible[Random.Range(0, possible.Count)].Invoke();
    }

    static int GetLevel(UpgradeData upgrade, int trackingIndex)
    {
        upgradeLevels.TryGetValue((upgrade, trackingIndex), out int lvl);
        return lvl;
    }

    static void IncrementLevel(UpgradeData upgrade, int trackingIndex)
    {
        var key = (upgrade, trackingIndex);
        upgradeLevels[key] = GetLevel(upgrade, trackingIndex) + 1;
    }

    public static void ClearAllLevels() => upgradeLevels.Clear();
}
