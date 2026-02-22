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
    public bool IsAttackPressed => _inputActions.Player.Attack.WasPressedThisFrame();

    //Thêm dòng này.Đảm bảo bạn đã tạo Action "Jump" trong file Input Asset!
    public bool IsJumpPressed => _inputActions.Player.Jump.WasPressedThisFrame();
}