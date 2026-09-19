using UnityEngine;

public class SoldierMoveState : SoldierState
{
    private Vector3 moveDirection;
    private float inputMagnitude;

    public SoldierMoveState(Soldier soldier, SoldierStateMachine stateMachine) 
        : base(soldier, stateMachine) { }

public override void LogicUpdate()
{
    base.LogicUpdate();
    
    if (soldier.IsShooting)
    {
        stateMachine.ChangeState(soldier.AttackState);
        return;
    }

    if (soldier.Joystick == null) return;

    Vector3 moveInput = new Vector3(soldier.Joystick.Horizontal, 0f, soldier.Joystick.Vertical);
    inputMagnitude = Mathf.Clamp01(moveInput.magnitude);

    if (inputMagnitude < 0.1f)
    {
        stateMachine.ChangeState(soldier.IdleState);
        return;
    }

    moveDirection = moveInput.normalized;
    moveDirection.y = 0f;

    if (moveDirection != Vector3.zero)
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        soldier.transform.rotation = Quaternion.Slerp(soldier.transform.rotation, targetRotation, soldier.RotateSpeed * Time.deltaTime);
    }

    // Thực hiện di chuyển trực tiếp trong LogicUpdate bằng Time.deltaTime
    soldier.Controller.Move(moveDirection * soldier.MoveSpeed * inputMagnitude * Time.deltaTime);

    soldier.Animator.SetFloat(AnimData.SpeedHash, inputMagnitude, 0.15f, Time.deltaTime);
}

public override void PhysicsUpdate()
{
    base.PhysicsUpdate();
    // Để trống hoặc bỏ hàm Controller.Move ở đây đi
}

    public override void Exit()
    {
        base.Exit();
        // soldier.Animator.SetFloat(AnimData.SpeedHash, 0f);
    }
}