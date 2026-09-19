public class SoldierStateMachine
{
    public SoldierState CurrentState { get; private set; }

    public void Initialize(SoldierState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(SoldierState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}