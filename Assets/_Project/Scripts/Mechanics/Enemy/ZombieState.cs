public abstract class ZombieState
{
    protected Zombie zombie;
    protected ZombieStateMachine stateMachine;
    protected string animBoolName;

    public ZombieState(Zombie zombie, ZombieStateMachine stateMachine, string animBoolName)
    {
        this.zombie = zombie;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        if (zombie.Animator != null && !string.IsNullOrEmpty(animBoolName))
        {
            zombie.Animator.SetBool(animBoolName, true);
        }
    }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void Exit()
    {
        if (zombie.Animator != null && !string.IsNullOrEmpty(animBoolName))
        {
            zombie.Animator.SetBool(animBoolName, false);
        }
    }
}
