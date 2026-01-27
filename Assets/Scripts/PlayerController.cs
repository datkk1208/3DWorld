using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float WalkSpeed = 5.0f;
    public float RunSpeed = 10.0f;
    public float RotationSpeed = 12.0f;
    public float Gravity = -9.81f;

    [Header("Combat Settings")]
    // Index 0 không dùng. Index 1, 2, 3 tương ứng Attack 1, 2, 3
    public int[] AttackDamages = { 0, 30, 20, 45 };
    public float AttackRange = 1.5f;
    public LayerMask EnemyLayer; // Nhớ tạo Layer "Enemy" và gán cho quái

    // References
    public CharacterController CharacterController { get; private set; }
    public Animator Animator { get; private set; }
    public IInputProvider InputProvider { get; private set; }

    // State Machine
    public PlayerBaseState CurrentState { get; set; }
    private PlayerStateFactory _states;

    // Internal Variables
    private Vector3 _velocity;
    private int _animIDSpeed;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");

        InputProvider = GetComponent<IInputProvider>();
        if (InputProvider == null) InputProvider = gameObject.AddComponent<InputSystemProvider>();

        _states = new PlayerStateFactory(this);
        CurrentState = _states.Idle();
        CurrentState.EnterState();
        // KHÓA CHUỘT VÀO GIỮA MÀN HÌNH + ẨN ĐI
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CurrentState.UpdateState();
        ApplyGravity();
    }

    public void Move(float speed)
    {
        Vector2 input = InputProvider.MoveInput;
        Vector3 direction = new Vector3(input.x, 0, input.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            // Lưu ý: Nếu có Camera, hãy cộng thêm góc Camera vào đây
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, RotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            CharacterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    public void SetAnimationSpeed(float targetSpeed)
    {
        Animator.SetFloat(_animIDSpeed, targetSpeed, 0.1f, Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (CharacterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        _velocity.y += Gravity * Time.deltaTime;
        CharacterController.Move(_velocity * Time.deltaTime);
    }

    // --- HÀM GÂY SÁT THƯƠNG (GỌI TỪ ANIMATION EVENT) ---
    public void DealDamageFromAnimation()
    {
        int currentCombo = Animator.GetInteger("ComboCounter");
        if (currentCombo < 1 || currentCombo >= AttackDamages.Length) return;

        int damage = AttackDamages[currentCombo];

        // Tạo vùng va chạm
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * 0.5f, AttackRange, EnemyLayer);

        foreach (var enemy in hitEnemies)
        {
            // --- ĐOẠN CODE MỚI CẬP NHẬT ---
            // Tìm xem đối tượng bị đánh có script EnemyHealth không
            EnemyHealth target = enemy.GetComponent<EnemyHealth>();

            if (target != null)
            {
                // Nếu có thì gọi hàm trừ máu
                target.TakeDamage(damage);
            }
            // -------------------------------
        }
    }

    // Vẽ vùng đánh để dễ chỉnh (Gizmos)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 0.5f, AttackRange);
    }
}