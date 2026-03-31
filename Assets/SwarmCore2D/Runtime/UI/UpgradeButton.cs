using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Image icon;

    public UpgradeSystem system; // 🔥 referencia directa

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

        if (system != null)
            system.CloseSelection();
        else
            Debug.LogError("UpgradeSystem no asignado en botón");
    }

    void ApplyUpgrade()
    {
        var controller = FindFirstObjectByType<PlayerWeaponController>();
        var player = FindFirstObjectByType<PlayerHealth>();

        if (controller == null)
            return;

        var runtime = controller.runtime;

        switch (data.type)
        {
            case UpgradeData.UpgradeType.Damage:
                runtime.damage += data.value;
                break;

            case UpgradeData.UpgradeType.AttackSpeed:
                runtime.cooldown *= (1f - data.value);
                break;

            case UpgradeData.UpgradeType.ProjectileCount:
                runtime.projectileCount += (int)data.value;
                break;

            case UpgradeData.UpgradeType.UnlockDirectionUp:
                runtime.attackUp = true;
                controller.RebuildDirections();
                break;

            case UpgradeData.UpgradeType.UnlockDirectionDown:
                runtime.attackDown = true;
                controller.RebuildDirections();
                break;

            case UpgradeData.UpgradeType.UnlockDirectionLeft:
                runtime.attackLeft = true;
                controller.RebuildDirections();
                break;

            case UpgradeData.UpgradeType.UnlockDirectionRight:
                runtime.attackRight = true;
                controller.RebuildDirections();
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
}