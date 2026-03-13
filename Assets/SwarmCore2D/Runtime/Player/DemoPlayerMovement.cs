using UnityEngine;
using UnityEngine.InputSystem;

public class DemoPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;

    Vector2 moveInput;

    void Update()
    {
        ReadInput();
        Move();
    }

    void ReadInput()
    {
        if (Keyboard.current == null) return;

        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

        moveInput = moveInput.normalized;
    }

    void Move()
    {
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0);

        transform.position += move * moveSpeed * Time.deltaTime;
    }
}