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
    public float attackDamage = 25f;
    public float maxHealth = 100f;
    
    private float currentHealth;
    public bool IsDead => currentHealth <= 0;

    public float ActualAttackRange => attackRange * transform.localScale.x;

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
        Agent.speed = moveSpeed * Random.Range(0.8f, 1.4f); 
        
        Agent.stoppingDistance = ActualAttackRange * 0.8f;

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
        Agent.stoppingDistance = ActualAttackRange * 0.8f;

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
        Debug.Log($"[{gameObject.name}] Đã trúng đạn! Máu còn: {currentHealth}");
        
        if (effects != null) effects.TriggerHitFlash();

        if (ObjectPooler.HasInstance)
        {
            float chestHeight = 1f * transform.localScale.y; 
            Vector3 chestPosition = transform.position + Vector3.up * chestHeight + transform.forward * (0.1f * transform.localScale.z);
            
            GameObject bloodVFX = ObjectPooler.Instance.SpawnFromPool("BloodVFX", chestPosition, Quaternion.LookRotation(Vector3.up));
            
            if (bloodVFX != null)
            {
                Debug.Log($"[{gameObject.name}] Đã spawn thành công BloodVFX tại {chestPosition}!");
                bloodVFX.transform.localScale = Vector3.one * transform.localScale.x;
                ObjectPooler.Instance.ReturnToPool(bloodVFX, 1.5f);
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] KHÔNG TÌM THẤY 'BloodVFX' trong ObjectPooler! Bạn hãy kiểm tra lại danh sách Pool trên Manager!");
            }
        }

        if (currentHealth <= 0)
        {
            if (SoundManager.HasInstance) SoundManager.Instance.PlayZombieDead();
            StateMachine.ChangeState(DeadState);
            if (effects != null) effects.TriggerDissolve();
        }
    }

    public void DealDamageEvent()
    {
        if (IsDead || TargetPlayer == null) return;
        
        if (SoundManager.HasInstance) SoundManager.Instance.PlayZombieAttack();

        if (GetDistanceToPlayer() <= ActualAttackRange * 1.5f) 
        {
            Soldier soldier = TargetPlayer.GetComponent<Soldier>();
            if (soldier != null)
            {
                soldier.TakeDamage(attackDamage);
            }
        }
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        if (Controller != null) Controller.enabled = false;
        Agent.enabled = true;
        
        if (Agent.isActiveAndEnabled)
        {
            Agent.Warp(transform.position);
        }

        Agent.speed = moveSpeed * Random.Range(0.8f, 1.4f);

        gameObject.layer = LayerMask.NameToLayer("Enemy");
        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            c.enabled = true;
        }
        
        if (effects != null) effects.ResetEffects();

        if (TargetPlayer == null)
        {
            Soldier soldier = Object.FindAnyObjectByType<Soldier>();
            if (soldier != null) TargetPlayer = soldier.transform;
        }

        StateMachine.Initialize(IdleState);
    }
}
