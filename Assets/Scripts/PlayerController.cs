using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Health")]
    public int MaxHealth = 100;
    private int _currentHealth;
    [Header("Movement Settings")]
    public float WalkSpeed = 5.0f;
    public float RunSpeed = 10.0f;
    public float RotationSpeed = 12.0f;
    public float Gravity = -9.81f;

    [Header("Jump Settings")]
    public float JumpHeight = 1.2f;
    public Vector3 Velocity;

    [Header("Combat Settings")]
    public int[] AttackDamages = { 0, 30, 20, 45 };
    public float AttackRange = 1.5f;
    public LayerMask EnemyLayer;

    // References
    public CharacterController CharacterController { get; private set; }
    public Animator Animator { get; private set; }
    public IInputProvider InputProvider { get; private set; }
    private Transform _mainCamera; // Thêm biến lưu Camera

    // State Machine
    public PlayerBaseState CurrentState { get; set; }
    private PlayerStateFactory _states;

    // Internal Variables
    private int _animIDSpeed;
    private void Start() // Nếu có hàm Start rồi thì chỉ thêm dòng dưới vào Start
    {
        _currentHealth = MaxHealth;
    }
    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");

        // Cache lại Main Camera để tối ưu
        if (Camera.main != null) _mainCamera = Camera.main.transform;

        InputProvider = GetComponent<IInputProvider>();
        if (InputProvider == null) InputProvider = gameObject.AddComponent<InputSystemProvider>();

        _states = new PlayerStateFactory(this);
        CurrentState = _states.Idle();
        CurrentState.EnterState();

        // Khóa chuột
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
            // Tính góc xoay dựa trên input
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // CỘNG THÊM GÓC CAMERA (Để đi chuẩn theo góc nhìn Genshin)
            if (_mainCamera != null) targetAngle += _mainCamera.eulerAngles.y;

            // Xoay nhân vật mượt mà
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, RotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Di chuyển
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
        // Thay CharacterController.isGrounded bằng hàm IsGrounded() mới
        if (IsGrounded() && Velocity.y < 0)
        {
            Velocity.y = -2f;
        }

        Velocity.y += Gravity * Time.deltaTime;
        CharacterController.Move(Velocity * Time.deltaTime);
    }

    public void Jump()
    {
        Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        Animator.SetTrigger("Jump");
    }

    // --- HÀM KIỂM TRA CHẠM ĐẤT (SỬA LỖI TERRAIN) ---
    public bool IsGrounded()
    {
        // Quét tia laser xuống đất 0.4m để đảm bảo nhảy được trên địa hình gồ ghề
        return CharacterController.isGrounded || Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.4f);
    }

    // --- HÀM GÂY SÁT THƯƠNG ---
    public void DealDamageFromAnimation()
    {
        int currentCombo = Animator.GetInteger("ComboCounter");
        if (currentCombo < 1 || currentCombo >= AttackDamages.Length) return;

        int damage = AttackDamages[currentCombo];
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * 0.5f, AttackRange, EnemyLayer);

        foreach (var enemy in hitEnemies)
        {
            EnemyHealth target = enemy.GetComponent<EnemyHealth>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 0.5f, AttackRange);
    }
    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return; // Đã chết thì thôi, không nhận dame nữa

        _currentHealth -= damage;
        Debug.Log($"Player bị đánh trúng! Mất {damage} máu. Còn lại: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Còn sống thì gọi animation giật mình
            Animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        Debug.Log("Player đã chết!");

        // Gọi animation chết
        Animator.SetTrigger("Die");

        // Tắt CharacterController và Script này đi để Player nằm im, không di chuyển hay chém được nữa
        CharacterController.enabled = false;
        this.enabled = false;

        // Tự động xóa object Player sau 3 giây (để kịp xem hết animation ngã xuống)
        Destroy(gameObject, 3f);
    }
}