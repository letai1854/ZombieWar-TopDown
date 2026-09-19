using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Config")]
    public string weaponName;
    public float fireRate = 0.15f;
    public Transform firePoint;
    public ParticleSystem muzzleFlash;

    protected float nextFireTime;
    public abstract void Fire();
}