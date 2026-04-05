using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Image icon;

    UpgradeData data;

    public void Setup(UpgradeData upgrade)
    {
        data = upgrade;

        if (upgrade == null)
            return;

        title.text = upgrade.upgradeName;
        desc.text = upgrade.description;

        if (icon != null)
            icon.sprite = upgrade.icon;
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

        if (weaponController == null)
            return;

        var runtime = weaponController.runtime;

        switch (data.type)
        {
            case UpgradeData.UpgradeType.Damage:
                runtime.damage += data.value;
                break;

            case UpgradeData.UpgradeType.AttackSpeed:
                runtime.cooldown *= (1f - data.value);
                break;

            case UpgradeData.UpgradeType.ProjectileCount:
            runtime.amount += (int)data.value; 
            break;

            case UpgradeData.UpgradeType.AddDirectionRandom:
                AddRandomDirection(runtime);
                weaponController.RebuildDirections();
                break;

            case UpgradeData.UpgradeType.Size: // 🔥 UNIFICADO
                runtime.radius += data.value;          // melee
                runtime.projectileSize += data.value;  // proyectiles
                break;

            case UpgradeData.UpgradeType.Knockback:
                runtime.knockback += data.value;
                break;

            case UpgradeData.UpgradeType.MaxHealth:
                if (player != null)
                {
                    player.maxHealth += data.value;
                    player.currentHealth += data.value;
                }
                break;

            case UpgradeData.UpgradeType.Heal:
                if (player != null)
                {
                    player.currentHealth += data.value;
                    player.currentHealth = Mathf.Min(player.currentHealth, player.maxHealth);
                }
                break;
        }
    }

    void AddRandomDirection(WeaponRuntimeStats runtime)
    {
        List<System.Action> possible = new List<System.Action>();

        if (!runtime.attackUp)
            possible.Add(() => runtime.attackUp = true);

        if (!runtime.attackDown)
            possible.Add(() => runtime.attackDown = true);

        if (!runtime.attackLeft)
            possible.Add(() => runtime.attackLeft = true);

        if (!runtime.attackRight)
            possible.Add(() => runtime.attackRight = true);

        if (possible.Count == 0)
            return;

        int index = Random.Range(0, possible.Count);
        possible[index].Invoke();
    }
}