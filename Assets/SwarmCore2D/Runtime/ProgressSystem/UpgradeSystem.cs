using UnityEngine;

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
        if (simulation != null)
            simulation.SetPaused(true);

        if (upgradePanel != null)
            upgradePanel.SetActive(true);

        if (UpgradeUI.Instance != null)
            UpgradeUI.Instance.GenerateOptions();
    }

    public void CloseSelection()
    {
        if (simulation != null)
            simulation.SetPaused(false);

        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }
}