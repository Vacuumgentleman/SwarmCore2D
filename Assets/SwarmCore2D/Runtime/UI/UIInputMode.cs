using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Auto-created at startup — no need to place in any scene.
// Detects whether the last active device was mouse or keyboard/gamepad,
// toggles cursor visibility, and fires OnModeChanged.
// Navigation scripts subscribe to OnModeChanged to manage EventSystem selection.
public class UIInputMode : MonoBehaviour
{
    public enum Mode { Mouse, Keyboard }

    public static Mode Current { get; private set; } = Mode.Mouse;
    public static event Action<Mode> OnModeChanged;

    const float MouseDeltaThreshold = 2f;

    static UIInputMode instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        if (instance != null) return;
        var go = new GameObject("[UIInputMode]");
        instance = go.AddComponent<UIInputMode>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure cursor starts visible (mouse mode default).
        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        bool mouseActive    = DetectMouse();
        bool keyboardActive = !mouseActive && DetectKeyboard();

        if (mouseActive && Current != Mode.Mouse)
            SetMode(Mode.Mouse);
        else if (keyboardActive && Current != Mode.Keyboard)
            SetMode(Mode.Keyboard);
    }

    static bool DetectMouse()
    {
        if (Mouse.current == null) return false;
        if (Mouse.current.delta.ReadValue().sqrMagnitude > MouseDeltaThreshold * MouseDeltaThreshold) return true;
        if (Mouse.current.leftButton.wasPressedThisFrame)  return true;
        if (Mouse.current.rightButton.wasPressedThisFrame) return true;
        return false;
    }

    static bool DetectKeyboard()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        if (Gamepad.current != null)
        {
            var gp = Gamepad.current;
            if (gp.buttonSouth.wasPressedThisFrame  || gp.buttonNorth.wasPressedThisFrame  ||
                gp.buttonEast.wasPressedThisFrame   || gp.buttonWest.wasPressedThisFrame   ||
                gp.startButton.wasPressedThisFrame  || gp.selectButton.wasPressedThisFrame ||
                gp.leftShoulder.wasPressedThisFrame || gp.rightShoulder.wasPressedThisFrame)
                return true;
            if (gp.leftStick.ReadValue().sqrMagnitude > 0.25f) return true;
            if (gp.dpad.ReadValue().sqrMagnitude      > 0.1f)  return true;
        }

        return false;
    }

    static void SetMode(Mode mode)
    {
        Current          = mode;
        Cursor.visible   = (mode == Mode.Mouse);
        Cursor.lockState = CursorLockMode.None;

        // Navigation scripts clear/restore EventSystem selection via OnModeChanged.
        // We do NOT touch EventSystem here to avoid interfering with pointer clicks.
        OnModeChanged?.Invoke(mode);
    }
}
