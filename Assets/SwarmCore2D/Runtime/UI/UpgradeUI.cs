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
        for (int i = 0; i < buttons.Length; i++)
        {
            UpgradeData data = UpgradeDatabase.Instance.GetRandom();
            buttons[i].Setup(data);
        }
    }
}