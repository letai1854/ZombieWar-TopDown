using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Danh sách vũ khí (0: Rifle tay phải, 1: Pistol tay trái)")]
    [SerializeField] private List<WeaponBase> weapons;
    
    [Header("Animator điều khiển tư thế tay")]
    [SerializeField] private Animator animator;
    
    private int currentWeaponIndex = 0;
    private Soldier soldier;

    public WeaponBase CurrentWeapon => weapons.Count > 0 ? weapons[currentWeaponIndex] : null;

    private void Start()
    {
        // Tự động tìm reference đến Soldier để check các trạng thái khóa (như ném bom)
        soldier = GetComponentInParent<Soldier>();
        if (soldier == null) soldier = FindObjectOfType<Soldier>();

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
        // Tuyệt đối không cho phép đổi súng nếu đang ném bom (để tránh lỗi deactive súng giữa chừng)
        if (soldier != null && soldier.IsThrowingBomb)
        {
            Debug.LogWarning("[WEAPON] Không thể đổi súng lúc này vì đang ném bom!");
            return;
        }

        if (weapons.Count <= 1) return;
        
        if (SoundManager.HasInstance) SoundManager.Instance.PlayButtonClick();
        
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipWeapon(currentWeaponIndex);
    }

    public void ShootCurrentWeapon()
    {
        if (CurrentWeapon != null) CurrentWeapon.Fire();
    }
}