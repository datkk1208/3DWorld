using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int _comboCounter;
    private bool _nextAttackQueued; // Biến lưu lệnh bấm (Input Buffering)

    // Config: Hủy động tác thừa khi animation chạy qua 60%
    private float _cancelThreshold = 0.6f;

    public PlayerAttackState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _comboCounter = 0;
        _nextAttackQueued = false;
        _ctx.SetAnimationSpeed(0); // Dừng di chuyển
        PerformAttack(); // Đánh đòn 1 ngay khi vào state
    }

    public override void UpdateState()
    {
        // 1. NHẬN LỆNH BẤM (Input Buffering)
        // Cho phép bấm trước khi đòn đánh kết thúc
        if (_ctx.InputProvider.IsAttackPressed)
        {
            _nextAttackQueued = true;
        }

        AnimatorStateInfo stateInfo = _ctx.Animator.GetCurrentAnimatorStateInfo(0);

        // Chỉ xử lý khi đang ở đúng State Attack (tránh lỗi khi blending)
        if (stateInfo.IsTag("Attack"))
        {
            // --- LOGIC HỦY ĐỘNG TÁC THỪA (Combo liên tục) ---
            // Nếu animation đã diễn qua ngưỡng Cancel (ví dụ chém xong rồi, đang thu kiếm)
            // VÀ người chơi CÓ bấm nút đánh tiếp
            if (stateInfo.normalizedTime >= _cancelThreshold && _nextAttackQueued)
            {
                if (_comboCounter < 3) // Nếu chưa hết chuỗi 3 đòn
                {
                    PerformAttack(); // -> Ngắt animation cũ, đánh đòn mới ngay!
                }
            }

            // --- LOGIC THOÁT VỀ IDLE (Nếu không đánh tiếp) ---
            if (stateInfo.normalizedTime >= 0.7f)
            {
                // 2. Nếu người chơi bấm nút Di Chuyển (WASD)
                if (_ctx.InputProvider.MoveInput.magnitude > 0.1f)
                {
                    SwitchState(_factory.Walk()); // Cho phép ngắt đòn để đi luôn
                }
                // 3. Hoặc nếu hết hẳn animation (như cũ) thì tự về
                else if (stateInfo.normalizedTime >= 0.95f)
                {
                    SwitchState(_factory.Idle());
                }
            }
        }
    }

    public override void ExitState()
    {
        _comboCounter = 0;
        _nextAttackQueued = false;
        // Reset Trigger để tránh lỗi lặp lại không mong muốn
        _ctx.Animator.ResetTrigger("Attack");
    }

    public override void CheckSwitchStates() { }

    private void PerformAttack()
    {
        _comboCounter++;
        _nextAttackQueued = false; // Đã dùng lệnh bấm, reset về false

        // Cập nhật Animator
        _ctx.Animator.SetInteger("ComboCounter", _comboCounter);
        _ctx.Animator.SetTrigger("Attack");
    }
}