using UnityEngine;

public interface IInputService
{
    Vector2 GetMoveInput();
    bool IsJumpPressed();
    bool IsAttackPressed();
    void Cleanup(); // Nhớ dòng này để gọi được hàm dọn dẹp
}