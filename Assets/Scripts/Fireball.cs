using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float Speed = 15f;
    public int Damage = 20;
    public GameObject HitVFX; // BẮT BUỘC: Kéo Prefab FireballHit vào đây trên Inspector

    private void Start()
    {
        // Thay vì biến mất im lặng, sau 3 giây quả cầu sẽ gọi hàm Nổ (Explode)
        Invoke("Explode", 3f);
    }

    private void Update()
    {
        // Bay thẳng về phía trước
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Bỏ qua nếu chạm vào chính con quái ném ra
        if (other.CompareTag("Enemy")) return;

        // Nếu chạm Player thì trừ máu
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) player.TakeDamage(Damage);
        }

        // Chạm vào Player, hay chạm vào Đất (Terrain), Tường... đều sẽ phát nổ
        Explode();
    }

    // Hàm xử lý Nổ
    private void Explode()
    {
        // Sinh ra hiệu ứng Hit (Nổ)
        if (HitVFX != null)
        {
            GameObject hit = Instantiate(HitVFX, transform.position, Quaternion.identity);
            Destroy(hit, 1.5f); // Xóa hiệu ứng Hit sau 1.5s (hoặc chỉnh theo độ dài particle của bạn)
        }

        // Xóa quả cầu lửa
        Destroy(gameObject);
    }
}