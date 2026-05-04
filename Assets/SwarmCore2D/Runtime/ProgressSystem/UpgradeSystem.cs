using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
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
        isOpen = true;
        SetPausedState(true);

        if (upgradePanel != null)
            upgradePanel.SetActive(true);

        if (UpgradeUI.Instance != null)
            UpgradeUI.Instance.GenerateOptions();
    }

    bool isOpen = false;

    void Update()
    {
        if (!isOpen) return;
        if (Keyboard.current == null) return;

        if (UIInputMode.Current == UIInputMode.Mode.Keyboard &&
            (Keyboard.current.spaceKey.wasPressedThisFrame ||
             (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)))
        {
            InvokeSelected();
        }
    }

    void InvokeSelected()
    {
        var go = EventSystem.current?.currentSelectedGameObject;
        if (go == null) return;
        var btn = go.GetComponent<Button>();
        if (btn != null && btn.interactable)
            btn.onClick.Invoke();
    }

    public void CloseSelection()
    {
        isOpen = false;
        SetPausedState(false);
        EventSystem.current?.SetSelectedGameObject(null);

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