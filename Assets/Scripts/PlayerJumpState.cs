using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Jump(); // Thực hiện lực nhảy
    }

    public override void UpdateState()
    {
        // Cho phép di chuyển nhẹ trên không (tùy game, có thể bỏ dòng này)
        _ctx.Move(_ctx.WalkSpeed);

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // Reset animator hoặc logic nếu cần
    }

    public override void CheckSwitchStates()
    {
        // Khi rơi xuống và chạm đất -> Về Idle hoặc Walk
        if (_ctx.CharacterController.isGrounded && _ctx.Velocity.y <= 0)
        {
            if (_ctx.InputProvider.MoveInput.magnitude > 0)
            {
                SwitchState(_factory.Walk());
            }
            else
            {
                SwitchState(_factory.Idle());
            }
        }
    }
}