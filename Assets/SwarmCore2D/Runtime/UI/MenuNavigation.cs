using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Attach to the root Canvas or panel of any menu.
// Mouse mode  → hover and click work normally, cursor visible, no forced selection.
// Keyboard mode → a button is always selected, arrows navigate, Space confirms.
public class MenuNavigation : MonoBehaviour
{
    [Tooltip("The button to select first. Leave empty to auto-pick the first active child Button.")]
    public Button firstSelected;

    Button[] navButtons;

    void Start()
    {
        Rebuild();
        if (UIInputMode.Current == UIInputMode.Mode.Keyboard)
            StartCoroutine(SelectFirstNextFrame());
    }

    void OnEnable()
    {
        UIInputMode.OnModeChanged += OnInputModeChanged;
        Rebuild();
        if (UIInputMode.Current == UIInputMode.Mode.Keyboard)
            StartCoroutine(SelectFirstNextFrame());
    }

    void OnDisable()
    {
        UIInputMode.OnModeChanged -= OnInputModeChanged;
    }

    void Update()
    {
        if (UIInputMode.Current != UIInputMode.Mode.Keyboard) return;

        // Restore selection if it was cleared while in keyboard mode.
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            SelectFirst();

        // Space confirms the focused button.
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            InvokeSelected();
    }

    void OnInputModeChanged(UIInputMode.Mode mode)
    {
        if (mode == UIInputMode.Mode.Keyboard)
            SelectFirst();
        // Mouse mode: leave EventSystem alone — pointer clicks work without a selected object.
    }

    // ── Navigation setup ────────────────────────────────────────────────────────

    void Rebuild()
    {
        navButtons = GetComponentsInChildren<Button>(false);
        EnsureNavigationEnabled();
    }

    void EnsureNavigationEnabled()
    {
        if (navButtons == null) return;
        foreach (var btn in navButtons)
        {
            if (btn == null) continue;
            var nav = btn.navigation;
            if (nav.mode == Navigation.Mode.None)
            {
                nav.mode = Navigation.Mode.Automatic;
                btn.navigation = nav;
            }
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    IEnumerator SelectFirstNextFrame()
    {
        yield return null;
        SelectFirst();
    }

    void SelectFirst()
    {
        var target = GetFirstActive();
        if (target != null)
            EventSystem.current?.SetSelectedGameObject(target.gameObject);
    }

    Button GetFirstActive()
    {
        if (firstSelected != null && firstSelected.isActiveAndEnabled && firstSelected.interactable)
            return firstSelected;
        if (navButtons == null) return null;
        foreach (var btn in navButtons)
            if (btn != null && btn.isActiveAndEnabled && btn.interactable)
                return btn;
        return null;
    }

    void InvokeSelected()
    {
        var go = EventSystem.current?.currentSelectedGameObject;
        if (go == null) return;
        var btn = go.GetComponent<Button>();
        if (btn != null && btn.interactable) btn.onClick.Invoke();
    }
}
