using UnityEngine;

public class Soldier : Entity
{
    [Header("Player Input Settings")]
    [SerializeField] private FixedJoystick joystick;

    public FixedJoystick Joystick => joystick;

    public SoldierStateMachine StateMachine { get; private set; }
    public SoldierIdleState IdleState { get; private set; }
    public SoldierMoveState MoveState { get; private set; }
    public SoldierAttackState AttackState { get; private set; }

    public bool IsShooting { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        StateMachine = new SoldierStateMachine();
        IdleState = new SoldierIdleState(this, StateMachine);
        MoveState = new SoldierMoveState(this, StateMachine);
        AttackState = new SoldierAttackState(this, StateMachine);
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState?.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState?.PhysicsUpdate();
    }
    public void OnPointerDownShoot()
    {
        IsShooting = true;
        StateMachine.ChangeState(AttackState);
    }

    public void OnPointerUpShoot()
    {
        IsShooting = false;
    }
}