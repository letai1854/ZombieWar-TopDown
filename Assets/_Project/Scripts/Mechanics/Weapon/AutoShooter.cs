using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Auto-Aim Settings")]
    [SerializeField] private float detectionRadius = 12f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private float rotationSpeed = 15f;

    private Transform currentTarget;

    private void Update()
    {
        FindClosestEnemy();

        if (currentTarget != null)
        {
            AimAtTarget();
            
            if (weaponManager != null)
            {
                weaponManager.ShootCurrentWeapon();
            }
        }
    }

    private void FindClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider col in colliders)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = col.transform;
            }
        }
        currentTarget = closest;
    }

    private void AimAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}