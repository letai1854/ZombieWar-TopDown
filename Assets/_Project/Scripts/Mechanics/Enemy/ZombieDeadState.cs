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

        // Đổi Layer sang Default để AutoShooter hoàn toàn ngó lơ nó
        zombie.gameObject.layer = LayerMask.NameToLayer("Default");

        // Tắt TOÀN BỘ Collider (kể cả các Collider phụ trên xương gối, đầu...)
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

        // Đợi Animation chết và Dissolve Shader chạy xong (khoảng 3 giây)
        if (deadTimer >= 3f)
        {
            // TODO: Trả Zombie về ObjectPool thay vì Destroy
            zombie.gameObject.SetActive(false);
        }
    }
}
