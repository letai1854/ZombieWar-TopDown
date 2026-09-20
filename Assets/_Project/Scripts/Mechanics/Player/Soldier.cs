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
    public SoldierThrowBombState ThrowBombState { get; private set; }

    public bool IsShooting { get; private set; }
    public bool IsThrowingBomb { get; set; }

    [Header("Bomb Settings")]
    [Tooltip("Thời gian đứng yên để phát animation ném bom trước khi trở lại bình thường (Thời gian GỐC)")]
    [SerializeField] private float throwBombDuration = 1.2f;
    [Tooltip("Vũ khí bên tay trái (AttachedPistol) để ẩn đi khi ném bom")]
    [SerializeField] private GameObject leftHandWeapon;
    [Tooltip("Kéo object WeaponManager vào đây để tính thêm thời gian delay riêng của từng súng")]
    [SerializeField] private WeaponManager weaponManager;
    
    [Header("Bomb Spawning")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform leftHandSpawnPoint;
    [SerializeField] private BombTrajectory bombTrajectory;
    [SerializeField] private float throwForwardForce = 15f;
    [SerializeField] private float throwUpwardForce = 5f;

    public BombTrajectory BombTrajectory => bombTrajectory;
    public Transform LeftHandSpawnPoint => leftHandSpawnPoint;
    public float ThrowForwardForce => throwForwardForce;
    public float ThrowUpwardForce => throwUpwardForce;
    public GameObject BombPrefab => bombPrefab;

    protected override void Awake()
    {
        base.Awake();
        StateMachine = new SoldierStateMachine();
        IdleState = new SoldierIdleState(this, StateMachine);
        MoveState = new SoldierMoveState(this, StateMachine);
        AttackState = new SoldierAttackState(this, StateMachine);
        ThrowBombState = new SoldierThrowBombState(this, StateMachine);
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
        if (!IsThrowingBomb)
        {
            StateMachine.ChangeState(AttackState);
        }
    }

    public void OnPointerUpShoot()
    {
        IsShooting = false;
    }

    public void OnPointerDownThrowBomb()
    {
        if (!IsThrowingBomb)
        {
            StateMachine.ChangeState(ThrowBombState);
        }
    }

    private bool wasLeftHandWeaponActive;

    public void ToggleLeftHandWeapon(bool isActive)
    {
        if (leftHandWeapon != null)
        {
            if (!isActive)
            {
                wasLeftHandWeaponActive = leftHandWeapon.activeSelf;
                leftHandWeapon.SetActive(false);
            }
            else
            {
                leftHandWeapon.SetActive(wasLeftHandWeaponActive);
            }
        }
    }

    public float GetTotalThrowDuration()
    {
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }

        float total = throwBombDuration;
        if (weaponManager != null && weaponManager.CurrentWeapon != null)
        {
            total += weaponManager.CurrentWeapon.extraThrowDelay;
            Debug.Log($"[BOMB] Tổng thời gian chờ: {total}s (Gốc {throwBombDuration}s + Thêm {weaponManager.CurrentWeapon.extraThrowDelay}s)");
        }
        else
        {
            Debug.LogWarning("[BOMB] Thiếu WeaponManager hoặc Súng! Chỉ dùng thời gian gốc.");
        }
        return total;
    }

    // Cầu nối (Bridge) chuyển tiếp sự kiện Animation cho State xử lý (Tuân thủ SOLID)
    public void AnimationEvent_SpawnBomb()
    {
        if (StateMachine.CurrentState is SoldierThrowBombState throwState)
        {
            throwState.SpawnAndThrowBomb();
        }
    }
}