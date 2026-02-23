using UnityEngine;
using UnityEngine.AI;

public enum EnemyClass { Melee, Tank, Ranged }

[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("Cài đặt Quái")]
    public EnemyClass ClassType;
    public float AttackRange = 2f;
    public float AttackCooldown = 2f;
    public float ChaseRange = 15f;

    [Header("Cài đặt Máu & Sát thương")]
    public int MeleeHealth = 50;
    public int MeleeDamage = 15;
    public int TankHealth = 200;
    public int TankDamage = 35;
    public int RangedHealth = 80;
    public int RangedDamage = 20;
    public LayerMask PlayerLayer;

    [Header("Kỹ năng Ranged (Chỉ dành cho quái đánh xa)")]
    public GameObject CastVFXPrefab;
    public GameObject FireballPrefab;
    public Transform FirePoint; // Kéo xương bàn tay của quái vào đây

    private NavMeshAgent _agent;
    private Animator _anim;
    private Transform _playerTarget;
    private float _lastAttackTime;
    private Vector3 _startPosition;
    private int _currentDamage;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        SetupStats();
    }

    private void Start()
    {
        _startPosition = transform.position;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _playerTarget = playerObj.transform;
    }

    private void SetupStats()
    {
        EnemyHealth healthScript = GetComponent<EnemyHealth>();
        switch (ClassType)
        {
            case EnemyClass.Melee:
                _agent.speed = 5f;
                AttackRange = 1.5f;
                _currentDamage = MeleeDamage;
                if (healthScript != null) healthScript.MaxHealth = MeleeHealth;
                break;
            case EnemyClass.Tank:
                _agent.speed = 1.5f;
                AttackRange = 2f;
                _currentDamage = TankDamage;
                if (healthScript != null) healthScript.MaxHealth = TankHealth;
                break;
            case EnemyClass.Ranged:
                _agent.speed = 3f;
                AttackRange = 8f;
                _currentDamage = RangedDamage;
                if (healthScript != null) healthScript.MaxHealth = RangedHealth;
                break;
        }
    }

    private void Update()
    {
        if (_playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.position);

        if (distanceToPlayer <= AttackRange)
        {
            _agent.isStopped = true;
            LookAtPlayer();
            TryAttack();
        }
        else if (distanceToPlayer <= ChaseRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_playerTarget.position);
        }
        else
        {
            if (Vector3.Distance(transform.position, _startPosition) > 0.5f)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_startPosition);
            }
            else
            {
                _agent.isStopped = true;
            }
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        float targetAnimSpeed = 0f;
        if (!_agent.isStopped && _agent.velocity.magnitude > 0.1f)
        {
            targetAnimSpeed = _agent.speed > 3f ? 1f : 0.5f;
        }
        _anim.SetFloat("Speed", targetAnimSpeed, 0.1f, Time.deltaTime);
    }

    private void LookAtPlayer()
    {
        Vector3 dir = (_playerTarget.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
    }

    private void TryAttack()
    {
        if (Time.time - _lastAttackTime >= AttackCooldown)
        {
            _lastAttackTime = Time.time;
            _anim.SetTrigger("Attack"); // Kích hoạt animation đánh (chung cho cả 3 loại)
        }
    }

    // --- HÀM 1: GỌI TỪ ANIMATION EVENT CỦA QUÁI TANK/MELEE ---
    public void DealDamageFromAnimation()
    {
        if (ClassType == EnemyClass.Ranged) return;

        Collider[] hitPlayers = Physics.OverlapSphere(transform.position + transform.forward * 1.0f, AttackRange, PlayerLayer);
        foreach (var p in hitPlayers)
        {
            PlayerController target = p.GetComponent<PlayerController>();
            if (target != null) target.TakeDamage(_currentDamage);
        }
    }

    // --- HÀM 2: GỌI TỪ ANIMATION EVENT CỦA QUÁI RANGED ---
    public void ShootFireballFromAnimation()
    {
        if (ClassType != EnemyClass.Ranged || _playerTarget == null || FirePoint == null) return;

        // Tụ phép
        if (CastVFXPrefab != null)
        {
            GameObject cast = Instantiate(CastVFXPrefab, FirePoint.position, FirePoint.rotation);
            Destroy(cast, 1f);
        }

        // Bắn cầu lửa
        if (FireballPrefab != null)
        {
            Vector3 targetPos = new Vector3(_playerTarget.position.x, _playerTarget.position.y + 1f, _playerTarget.position.z);
            Vector3 dir = (targetPos - FirePoint.position).normalized;

            GameObject fb = Instantiate(FireballPrefab, FirePoint.position, Quaternion.LookRotation(dir));

            Fireball script = fb.GetComponent<Fireball>();
            if (script != null) script.Damage = _currentDamage;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1.0f, AttackRange);
    }
}