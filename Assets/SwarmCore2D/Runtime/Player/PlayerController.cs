using UnityEngine;
using UnityEngine.InputSystem;
using SwarmCore2D.Core;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;

    public enum RotationMode
    {
        Flip,
        FourDirections
    }

    [Header("Rotation")]
    public RotationMode rotationMode = RotationMode.Flip;

    Vector2 input;
    Vector3 velocity;

    void Update()
    {
        ReadInput();
        UpdateRotation();
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

    void UpdateRotation()
    {
        if (input == Vector2.zero)
            return;

        if (rotationMode == RotationMode.Flip)
        {
            if (input.x < -0.01f)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (input.x > 0.01f)
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (rotationMode == RotationMode.FourDirections)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > 0)
                    transform.rotation = Quaternion.Euler(0, 0, -90);     // derecha
                else
                    transform.rotation = Quaternion.Euler(0, 0, 90);   // izquierda
            }
            else
            {
                if (input.y > 0)
                    transform.rotation = Quaternion.Euler(0, 0, 0);    // arriba
                else
                    transform.rotation = Quaternion.Euler(0, 0, 180);   // abajo
            }
        }
    }
}