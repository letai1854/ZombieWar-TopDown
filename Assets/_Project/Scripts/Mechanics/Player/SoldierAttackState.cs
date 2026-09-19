using UnityEngine;

public class SoldierAttackState : SoldierState
{
    public SoldierAttackState(Soldier soldier, SoldierStateMachine stateMachine) 
        : base(soldier, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        soldier.Animator.SetBool(AnimData.IsShootingHash, true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (soldier.Joystick == null) return;

        Vector3 moveInput = new Vector3(soldier.Joystick.Horizontal, 0f, soldier.Joystick.Vertical);
        float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);

        soldier.Animator.SetFloat(AnimData.SpeedHash, inputMagnitude, 0.1f, Time.deltaTime);

        if (inputMagnitude >= 0.1f)
        {
            Vector3 moveDirection = moveInput.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            soldier.transform.rotation = Quaternion.Slerp(soldier.transform.rotation, targetRotation, soldier.RotateSpeed * Time.deltaTime);
        }

        if (!soldier.IsShooting)
        {
            if (inputMagnitude >= 0.1f)
            {
                stateMachine.ChangeState(soldier.MoveState);
            }
            else
            {
                stateMachine.ChangeState(soldier.IdleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector3 moveInput = new Vector3(soldier.Joystick.Horizontal, 0f, soldier.Joystick.Vertical);
        float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);

        if (inputMagnitude >= 0.1f)
        {
            Vector3 moveDirection = moveInput.normalized;
            soldier.Controller.Move(moveDirection * soldier.MoveSpeed * Time.fixedDeltaTime);
        }
    }

    public override void Exit()
    {
        base.Exit();
        soldier.Animator.SetBool(AnimData.IsShootingHash, false);
    }
}