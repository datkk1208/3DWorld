using UnityEngine;

public class UnityInputService : IInputService
{
    // Class MyControl do Unity tự sinh ra (không có s)
    private MyControl _controls;

    public UnityInputService()
    {
        _controls = new MyControl();
        _controls.Enable(); // Bật Input lên
    }

    // Hàm dọn dẹp (Quan trọng cho Android)
    public void Cleanup()
    {
        _controls.Disable();
    }

    public Vector2 GetMoveInput()
    {
        // Đọc từ New Input System
        return _controls.Player.Move.ReadValue<Vector2>();
    }

    public bool IsJumpPressed()
    {
        return _controls.Player.Jump.WasPressedThisFrame();
    }

    public bool IsAttackPressed()
    {
        return _controls.Player.Attack.WasPressedThisFrame();
    }
}