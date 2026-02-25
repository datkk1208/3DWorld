using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    public Slider HealthBar;
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

    // Biến khóa nhân vật khi bị choáng
    public bool IsStunned = false;

    // References
    public CharacterController CharacterController { get; private set; }
    public Animator Animator { get; private set; }
    public IInputProvider InputProvider { get; private set; }
    private Transform _mainCamera;

    // State Machine
    public PlayerBaseState CurrentState { get; set; }
    private PlayerStateFactory _states;

    // Internal Variables
    private int _animIDSpeed;

    private void Start()
    {
        _currentHealth = MaxHealth;
        if (HealthBar != null) HealthBar.value = 1f;
        Debug.Log($"[Player] Bắt đầu với {_currentHealth} máu.");
    }

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");

        if (Camera.main != null) _mainCamera = Camera.main.transform;

        InputProvider = GetComponent<IInputProvider>();
        if (InputProvider == null) InputProvider = gameObject.AddComponent<InputSystemProvider>();

        _states = new PlayerStateFactory(this);
        CurrentState = _states.Idle();
        CurrentState.EnterState();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // 1. NẾU ĐANG BỊ CHOÁNG -> KHÔNG CHẠY STATE MACHINE
        if (IsStunned)
        {
            ApplyGravity();
            return;
        }

        // 2. NẾU BÌNH THƯỜNG -> HOẠT ĐỘNG NHƯ CŨ
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

            if (_mainCamera != null) targetAngle += _mainCamera.eulerAngles.y;

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

    public bool IsGrounded()
    {
        return CharacterController.isGrounded || Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.4f);
    }

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
                Debug.Log($"[Player] Chém trúng quái! Gây {damage} sát thương.");
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
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;
        if (HealthBar != null) HealthBar.value = (float)_currentHealth / MaxHealth;
        Debug.Log($"[Player] Bị đánh trúng! Mất {damage} máu. Máu còn lại: {_currentHealth}");

        GetComponent<PlayerAudioManager>().PlayHitSound();

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Animator.SetTrigger("Hit");

            Debug.Log($"[Player] Đang ép State Machine từ {CurrentState} về Idle và reset Combo.");
            CurrentState = _states.Idle();
            CurrentState.EnterState();
            Animator.SetInteger("ComboCounter", 0);

            StartCoroutine(StunRoutine());
        }
    }

    private void Die()
    {
        Debug.Log("[Player] ĐÃ CHẾT!");
        GetComponent<PlayerAudioManager>().PlayDeathSound();

        Animator.SetTrigger("Die");
        CharacterController.enabled = false;
        this.enabled = false;
        Destroy(gameObject, 3f);
    }

    private System.Collections.IEnumerator StunRoutine()
    {
        Debug.Log("[Player] Bắt đầu dính Stun (khóa di chuyển/tấn công).");
        IsStunned = true;

        Velocity = new Vector3(0, Velocity.y, 0);

        yield return new WaitForSeconds(0.5f);

        IsStunned = false;
        Debug.Log("[Player] Hết Stun, có thể điều khiển lại.");
    }
}