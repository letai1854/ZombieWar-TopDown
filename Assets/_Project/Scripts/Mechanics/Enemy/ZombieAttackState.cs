using UnityEngine;

public class ZombieAttackState : ZombieState
{
    public ZombieAttackState(Zombie zombie, ZombieStateMachine stateMachine, string animBoolName) : base(zombie, stateMachine, animBoolName) { }

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

        if (zombie.TargetPlayer == null) return;

        Vector3 dirToPlayer = (zombie.TargetPlayer.position - zombie.transform.position).normalized;
        dirToPlayer.y = 0;
        if (dirToPlayer != Vector3.zero)
        {
            zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, Quaternion.LookRotation(dirToPlayer), Time.deltaTime * 5f);
        }

        float distance = zombie.GetDistanceToPlayer();

        if (distance > zombie.ActualAttackRange)
        {
            stateMachine.ChangeState(zombie.ChaseState);
        }
    }
}
