using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Image upgradeIcon;
    public Image weaponIcon;

    UpgradeData data;
    int resolvedWeaponIndex;

    public void Setup(UpgradeData upgrade, int weaponIndexOverride = -1)
    {
        data = upgrade;
        resolvedWeaponIndex = (!upgrade.isGlobal && weaponIndexOverride >= 0)
            ? weaponIndexOverride
            : upgrade.weaponIndex;

        if (upgrade == null)
            return;

        title.text = upgrade.upgradeName;
        desc.text = upgrade.description;

        if (upgradeIcon != null)
            upgradeIcon.sprite = upgrade.icon;

        UpdateWeaponIcon(resolvedWeaponIndex);
    }

    void UpdateWeaponIcon(int wIndex)
    {
        if (weaponIcon == null)
            return;

        if (data.isGlobal)
        {
            weaponIcon.gameObject.SetActive(false);
            return;
        }

        var weaponController = FindFirstObjectByType<PlayerWeaponController>();

        if (weaponController == null || wIndex >= weaponController.weapons.Count)
        {
            weaponIcon.gameObject.SetActive(false);
            return;
        }

        var weapon = weaponController.weapons[wIndex];

        if (weapon == null || weapon.weaponIcon == null)
        {
            weaponIcon.gameObject.SetActive(false);
            return;
        }

        weaponIcon.sprite = weapon.weaponIcon;
        weaponIcon.gameObject.SetActive(true);
    }

    public void OnClick()
    {
        if (data == null)
            return;

        ApplyUpgrade();

        var system = FindFirstObjectByType<UpgradeSystem>();
        if (system != null)
            system.CloseSelection();
    }

    void ApplyUpgrade()
    {
        var weaponController = FindFirstObjectByType<PlayerWeaponController>();
        var player = FindFirstObjectByType<PlayerHealth>();
        var global = PlayerStats.Instance != null ? PlayerStats.Instance.stats : null;

        switch (data.type)
        {
            case UpgradeData.UpgradeType.Damage:
                ApplyToWeapon(weaponController, r => r.damage += data.value);
                break;

            case UpgradeData.UpgradeType.AttackSpeed:
                ApplyToWeapon(weaponController, r => r.cooldown *= (1f - data.value));
                break;

            case UpgradeData.UpgradeType.ProjectileAmount:
                ApplyToWeapon(weaponController, r => r.amount += (int)data.value);
                break;

            case UpgradeData.UpgradeType.Size:
                ApplyToWeapon(weaponController, r =>
                {
                    r.hitRadius += data.value;
                    r.projectileSize += data.value;
                });
                break;

            case UpgradeData.UpgradeType.Knockback:
                ApplyToWeapon(weaponController, r => r.knockback += data.value);
                break;

            case UpgradeData.UpgradeType.Pierce:
                ApplyToWeapon(weaponController, r => r.pierceCount += (int)data.value);
                break;

            case UpgradeData.UpgradeType.AddDirectionRandom:
                if (weaponController != null)
                {
                    var runtime = weaponController.GetRuntime(resolvedWeaponIndex);
                    if (runtime != null)
                    {
                        AddRandomDirection(runtime);
                        weaponController.RebuildDirections(resolvedWeaponIndex);
                    }
                }
                break;

            case UpgradeData.UpgradeType.GlobalDamage:
                if (global != null) global.damageMultiplier += data.value;
                break;

            case UpgradeData.UpgradeType.GlobalAttackSpeed:
                if (global != null) global.cooldownMultiplier *= (1f - data.value);
                break;

            case UpgradeData.UpgradeType.GlobalProjectile:
                if (global != null) global.extraProjectiles += (int)data.value;
                break;

            case UpgradeData.UpgradeType.GlobalArea:
                if (global != null) global.areaMultiplier += data.value;
                break;

            case UpgradeData.UpgradeType.GlobalSpeed:
                if (global != null) global.speedMultiplier += data.value;
                break;

            case UpgradeData.UpgradeType.GlobalDuration:
                if (global != null) global.durationMultiplier += data.value;
                break;

            case UpgradeData.UpgradeType.GlobalPierce:
                if (global != null) global.pierceBonus += (int)data.value;
                break;

            case UpgradeData.UpgradeType.GlobalCritChance:
                if (global != null) global.critChance += data.value;
                break;

            case UpgradeData.UpgradeType.GlobalLifeSteal:
                if (global != null) global.lifeSteal += data.value;
                break;

            case UpgradeData.UpgradeType.MaxHealth:
                if (player != null)
                    player.AddMaxHealth(data.value);
                break;

            case UpgradeData.UpgradeType.Heal:
                if (player != null)
                    player.Heal(data.value);
                break;
        }
    }

    void ApplyToWeapon(PlayerWeaponController weaponController, System.Action<WeaponRuntimeStats> action)
    {
        if (weaponController == null)
            return;

        if (data.isGlobal)
        {
            for (int i = 0; i < weaponController.WeaponCount; i++)
            {
                var r = weaponController.GetRuntime(i);
                if (r != null) action(r);
            }
        }
        else
        {
            var r = weaponController.GetRuntime(resolvedWeaponIndex);
            if (r != null) action(r);
        }
    }

    void AddRandomDirection(WeaponRuntimeStats runtime)
    {
        var possible = new List<System.Action>();

        if (!runtime.attackUp)    possible.Add(() => runtime.attackUp = true);
        if (!runtime.attackDown)  possible.Add(() => runtime.attackDown = true);
        if (!runtime.attackLeft)  possible.Add(() => runtime.attackLeft = true);
        if (!runtime.attackRight) possible.Add(() => runtime.attackRight = true);

        if (possible.Count == 0)
            return;

        possible[Random.Range(0, possible.Count)].Invoke();
    }
}
