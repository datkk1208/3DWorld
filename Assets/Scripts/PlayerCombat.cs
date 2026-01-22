using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float resetComboTime = 1f; // Thời gian reset về đòn 1 (1 giây)

    private Animator _animator;
    private int _currentCombo = 0;
    private float _lastAttackTime = -99f;
    // Thuộc tính để bên ngoài đọc (Chỉ đọc)
    public bool IsAttacking { get; private set; }
    private void Update()
    {
        // Kiểm tra xem Animator có đang chạy state nào có Tag là "Attack" không
        // Layer 0 là layer mặc định (Base Layer)
        if (_animator != null)
        {
            IsAttacking = _animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
        }
    }
    public void Initialize()
    {
        _animator = GetComponent<Animator>();
    }

    public void HandleAttackInput()
    {
        // Tính thời gian từ cú đánh trước đến giờ
        float timeSinceLastAttack = Time.time - _lastAttackTime;

        // --- ĐOẠN LOGIC MỚI ---
        // Nếu đang ở đòn 3 (Last Hit) mà bấm quá nhanh (chưa hết thời gian reset)
        // -> Thì BỎ QUA luôn cú click đó (Không cho quay lại đòn 1 ngay)
        if (_currentCombo == 3 && timeSinceLastAttack < resetComboTime)
        {
            return; // Thoát hàm ngay, không làm gì cả
        }
        // ---------------------

        // Nếu đã quá thời gian chờ -> Reset combo về 0 để bắt đầu chuỗi mới
        if (timeSinceLastAttack > resetComboTime)
        {
            _currentCombo = 0;
        }

        // Tăng combo
        _currentCombo++;

        // (Dự phòng) Nếu vẫn vượt quá 3 thì về 1
        if (_currentCombo > 3)
        {
            _currentCombo = 1;
        }

        _lastAttackTime = Time.time;
        PlayAttackAnimation();
    }
    private void PlayAttackAnimation()
    {
        if (_animator != null)
        {
            // Reset trigger cũ để tránh lỗi double click
            _animator.ResetTrigger("Attack");

            // Gửi số combo hiện tại vào Animator
            _animator.SetInteger("ComboIndex", _currentCombo);

            // Kích hoạt đánh
            _animator.SetTrigger("Attack");
        }
    }
}