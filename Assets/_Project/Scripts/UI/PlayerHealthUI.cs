using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Kéo Slider hiển thị máu vào đây")]
    public Slider healthSlider;

    private Soldier player;

    private void Start()
    {
        // Tìm player trong scene
        player = Object.FindAnyObjectByType<Soldier>();

        if (player != null && healthSlider != null)
        {
            // Cài đặt giá trị ban đầu cho Slider
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.maxHealth; // Lúc mới vào thì máu đầy

            // Đăng ký sự kiện: Khi Player bị trừ máu, tự động gọi hàm UpdateHealthUI
            player.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void OnDestroy()
    {
        // Nhớ huỷ đăng ký sự kiện khi object bị xoá để tránh lỗi bộ nhớ (Memory Leak)
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealthUI;
        }
    }

    // Hàm này sẽ tự động chạy mỗi khi máu bị trừ hoặc được hồi
    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}
