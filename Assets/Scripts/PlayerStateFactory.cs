public class PlayerStateFactory
{
    private PlayerController _context;

    public PlayerStateFactory(PlayerController currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle() => new PlayerIdleState(_context, this);
    public PlayerBaseState Walk() => new PlayerWalkState(_context, this);
    public PlayerBaseState Run() => new PlayerRunState(_context, this);
    // Đăng ký trạng thái tấn công
    public PlayerBaseState Attack() => new PlayerAttackState(_context, this);
}