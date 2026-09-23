using UnityEngine;

public class ZombieChaseState : ZombieState
{
    public ZombieChaseState(Zombie zombie, ZombieStateMachine stateMachine, string animBoolName) : base(zombie, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();

        if (zombie.Agent != null && zombie.Agent.isActiveAndEnabled)
        {
            zombie.Agent.isStopped = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (zombie.TargetPlayer == null) return;

        float distance = zombie.GetDistanceToPlayer();
        Debug.Log($"[Zombie] Distance to Player: {distance}");

        bool isCloseEnough = distance <= zombie.attackRange;

        // Thêm kiểm tra bằng Raycast theo đúng yêu cầu của bạn để chống lỗi khoảng cách
        if (!isCloseEnough)
        {
            Vector3 rayStart = zombie.transform.position + Vector3.up * 1f; // Bắn từ ngang ngực
            Vector3 dirToPlayer = (zombie.TargetPlayer.position - zombie.transform.position).normalized;
            if (Physics.Raycast(rayStart, dirToPlayer, out RaycastHit hit, zombie.attackRange))
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
        else if (distance > zombie.detectionRadius * 1.5f) // Thoát khỏi tầm nhìn
        {
            stateMachine.ChangeState(zombie.IdleState);
        }
        else
        {
            // Cập nhật vị trí NavMeshAgent
            if (zombie.Agent.isActiveAndEnabled)
            {
                zombie.Agent.SetDestination(zombie.TargetPlayer.position);
            }
        }
    }
}
