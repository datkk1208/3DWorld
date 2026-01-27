using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public int MaxHealth = 100;
    private int _currentHealth;

    private void Start()
    {
        _currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"<color=yellow>{gameObject.name}</color> bị chém! Mất: <color=red>{damage}</color> HP. Còn lại: {_currentHealth}");

        // Hiệu ứng nhấp nháy đỏ khi bị đánh (Optional)
        StartCoroutine(FlashColor());

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"<color=red>{gameObject.name} ĐÃ BỊ HẠ GỤC!</color>");
        Destroy(gameObject); // Xóa quái khỏi game
    }

    // Làm quái nháy đỏ chút cho đẹp
    private System.Collections.IEnumerator FlashColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            
            Color oldColor = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            renderer.material.color = oldColor;
        }
    }
}