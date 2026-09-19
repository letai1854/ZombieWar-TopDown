using UnityEngine;

public class SoldierIdleState : SoldierState
{
    public SoldierIdleState(Soldier soldier, SoldierStateMachine stateMachine) 
        : base(soldier, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        soldier.Animator.SetFloat(AnimData.SpeedHash, 0f, 0.15f, Time.deltaTime);

        if (soldier.IsShooting)
        {
            stateMachine.ChangeState(soldier.AttackState);
            return;
        }

        if (soldier.Joystick != null)
        {
            Vector3 moveInput = new Vector3(soldier.Joystick.Horizontal, 0f, soldier.Joystick.Vertical);
            
            if (moveInput.magnitude > 0.1f)
            {
                stateMachine.ChangeState(soldier.MoveState);
            }
        }
    }
}