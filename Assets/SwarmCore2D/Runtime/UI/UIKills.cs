using UnityEngine;
using TMPro;

public class UIKills : MonoBehaviour
{
    public TextMeshProUGUI text;

    PlayerStats stats;

    void Start()
    {
        stats = Object.FindFirstObjectByType<PlayerStats>();

        if (stats == null)
        {
            Debug.LogError("PlayerStats no encontrado");
            return;
        }

        stats.OnKillsChanged += UpdateUI;

        UpdateUI();
    }

    void UpdateUI()
    {
        text.text = "Kills: " + stats.kills;
    }
}