using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    public Image healthBar;

    PlayerHealth player;

    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerHealth>();

        if (player == null)
        {
            Debug.LogError("PlayerHealth no encontrado");
            return;
        }

        player.OnHealthChanged += UpdateUI;
        UpdateUI();
    }

    void OnDestroy()
    {
        if (player != null)
            player.OnHealthChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        if (player == null || healthBar == null)
            return;

        healthBar.fillAmount = player.GetHealthPercent();
    }
}