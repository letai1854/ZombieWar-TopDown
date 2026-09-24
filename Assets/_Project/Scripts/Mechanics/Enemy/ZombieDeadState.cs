using UnityEngine;

public class ZombieDeadState : ZombieState
{
    private float deadTimer;

    public ZombieDeadState(Zombie zombie, ZombieStateMachine stateMachine, string animBoolName) : base(zombie, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        
        if (zombie.Agent != null && zombie.Agent.isActiveAndEnabled)
        {
            zombie.Agent.isStopped = true;
            zombie.Agent.enabled = false;
        }

        zombie.gameObject.layer = LayerMask.NameToLayer("Default");

        Collider[] cols = zombie.GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            c.enabled = false;
        }

        deadTimer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        deadTimer += Time.deltaTime;

        if (deadTimer >= 3f)
        {
            zombie.gameObject.SetActive(false);
        }
    }
}
