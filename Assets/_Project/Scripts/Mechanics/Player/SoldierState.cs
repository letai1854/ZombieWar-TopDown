public abstract class SoldierState
{
    protected Soldier soldier;
    protected SoldierStateMachine stateMachine;

    public SoldierState(Soldier soldier, SoldierStateMachine stateMachine)
    {
        this.soldier = soldier;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }
}