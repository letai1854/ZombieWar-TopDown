using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : Entity
{
    public NavMeshAgent Agent { get; private set; }
    
    public ZombieStateMachine StateMachine { get; private set; }
    public ZombieIdleState IdleState { get; private set; }
    public ZombieChaseState ChaseState { get; private set; }
    public ZombieAttackState AttackState { get; private set; }
    public ZombieDeadState DeadState { get; private set; }

    public Transform TargetPlayer { get; private set; }
    private ZombieEffects effects;

    [Header("Zombie Settings")]
    public float detectionRadius = 100f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float maxHealth = 100f;
    
    private float currentHealth;
    public bool IsDead => currentHealth <= 0;

    protected override void Awake()
    {
        base.Awake();
        Agent = GetComponent<NavMeshAgent>();
        effects = GetComponent<ZombieEffects>();
        if (effects == null) effects = gameObject.AddComponent<ZombieEffects>();
        
        if (Controller != null)
        {
            Controller.enabled = false;
        }

        StateMachine = new ZombieStateMachine();
        
        IdleState = new ZombieIdleState(this, StateMachine, "isIdle");
        ChaseState = new ZombieChaseState(this, StateMachine, "isChasing");
        AttackState = new ZombieAttackState(this, StateMachine, "isAttacking");
        DeadState = new ZombieDeadState(this, StateMachine, "isDead");
    }

    private void Start()
    {
        currentHealth = maxHealth;
        // Balance: random tốc độ để quái ép góc người chơi (không thể chỉ chạy trốn 1 mạch)
        Agent.speed = moveSpeed * Random.Range(0.8f, 1.4f); 
        
        Agent.stoppingDistance = attackRange * 0.8f;

        if (TargetPlayer == null)
        {
            Soldier soldier = Object.FindAnyObjectByType<Soldier>();
            if (soldier != null)
            {
                TargetPlayer = soldier.transform;
            }
        }

        StateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        Agent.stoppingDistance = attackRange * 0.8f;

        if (StateMachine.CurrentState != null)
        {
            StateMachine.CurrentState.LogicUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (StateMachine.CurrentState != null)
        {
            StateMachine.CurrentState.PhysicsUpdate();
        }
    }


    public float GetDistanceToPlayer()
    {
        if (TargetPlayer == null) return float.MaxValue;
        Vector3 p1 = transform.position;
        Vector3 p2 = TargetPlayer.position;
        p1.y = 0;
        p2.y = 0;
        return Vector3.Distance(p1, p2);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        
        // Gọi hiệu ứng Hit Flash
        if (effects != null) effects.TriggerHitFlash();

        if (currentHealth <= 0)
        {
            if (SoundManager.HasInstance) SoundManager.Instance.PlayZombieDead();
            StateMachine.ChangeState(DeadState);
            // Gọi hiệu ứng Dissolve
            if (effects != null) effects.TriggerDissolve();
        }
    }

    // Hàm này được gọi từ Unity Animation Event (vào đúng frame Zombie cào trúng)
    public void DealDamageEvent()
    {
        if (IsDead || TargetPlayer == null) return;
        
        if (SoundManager.HasInstance) SoundManager.Instance.PlayZombieAttack();

        // Chỉ gây damage nếu Player vẫn nằm trong tầm cào
        if (GetDistanceToPlayer() <= attackRange * 1.5f) // Dư dả một chút để đánh dễ trúng
        {
            Soldier soldier = TargetPlayer.GetComponent<Soldier>();
            if (soldier != null)
            {
                soldier.TakeDamage(attackDamage);
            }
        }
    }

    // Được gọi bởi ObjectPool khi tái sử dụng Zombie
    public void Revive()
    {
        currentHealth = maxHealth;
        if (Controller != null) Controller.enabled = false;
        Agent.enabled = true;
        
        // Balance: random lại tốc độ mỗi lần hồi sinh
        Agent.speed = moveSpeed * Random.Range(0.8f, 1.4f);

        // Khôi phục Layer và bật lại Collider
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            c.enabled = true;
        }
        
        if (effects != null) effects.ResetEffects();

        // Đảm bảo luôn lấy được mục tiêu khi revive
        if (TargetPlayer == null)
        {
            Soldier soldier = Object.FindAnyObjectByType<Soldier>();
            if (soldier != null) TargetPlayer = soldier.transform;
        }

        StateMachine.Initialize(IdleState);
    }
}
