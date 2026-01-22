using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private IInputService _inputService;
    private PlayerMotor _motor;
    private PlayerCombat _combat;
    private Animator _animator;

    private void Awake()
    {
        _inputService = new UnityInputService();
        _motor = GetComponent<PlayerMotor>();
        _combat = GetComponent<PlayerCombat>();
        _animator = GetComponent<Animator>();

        _motor.Initialize();
        _combat.Initialize();
    }

    private void Update()
    {
        // 1. Kiểm tra trạng thái
        bool isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.3f, LayerMask.GetMask("Ground"));
        bool isAttacking = _combat.IsAttacking;

        // 2. Xử lý Di chuyển (Tách riêng ra)
        if (isAttacking)
        {
            // Nếu đang đánh -> Ép đứng yên
            _motor.Tick(Vector2.zero);
        }
        else
        {
            // Nếu KHÔNG đánh -> Mới cho phép di chuyển và nhảy
            Vector2 moveInput = _inputService.GetMoveInput();
            _motor.Tick(moveInput);

            if (_inputService.IsJumpPressed())
            {
                _motor.Jump();
            }
        }

        // 3. Xử lý Tấn công (Luôn luôn lắng nghe, kể cả lúc đang chém)
        // Bỏ cái return đi thì nó mới chạy xuống đây được
        if (_inputService.IsAttackPressed())
        {
            if (isGrounded)
            {
                _combat.HandleAttackInput();
            }
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (_animator != null)
        {
            _animator.SetFloat("Speed", _motor.CurrentSpeed);
        }
    }
}