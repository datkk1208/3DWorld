using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 1f); // Tự xóa sau 1 giây
    }

    private void Update()
    {
        transform.position += Vector3.up * 2f * Time.deltaTime; // Trôi từ từ lên trên
        transform.rotation = Camera.main.transform.rotation; // Luôn nhìn thẳng vào màn hình
    }
}