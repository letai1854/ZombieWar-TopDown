using UnityEngine;

public class SoldierThrowBombState : SoldierState
{
    private float throwTimer;
    private float throwDuration;
    
    private static readonly int ThrowBombHash = Animator.StringToHash("ThrowBomb");

    private Vector3 calculatedThrowVelocity;
    private const float FIXED_TIME_OF_FLIGHT = 1.0f;
    private bool hasThrownBomb = false; // Biến đánh dấu bom đã rời tay chưa

    public SoldierThrowBombState(Soldier soldier, SoldierStateMachine stateMachine) 
        : base(soldier, stateMachine) 
    { 
    }

    public override void Enter()
    {
        base.Enter();
        soldier.IsThrowingBomb = true;
        hasThrownBomb = false;
        throwTimer = 0f;
        
        throwDuration = soldier.GetTotalThrowDuration();
        
        soldier.ToggleLeftHandWeapon(false); 
        
        if (soldier.Animator != null)
        {
            soldier.Animator.SetTrigger(ThrowBombHash);
            soldier.Animator.SetFloat(AnimData.SpeedHash, 0f);
        }

        // TÌM MỤC TIÊU 360 ĐỘ THAY VÌ DÙNG AUTO SHOOTER
        Transform bombTarget = FindClosestEnemyForBomb();

        if (bombTarget != null)
        {
            Vector3 dirToTarget = (bombTarget.position - soldier.transform.position).normalized;
            dirToTarget.y = 0;
            if (dirToTarget != Vector3.zero)
            {
                soldier.transform.rotation = Quaternion.LookRotation(dirToTarget);
            }
        }

        UpdateTrajectory();
    }

    private Transform FindClosestEnemyForBomb()
    {
        // Quét góc rộng 360 độ (bán kính 15m) để dễ dàng chọn quái ném bom
        float detectionRadius = 15f; 
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider[] colliders = Physics.OverlapSphere(soldier.transform.position, detectionRadius, enemyLayer);
        
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider col in colliders)
        {
            float dist = Vector3.Distance(soldier.transform.position, col.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = col.transform;
            }
        }
        return closest;
    }

    private void UpdateTrajectory()
    {
        Vector3 startPos = soldier.LeftHandSpawnPoint != null ? soldier.LeftHandSpawnPoint.position : soldier.transform.position + Vector3.up;
        
        // Quét tìm quái liên tục để cập nhật đường bay
        Transform bombTarget = FindClosestEnemyForBomb();

        if (bombTarget != null)
        {
            // Lựa chọn 1 (Có Quái): MƯỢN KHOẢNG CÁCH của quái, nhưng ÉP HƯỚNG THEO JOYSTICK
            float distanceToEnemy = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(bombTarget.position.x, 0, bombTarget.position.z));
            float offsetDistance = Mathf.Min(2f, distanceToEnemy * 0.5f);
            float finalThrowDistance = distanceToEnemy - offsetDistance;

            Vector3 throwDirection = soldier.transform.forward;
            throwDirection.y = 0;
            throwDirection.Normalize();

            Vector3 velocityXZ = throwDirection * (finalThrowDistance / FIXED_TIME_OF_FLIGHT);
            
            // Giữ nguyên tính toán độ cao rớt theo vị trí Y của quái
            float heightDiff = bombTarget.position.y - startPos.y;
            float velocityY = (heightDiff - 0.5f * Physics.gravity.y * FIXED_TIME_OF_FLIGHT * FIXED_TIME_OF_FLIGHT) / FIXED_TIME_OF_FLIGHT;

            calculatedThrowVelocity = velocityXZ + Vector3.up * velocityY;
        }
        else
        {
            // Lựa chọn 2 (Không có quái): Ném hướng Joystick với lực cố định
            calculatedThrowVelocity = soldier.transform.forward * soldier.ThrowForwardForce + Vector3.up * soldier.ThrowUpwardForce;
        }

        if (soldier.BombTrajectory != null)
        {
            soldier.BombTrajectory.ShowTrajectory(startPos, calculatedThrowVelocity);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (soldier.Joystick != null)
        {
            Vector3 moveInput = new Vector3(soldier.Joystick.Horizontal, 0f, soldier.Joystick.Vertical);
            float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);

            soldier.Animator.SetFloat(AnimData.SpeedHash, inputMagnitude, 0.1f, Time.deltaTime);

            if (inputMagnitude >= 0.1f)
            {
                Vector3 moveDirection = moveInput.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                soldier.transform.rotation = Quaternion.Slerp(soldier.transform.rotation, targetRotation, soldier.RotateSpeed * Time.deltaTime);
            }
        }

        // CẬP NHẬT ĐƯỜNG BAY LIÊN TỤC: Chỉ cập nhật khi quả bom CHƯA rời khỏi tay
        if (!hasThrownBomb)
        {
            UpdateTrajectory();
        }
        else
        {
            // Tắt đường kẻ đi ngay khi bom vừa văng ra
            if (soldier.BombTrajectory != null)
            {
                soldier.BombTrajectory.HideTrajectory();
            }
        }

        throwTimer += Time.deltaTime;
        
        if (throwTimer >= throwDuration)
        {
            soldier.IsThrowingBomb = false;
            
            if (soldier.BombTrajectory != null)
                soldier.BombTrajectory.HideTrajectory();

            if (soldier.Joystick != null)
            {
                Vector3 moveInput = new Vector3(soldier.Joystick.Horizontal, 0f, soldier.Joystick.Vertical);
                if (moveInput.magnitude > 0.1f)
                {
                    stateMachine.ChangeState(soldier.MoveState);
                    return;
                }
            }
            
            stateMachine.ChangeState(soldier.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (soldier.Joystick != null)
        {
            Vector3 moveInput = new Vector3(soldier.Joystick.Horizontal, 0f, soldier.Joystick.Vertical);
            float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);

            if (inputMagnitude >= 0.1f)
            {
                Vector3 moveDirection = moveInput.normalized;
                soldier.Controller.Move(moveDirection * soldier.MoveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        soldier.IsThrowingBomb = false;
        soldier.ToggleLeftHandWeapon(true); 

        if (soldier.BombTrajectory != null)
            soldier.BombTrajectory.HideTrajectory();
    }

    // Xử lý sinh vật lý của Bom tại State này để đảm bảo Single Responsibility
    public void SpawnAndThrowBomb()
    {
        if (soldier.BombPrefab == null || soldier.LeftHandSpawnPoint == null)
        {
            Debug.LogError("[BOMB] Thiếu BombPrefab hoặc LeftHandSpawnPoint trên Soldier!");
            return;
        }

        GameObject bomb = null;
        if (ObjectPool.Instance != null)
        {
            bomb = ObjectPool.Instance.GetFromPool(soldier.BombPrefab);
            bomb.transform.position = soldier.LeftHandSpawnPoint.position;
            bomb.transform.rotation = Quaternion.identity;
            bomb.SetActive(true);
        }
        else
        {
            bomb = Object.Instantiate(soldier.BombPrefab, soldier.LeftHandSpawnPoint.position, Quaternion.identity);
        }

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.linearVelocity = calculatedThrowVelocity;
            rb.angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        }

        // Đánh dấu bom đã rời tay để lập tức tắt đường ngắm Parabol
        hasThrownBomb = true;
    }
}
