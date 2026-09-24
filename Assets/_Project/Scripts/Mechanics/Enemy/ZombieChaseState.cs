using UnityEngine;

public class ZombieChaseState : ZombieState
{
    private float pathUpdateTimer;
    private const float PATH_UPDATE_INTERVAL = 0.2f; 

    public ZombieChaseState(Zombie zombie, ZombieStateMachine stateMachine, string animBoolName) : base(zombie, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();

        if (zombie.Agent != null && zombie.Agent.isActiveAndEnabled && zombie.Agent.isOnNavMesh)
        {
            zombie.Agent.isStopped = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (zombie.TargetPlayer == null) return;

        float distance = zombie.GetDistanceToPlayer();

        bool isCloseEnough = distance <= zombie.ActualAttackRange;

        if (!isCloseEnough)
        {
            float chestHeight = 1f * zombie.transform.localScale.y;
            Vector3 rayStart = zombie.transform.position + Vector3.up * chestHeight; 
            Vector3 dirToPlayer = (zombie.TargetPlayer.position - zombie.transform.position).normalized;
            
            if (Physics.Raycast(rayStart, dirToPlayer, out RaycastHit hit, zombie.ActualAttackRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    isCloseEnough = true;
                }
            }
        }

        if (isCloseEnough)
        {
            stateMachine.ChangeState(zombie.AttackState);
        }
        else if (distance > zombie.detectionRadius * 1.5f) 
        {
            stateMachine.ChangeState(zombie.IdleState);
        }
        else
        {
            pathUpdateTimer -= Time.deltaTime;
            if (pathUpdateTimer <= 0f)
            {
                pathUpdateTimer = PATH_UPDATE_INTERVAL;
                if (zombie.Agent.isActiveAndEnabled && zombie.Agent.isOnNavMesh)
                {
                    zombie.Agent.SetDestination(zombie.TargetPlayer.position);
                }
            }
        }
    }
}
