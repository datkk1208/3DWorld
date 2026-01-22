using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 15f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private CharacterController _controller;
    private Transform _cameraTransform;
    private Vector3 _velocity;
    private bool _isGrounded;

    public float CurrentSpeed { get; private set; } // Để Animation đọc

    public void Initialize()
    {
        _controller = GetComponent<CharacterController>();
        _cameraTransform = Camera.main.transform;
    }

    public void Tick(Vector2 input)
    {
        HandleGravity();
        HandleMovement(input);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleMovement(Vector2 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            CurrentSpeed = 0;
            return;
        }

        // Tính hướng theo Camera (Logic Genshin Impact)
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 direction = camForward * input.y + camRight * input.x;

        // Xoay nhân vật
        if (direction.sqrMagnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);

            _controller.Move(direction * moveSpeed * Time.deltaTime);
        }

        CurrentSpeed = direction.magnitude;
    }

    private void HandleGravity()
    {
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}