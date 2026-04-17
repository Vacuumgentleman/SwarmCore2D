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
            UpgradeData data = UpgradeDatabase.Instance.GetRandom();

            int weaponIndex = -1;
            if (data != null && !data.isGlobal)
                weaponIndex = Random.Range(0, weaponCount);

            buttons[i].Setup(data, weaponIndex);
        }
    }
}
