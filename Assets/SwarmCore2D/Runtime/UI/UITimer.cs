using UnityEngine;
using TMPro;

public class UITimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    GameTimer timer;

    void Start()
    {
        timer = Object.FindFirstObjectByType<GameTimer>();

        if (timer == null)
        {
            Debug.LogError("GameTimer no encontrado");
            return;
        }

        timer.OnTimeChanged += UpdateUI;

        UpdateUI();
    }

    void UpdateUI()
    {
        timerText.text = timer.GetFormattedTime();
    }
}