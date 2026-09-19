using UnityEngine;
using Cinemachine;

public class Shotgun : WeaponBase
{
    [Header("Shotgun Settings")]
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float spreadAngle = 8f;

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (muzzleFlash != null) muzzleFlash.Play();

        if (firePoint != null)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                GameObject bullet = ObjectPool.Instance.GetBullet();
                if (bullet != null)
                {
                    float randomAngle = Random.Range(-spreadAngle, spreadAngle);
                    Quaternion rot = firePoint.rotation * Quaternion.Euler(0, randomAngle, 0);
                    bullet.transform.position = firePoint.position;
                    bullet.transform.rotation = rot;
                }
            }
        }

        GetComponentInParent<WeaponRecoil>()?.TriggerRecoil();
        CinemachineImpulseSource impulse = GetComponent<CinemachineImpulseSource>();
        if (impulse != null) impulse.GenerateImpulse();
    }
}