using UnityEngine;

public class Rifle : WeaponBase
{
    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (muzzleFlash != null) muzzleFlash.Play();

        GameObject bullet = ObjectPool.Instance != null ? ObjectPool.Instance.GetBullet() : null;
        if (bullet != null && firePoint != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
        }
    }
}