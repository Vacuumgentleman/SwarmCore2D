using UnityEngine;
using UnityEngine.InputSystem;
using SwarmCore2D.Core;
using SwarmCore2D.Simulation;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;

    [Header("Referencias")]
    public SpriteRenderer spriteRenderer;

    Vector2 input;
    Vector3 velocity;

    SwarmSimulationController sim;

    void Start()
    {
        sim = FindFirstObjectByType<SwarmSimulationController>();
    }

    void Update()
    {
        if (sim != null && sim.IsPaused())
            return;

        ReadInput();
        UpdateVisual();
    }

    void FixedUpdate()
    {
        if (sim != null && sim.IsPaused())
            return;

        Move();
    }

    void ReadInput()
    {
        input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    input.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  input.y -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  input.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1;
        }

        if (Gamepad.current != null)
        {
            var stick = Gamepad.current.leftStick.ReadValue();
            if (stick.sqrMagnitude > 0.1f)
                input += stick;

            if (Gamepad.current.dpad.up.isPressed)    input.y += 1;
            if (Gamepad.current.dpad.down.isPressed)  input.y -= 1;
            if (Gamepad.current.dpad.left.isPressed)  input.x -= 1;
            if (Gamepad.current.dpad.right.isPressed) input.x += 1;
        }

        input = Vector2.ClampMagnitude(input, 1f);
    }

    void Move()
    {
        velocity.x = input.x * moveSpeed;
        velocity.y = input.y * moveSpeed;

        transform.position += velocity * SwarmTime.FixedDelta;
    }

    void UpdateVisual()
    {
        if (spriteRenderer == null)
            return;

        if (input.x < -0.01f)
            spriteRenderer.flipX = true;
        else if (input.x > 0.01f)
            spriteRenderer.flipX = false;
    }
}