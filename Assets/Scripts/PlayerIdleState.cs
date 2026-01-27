using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState() { }

    public override void UpdateState()
    {
        _ctx.SetAnimationSpeed(0f);
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        // 1. ƯU TIÊN KIỂM TRA TẤN CÔNG TRƯỚC
        if (_ctx.InputProvider.IsAttackPressed)
        {
            SwitchState(_factory.Attack());
            return; // Quan trọng: return ngay để không chạy logic di chuyển
        }

        // 2. Logic di chuyển
        if (_ctx.InputProvider.MoveInput.magnitude > 0)
        {
            SwitchState(_factory.Walk());
        }
    }
}