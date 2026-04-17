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

        for (int i = 0; i < buttons.Length; i++)
        {
            int weaponIndex = Random.Range(0, weaponCount);
            WeaponStats stats = weaponController != null ? weaponController.weapons[weaponIndex] : null;
            UpgradeData data = UpgradeDatabase.Instance.GetRandomForWeapon(stats);
            int resolvedIndex = (data != null && !data.isGlobal) ? weaponIndex : -1;
            buttons[i].Setup(data, resolvedIndex);
        }
    }
}
