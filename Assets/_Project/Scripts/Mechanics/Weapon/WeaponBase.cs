using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Config")]
    public string weaponName;
    public float fireRate = 0.15f;
    public Transform firePoint;
    public ParticleSystem muzzleFlash;
    
    [Tooltip("Thời gian delay cộng thêm khi ném bom dành riêng cho vũ khí này")]
    public float extraThrowDelay = 0f;

    protected float nextFireTime;
    public abstract void Fire();
}