using UnityEngine;
using UnityEngine.InputSystem;
using SwarmCore2D.Core;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;

    [Header("Referencias")]
    public SpriteRenderer spriteRenderer;

    Vector2 input;
    Vector3 velocity;

    void Update()
    {
        ReadInput();
        UpdateVisual();
    }

    void FixedUpdate()
    {
        Move();
    }

    void ReadInput()
    {
        if (Keyboard.current == null)
            return;

        input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        input = input.normalized;
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
        {
            spriteRenderer.flipX = true;   
        }
        else if (input.x > 0.01f)
        {
            spriteRenderer.flipX = false;  
        }
    }
}