using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Attach to the root of the Options/Volume panel.
//
// Layout expected (assign in Inspector, top to bottom):
//   - Sliders[]   : one or more horizontal Sliders (music, sfx, etc.)
//   - backButton  : the "Back / Volver" Button
//
// Controls (Keyboard mode only for navigation/confirm):
//   Up / Down      → move between sliders and back button
//   Left / Right   → change the focused slider's value
//   Space / Enter  → click the focused back button
//   B (gamepad) / Escape → go back regardless of mode
public class OptionsMenuNavigation : MonoBehaviour
{
    [Header("Elements — assign top to bottom")]
    public Slider[] sliders;
    public Button backButton;

    [Tooltip("How much each left/right press moves a slider (0–1 range).")]
    [Range(0.01f, 0.2f)]
    public float sliderStep = 0.05f;

    void OnEnable()
    {
        UIInputMode.OnModeChanged += OnInputModeChanged;
        SetupNavigation();
        if (UIInputMode.Current == UIInputMode.Mode.Keyboard)
            StartCoroutine(SelectFirstNextFrame());
    }

    void OnDisable()
    {
        UIInputMode.OnModeChanged -= OnInputModeChanged;
        EventSystem.current?.SetSelectedGameObject(null);
    }

    void Update()
    {
        // B / Escape go back regardless of input mode.
        if (ShouldGoBack())
        {
            GoBack();
            return;
        }

        if (UIInputMode.Current != UIInputMode.Mode.Keyboard) return;

        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            SelectFirst();

        HandleSliderInput();
        HandleSubmit();
    }

    // ── Mode change ────────────────────────────────────────────────────────────

    void OnInputModeChanged(UIInputMode.Mode mode)
    {
        if (mode == UIInputMode.Mode.Keyboard)
            SelectFirst();
        else
            EventSystem.current?.SetSelectedGameObject(null);
    }

    // ── Input handlers ─────────────────────────────────────────────────────────

    bool ShouldGoBack()
    {
        if (Gamepad.current  != null && Gamepad.current.buttonEast.wasPressedThisFrame)   return true;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)   return true;
        return false;
    }

    void HandleSubmit()
    {
        if (Keyboard.current == null || !Keyboard.current.spaceKey.wasPressedThisFrame) return;
        var go  = EventSystem.current?.currentSelectedGameObject;
        var btn = go?.GetComponent<Button>();
        if (btn != null && btn.interactable) btn.onClick.Invoke();
    }

    float moveHoldTimer;
    const float MoveRepeatDelay = 0.4f;
    const float MoveRepeatRate  = 0.1f;

    void HandleSliderInput()
    {
        var go     = EventSystem.current?.currentSelectedGameObject;
        var slider = go?.GetComponent<Slider>();
        if (slider == null) { moveHoldTimer = 0f; return; }

        float axis = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed  || Keyboard.current.aKey.isPressed)  axis = -1f;
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)  axis =  1f;
        }

        if (Gamepad.current != null)
        {
            float pad = Gamepad.current.leftStick.x.ReadValue();
            if (Mathf.Abs(pad) > 0.5f) axis = Mathf.Sign(pad);
            if (Gamepad.current.dpad.left.isPressed)  axis = -1f;
            if (Gamepad.current.dpad.right.isPressed) axis =  1f;
        }

        if (Mathf.Abs(axis) < 0.01f) { moveHoldTimer = 0f; return; }

        moveHoldTimer += Time.unscaledDeltaTime;

        bool step = moveHoldTimer <= Time.unscaledDeltaTime + 0.001f
                 || (moveHoldTimer > MoveRepeatDelay && moveHoldTimer % MoveRepeatRate < Time.unscaledDeltaTime);

        if (step)
            slider.value = Mathf.Clamp01(slider.value + axis * sliderStep);
    }

    // ── Navigation setup ───────────────────────────────────────────────────────

    void SetupNavigation()
    {
        var elements = new List<Selectable>();
        if (sliders != null)
            foreach (var s in sliders)
                if (s != null) elements.Add(s);
        if (backButton != null)
            elements.Add(backButton);

        int count = elements.Count;
        for (int i = 0; i < count; i++)
        {
            var nav = elements[i].navigation;
            nav.mode          = Navigation.Mode.Explicit;
            nav.selectOnLeft  = null;
            nav.selectOnRight = null;
            nav.selectOnUp    = elements[(i - 1 + count) % count];
            nav.selectOnDown  = elements[(i + 1) % count];
            elements[i].navigation = nav;
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    IEnumerator SelectFirstNextFrame()
    {
        yield return null;
        SelectFirst();
    }

    void SelectFirst()
    {
        Selectable target = null;
        if (sliders != null && sliders.Length > 0 && sliders[0] != null && sliders[0].isActiveAndEnabled)
            target = sliders[0];
        else if (backButton != null && backButton.isActiveAndEnabled)
            target = backButton;

        if (target != null)
            EventSystem.current?.SetSelectedGameObject(target.gameObject);
    }

    void GoBack()
    {
        if (backButton != null && backButton.isActiveAndEnabled && backButton.interactable)
            backButton.onClick.Invoke();
    }
}
