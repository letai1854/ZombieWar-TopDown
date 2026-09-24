using System.Collections;
using UnityEngine;

public class Handgun : WeaponBase
{
    [Header("Adjust Direction")]
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private Vector3 muzzleFlashPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 muzzleFlashRotationOffset = Vector3.zero;

    [Header("Equip Settings")]
    [SerializeField] private float equipDelay = 0.5f; 
    private System.Collections.Generic.List<GameObject> activeFlashes = new System.Collections.Generic.List<GameObject>();
    private WeaponRecoil weaponRecoil;

    private void Awake()
    {
        fireRate = 0.35f;
        weaponRecoil = GetComponentInChildren<WeaponRecoil>();
    }

    private void OnEnable()
    {
        nextFireTime = Time.time + equipDelay;
    }

    private void OnDisable()
    {
        nextFireTime = 0f; 
        StopAllCoroutines();

        foreach (var flash in activeFlashes)
        {
            if (flash != null)
            {
                ParticleSystem ps = flash.GetComponent<ParticleSystem>();
                if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                flash.SetActive(false);
            }
        }
        activeFlashes.Clear();
    }

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate; 

        if (firePoint != null)
        {
            Vector3 shootDir = firePoint.root.forward;
            AutoShooter autoShooter = firePoint.root.GetComponent<AutoShooter>();
            if (autoShooter != null && autoShooter.CurrentTarget != null)
            {
                shootDir = autoShooter.CurrentTarget.position - firePoint.position;
            }

            shootDir.y = 0;
            if (shootDir == Vector3.zero) shootDir = transform.forward;
            Quaternion baseRotation = Quaternion.LookRotation(shootDir);

            GameObject bullet = ObjectPool.Instance != null ? ObjectPool.Instance.GetBullet() : null;
            bool bulletSpawned = false;

            if (bullet != null)
            {
                Quaternion rot = baseRotation * Quaternion.Euler(rotationOffset);

                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = rot;
                bullet.SetActive(true);
                
                bulletSpawned = true;
            }

            if (bulletSpawned)
            {
                if (SoundManager.HasInstance) SoundManager.Instance.PlayShotgunShot();
                StartCoroutine(MuzzleFlashRoutine());
            }
        }
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        yield return null;

        GameObject flashObj = ObjectPool.Instance != null ? ObjectPool.Instance.GetMuzzleFlash() : null;
        if (flashObj != null)
        {
            activeFlashes.Add(flashObj); 

            flashObj.transform.position = firePoint.position + firePoint.rotation * muzzleFlashPositionOffset;
            flashObj.transform.rotation = firePoint.rotation * Quaternion.Euler(muzzleFlashRotationOffset);
            flashObj.SetActive(true);

            ParticleSystem ps = flashObj.GetComponent<ParticleSystem>();
            if (ps != null) 
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }

            if (weaponRecoil != null)
            {
                weaponRecoil.TriggerRecoil();
            }

            yield return new WaitForSeconds(0.1f);

            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            if (flashObj != null)
            {
                flashObj.SetActive(false);
            }

            activeFlashes.Remove(flashObj); 
        }
    }
}