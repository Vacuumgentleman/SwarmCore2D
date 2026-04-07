using UnityEngine;
using SwarmCore2D.Core;

public class UpgradeSystem : MonoBehaviour
{
    public GameObject upgradePanel;

    SwarmCore2D.Simulation.SwarmSimulationController simulation;

    void Start()
    {
        simulation = FindFirstObjectByType<SwarmCore2D.Simulation.SwarmSimulationController>();

        if (PlayerProgress.Instance != null)
            PlayerProgress.Instance.OnLevelUp += OpenSelection;

        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    void OpenSelection()
    {
        SetPausedState(true);

        if (upgradePanel != null)
            upgradePanel.SetActive(true);

        if (UpgradeUI.Instance != null)
            UpgradeUI.Instance.GenerateOptions();
    }

    public void CloseSelection()
    {
        SetPausedState(false);

        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    void SetPausedState(bool paused)
    {
        if (simulation != null)
            simulation.SetPaused(paused);

        SwarmTime.Paused = paused;

        if (GameTimer.Instance != null)
        {
            if (paused)
                GameTimer.Instance.StopTimer();
            else
                GameTimer.Instance.StartTimer();
        }
    }
}