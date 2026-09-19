using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Danh sách vũ khí (0: Rifle tay phải, 1: Pistol tay trái)")]
    [SerializeField] private List<WeaponBase> weapons;
    
    [Header("Animator điều khiển tư thế tay")]
    [SerializeField] private Animator animator;
    
    private int currentWeaponIndex = 0;

    public WeaponBase CurrentWeapon => weapons.Count > 0 ? weapons[currentWeaponIndex] : null;

    private void Start()
    {
        if (weapons.Count > 0) EquipWeapon(0);
    }

    public void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].gameObject.SetActive(i == index);
            }
        }
        currentWeaponIndex = index;

        Debug.Log("Dang doi sung, index = " + index);
        if (animator != null)
        {
            animator.SetInteger("WeaponIndex", currentWeaponIndex);
        }
    }

    public void SwitchWeapon()
    {
        if (weapons.Count <= 1) return;
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipWeapon(currentWeaponIndex);
        
    }

    public void ShootCurrentWeapon()
    {
        if (CurrentWeapon != null) CurrentWeapon.Fire();
    }
}