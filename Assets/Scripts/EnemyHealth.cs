using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
public class EnemyHealth : MonoBehaviour
{
    // 2. Thêm biến vào phần khai báo:
    [Header("UI")]
    public Slider HealthBar;
    public GameObject DamageTextPrefab;
    [Header("Stats")]
    [HideInInspector]
    public int MaxHealth = 100;
    private int _currentHealth;

    private Animator _anim;
    private EnemyAI _aiScript;
    private NavMeshAgent _agent;
    private Collider _collider;

    private void Start()
    {
        if (HealthBar != null) HealthBar.value = 1f;
        _currentHealth = MaxHealth;
        _anim = GetComponent<Animator>();
        _aiScript = GetComponent<EnemyAI>();
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
    }

    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;
        if (HealthBar != null) HealthBar.value = (float)_currentHealth / MaxHealth;
        if (DamageTextPrefab != null)
        {
            // Tạo text số dame nảy lên trên đầu quái 1.5m
            GameObject popup = Instantiate(DamageTextPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            popup.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damage;
        }
        StartCoroutine(FlashColor());

        if (_currentHealth <= 0)
        {
            Die(); // Hết máu thì chỉ gọi Die
        }
        else
        {
            _anim.SetTrigger("Hit"); // Còn sống mới gọi Hit
        }
    }

    private void Die()
    {
        _anim.SetTrigger("Die"); // Gọi animation chết

        // Tắt AI, tắt NavMesh, tắt va chạm để quái nằm im
        if (_aiScript != null) _aiScript.enabled = false;
        if (_agent != null) _agent.enabled = false;
        if (_collider != null) _collider.enabled = false;

        // Xóa quái khỏi game sau 3 giây (Đợi chạy xong Animation)
        Destroy(gameObject, 3f);
    }

    private System.Collections.IEnumerator FlashColor()
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (renderers.Length > 0)
        {
            foreach (var r in renderers) r.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            foreach (var r in renderers) r.material.color = Color.white;
        }
    }
}