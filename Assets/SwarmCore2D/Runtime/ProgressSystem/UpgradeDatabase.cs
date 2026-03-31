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
}