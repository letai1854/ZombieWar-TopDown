using UnityEngine;
using DG.Tweening;

public class AutoShooter : MonoBehaviour
{
    [Header("Auto-Aim Settings")]
    [SerializeField] private float detectionRadius = 12f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private float rotationSpeed = 15f; 

    [Header("Field of View (Triangle) Settings")]
    [SerializeField, Range(0, 360)] private float fieldOfView = 120f; 

    [Header("Upper Body Aim Settings")]
    [SerializeField] private Transform spineBone; 

    private Transform currentTarget;
    public Transform CurrentTarget => currentTarget;
    
    private float currentSpineYaw = 0f;
    private float targetSpineYaw = 0f;
    private Tween aimTween;
    private Soldier soldier;

    private void Start()
    {
        soldier = GetComponent<Soldier>();
        if (weaponManager == null)
        {
            weaponManager = GetComponentInChildren<WeaponManager>();
        }
    }

    private void Update()
    {
        // Nếu đang ném bom thì tuyệt đối KHÔNG quét quái và KHÔNG bắn đạn
        if (soldier != null && soldier.IsThrowingBomb) return;

        FindClosestEnemyInFOV();
        
        // CẬP NHẬT GÓC XOAY NGAY TRONG UPDATE ĐỂ TRÁNH TRỄ 1 FRAME
        UpdateTargetYaw(); 

        if (currentTarget != null)
        {
            // CHỈ BẮN KHI ĐÃ XOAY XONG VÀ HƯỚNG MẶT ĐÃ KHỚP (Sai số góc <= 2 độ)
            // Fix triệt để lỗi viên đạn đầu tiên bay ra trước khi mặt kịp quay
            if (Mathf.Abs(targetSpineYaw - currentSpineYaw) <= 2f)
            {
                if (weaponManager != null)
                {
                    weaponManager.ShootCurrentWeapon();
                }
            }
        }
    }

    private void LateUpdate()
    {
        // Áp dụng góc xoay vào xương trong LateUpdate để không bị Animator ghi đè
        if (spineBone != null && Mathf.Abs(currentSpineYaw) > 0.1f)
        {
            spineBone.rotation = Quaternion.AngleAxis(currentSpineYaw, Vector3.up) * spineBone.rotation;
        }
    }

    private void FindClosestEnemyInFOV()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider col in colliders)
        {
            Vector3 dirToEnemy = (col.transform.position - transform.position).normalized;
            dirToEnemy.y = 0;

            if (Vector3.Angle(transform.forward, dirToEnemy) < fieldOfView / 2f)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = col.transform;
                }
            }
        }
        currentTarget = closest;
    }

    private void UpdateTargetYaw()
    {
        if (currentTarget != null)
        {
            Vector3 dirToTarget = currentTarget.position - transform.position;
            dirToTarget.y = 0;

            if (dirToTarget != Vector3.zero)
            {
                float angleToTarget = Vector3.SignedAngle(transform.forward, dirToTarget, Vector3.up);
                float newTargetYaw = Mathf.Clamp(angleToTarget, -fieldOfView / 2f, fieldOfView / 2f);
                
                if (Mathf.Abs(targetSpineYaw - newTargetYaw) > 1f)
                {
                    targetSpineYaw = newTargetYaw;
                    
                    aimTween?.Kill(); 
                    aimTween = DOTween.To(() => currentSpineYaw, x => currentSpineYaw = x, targetSpineYaw, 0.15f)
                        .SetEase(Ease.OutQuad);
                }
            }
        }
        else
        {
            if (targetSpineYaw != 0f)
            {
                targetSpineYaw = 0f;
                aimTween?.Kill();
                aimTween = DOTween.To(() => currentSpineYaw, x => currentSpineYaw = x, 0f, 0.2f)
                    .SetEase(Ease.OutQuad);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward * detectionRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward * detectionRadius;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}