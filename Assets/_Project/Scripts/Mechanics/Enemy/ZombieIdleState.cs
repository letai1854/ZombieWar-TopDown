using UnityEngine;

public class ZombieIdleState : ZombieState
{
    public ZombieIdleState(Zombie zombie, ZombieStateMachine stateMachine, string animBoolName) : base(zombie, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        if (zombie.Agent != null && zombie.Agent.isActiveAndEnabled)
        {
            zombie.Agent.isStopped = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (zombie.TargetPlayer != null)
        {
            float distance = zombie.GetDistanceToPlayer();
            if (distance <= zombie.detectionRadius)
            {
                stateMachine.ChangeState(zombie.ChaseState);
            }
        }
    }
}
