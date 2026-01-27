using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemProvider : MonoBehaviour, IInputProvider
{
    private PlayerControls _inputActions;

    private void Awake() => _inputActions = new PlayerControls();
    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    public Vector2 MoveInput => _inputActions.Player.Move.ReadValue<Vector2>();

    public bool IsRunning => _inputActions.Player.Run.IsPressed();

    // Dùng WasPressedThisFrame để bắt sự kiện click 1 lần (tránh bị spam liên tục)
    public bool IsAttackPressed => _inputActions.Player.Attack.WasPressedThisFrame();
}