using UnityEngine;

public interface IInputProvider
{
    Vector2 MoveInput { get; }
    bool IsRunning { get; }
    bool IsAttackPressed { get; } // Bắt buộc có để nhận lệnh đánh
}