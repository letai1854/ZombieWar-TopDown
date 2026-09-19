using UnityEngine;

public class Rifle : WeaponBase
{
    [Header("Adjust Direction")]
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (muzzleFlash != null) muzzleFlash.Play();

        GameObject bullet = ObjectPool.Instance != null ? ObjectPool.Instance.GetBullet() : null;
        if (bullet != null && firePoint != null)
        {
            float currentYAngle = firePoint.eulerAngles.y;
            Quaternion flatRotation = Quaternion.Euler(0f, currentYAngle, 0f) * Quaternion.Euler(rotationOffset);

            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = flatRotation;

            bullet.SetActive(true);
        }
    }
}