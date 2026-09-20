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

        UpdateTrajectory();
    }

    private void UpdateTrajectory()
    {
        // --- LOGIC: SMART TARGETING (VÙNG QUÉT RIÊNG CHO BOMB 360 ĐỘ) ---
        Vector3 startPos = soldier.LeftHandSpawnPoint != null ? soldier.LeftHandSpawnPoint.position : soldier.transform.position + Vector3.up;
        
        // Quét quái riêng cho Bom (360 độ, bán kính 15m)
        Transform bombTarget = FindClosestEnemyForBomb(soldier.transform.position, 15f);

        if (bombTarget != null)
        {
            Vector3 targetPos = bombTarget.position;
            
            // Tính toán khoảng cách bù trừ động: Tối đa lùi 2 mét, nhưng nếu địch quá gần thì lùi ít lại
            Vector3 dirToTarget = (targetPos - startPos).normalized;
            dirToTarget.y = 0; 
            float distance = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
            
            float offsetDistance = Mathf.Min(2f, distance * 0.5f);
            targetPos -= dirToTarget * offsetDistance;

            Vector3 displacement = targetPos - startPos;

            Vector3 velocityXZ = new Vector3(displacement.x, 0, displacement.z) / FIXED_TIME_OF_FLIGHT;
            float velocityY = (displacement.y - 0.5f * Physics.gravity.y * FIXED_TIME_OF_FLIGHT * FIXED_TIME_OF_FLIGHT) / FIXED_TIME_OF_FLIGHT;

            calculatedThrowVelocity = velocityXZ + Vector3.up * velocityY;
        }
        else
        {
            // Lựa chọn 2: Ném thẳng theo hướng nhân vật (Cập nhật liên tục theo Joystick)
            calculatedThrowVelocity = soldier.transform.forward * soldier.ThrowForwardForce + Vector3.up * soldier.ThrowUpwardForce;
        }

        if (soldier.BombTrajectory != null)
        {
            soldier.BombTrajectory.ShowTrajectory(startPos, calculatedThrowVelocity);
        }
    }

    // Hàm quét quái vật ĐỘC LẬP dành riêng cho Ném Bom (Quét 360 độ xung quanh)
    private Transform FindClosestEnemyForBomb(Vector3 center, float checkRadius)
    {
        // Giả sử quái vật của bạn nằm ở Layer "Enemy". Cần sửa lại LayerMask nếu game của bạn dùng layer khác.
        int enemyLayerMask = LayerMask.GetMask("Enemy", "Default"); 
        Collider[] colliders = Physics.OverlapSphere(center, checkRadius, enemyLayerMask);
        
        float minDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider col in colliders)
        {
            // Kiểm tra Tag Enemy cho chắc chắn
            if (col.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(center, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestEnemy = col.transform;
                }
            }
        }
        return closestEnemy;
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
