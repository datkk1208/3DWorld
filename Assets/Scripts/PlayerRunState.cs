using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState() { }

    public override void UpdateState()
    {
        _ctx.Move(_ctx.RunSpeed);
        _ctx.SetAnimationSpeed(1.0f);
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        // ƯU TIÊN TẤN CÔNG
        if (_ctx.InputProvider.IsAttackPressed)
        {
            SwitchState(_factory.Attack());
            return;
        }

        if (_ctx.InputProvider.MoveInput.magnitude == 0)
        {
            SwitchState(_factory.Idle());
        }
        else if (!_ctx.InputProvider.IsRunning)
        {
            SwitchState(_factory.Walk());
        }
    }
}