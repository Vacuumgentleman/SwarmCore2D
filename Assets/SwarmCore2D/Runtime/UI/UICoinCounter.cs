using UnityEngine;
using TMPro;

public class UICoinCounter : MonoBehaviour
{
    public TextMeshProUGUI text;

    PlayerCurrencySystem currency;

    void Start()
    {
        currency = PlayerCurrencySystem.Instance;

        if (currency == null)
        {
            Debug.LogError("PlayerCurrencySystem no encontrado");
            return;
        }

        currency.OnCoinsChanged += UpdateUI;

        UpdateUI();
    }

    void UpdateUI()
    {
        text.text = currency != null ? currency.coins.ToString() : "0";
    }
}
