using System.Collections;
using UnityEngine;

public class Shotgun : WeaponBase
{
    [Header("Shotgun Burst Settings")]
    [SerializeField] private int pelletsPerShot = 3;   
    [SerializeField] private float spreadAngle = 10f;  
    [SerializeField] private int burstCount = 3;     
    [SerializeField] private float burstDelay = 0.08f;  
    [Header("Adjust Direction")]
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    private bool isFiring = false;

    public override void Fire()
    {
        if (Time.time < nextFireTime || isFiring) return;
        nextFireTime = Time.time + fireRate;

        StartCoroutine(FireBurstRoutine());
    }

    private IEnumerator FireBurstRoutine()
    {
        isFiring = true;

        for (int b = 0; b < burstCount; b++)
        {
            if (muzzleFlash != null) muzzleFlash.Play();

            if (firePoint != null)
            {
                float currentYAngle = firePoint.eulerAngles.y;

                float[] angles = { -spreadAngle, 0f, spreadAngle };

                for (int i = 0; i < pelletsPerShot; i++)
                {
                    GameObject bullet = ObjectPool.Instance != null ? ObjectPool.Instance.GetBullet() : null;
                    if (bullet != null)
                    {
                        Quaternion rot = Quaternion.Euler(0f, currentYAngle + angles[i], 0f) * Quaternion.Euler(rotationOffset);

                        bullet.transform.position = firePoint.position;
                        bullet.transform.rotation = rot;
                        bullet.SetActive(true);
                    }
                }
            }

            yield return new WaitForSeconds(burstDelay);
        }

        isFiring = false;
    }
}