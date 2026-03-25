using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILevelXP : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Image xpBar;

    PlayerProgress progress;

    void Start()
    {
        progress = Object.FindFirstObjectByType<PlayerProgress>();

        if (progress == null)
        {
            Debug.LogError("PlayerProgress no encontrado");
            return;
        }

        progress.OnXPChanged += UpdateUI;
        progress.OnLevelUp += UpdateUI;

        UpdateUI();
    }

    void UpdateUI()
    {
        levelText.text = "Lv " + progress.level;
        xpBar.fillAmount = progress.GetXPPercent();
    }
}