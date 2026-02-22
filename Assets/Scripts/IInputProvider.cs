using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputProvider
{
    Vector2 MoveInput { get; }
    bool IsRunning { get; }
    bool IsAttackPressed { get; }
    bool IsJumpPressed { get; }
}