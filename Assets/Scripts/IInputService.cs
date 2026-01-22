using UnityEngine;

public interface IInputService
{
    Vector2 GetMoveInput();
    bool IsJumpPressed();
    bool IsAttackPressed(); // <--- Thêm dòng này
}

public class UnityInputService : IInputService
{
    public Vector2 GetMoveInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        return new Vector2(x, z);
    }

    public bool IsJumpPressed() => Input.GetButtonDown("Jump");

    // Mặc định Fire1 là Chuột trái hoặc Chạm màn hình
    public bool IsAttackPressed() => Input.GetButtonDown("Fire1");
}