using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Kéo Slider hiển thị máu vào đây")]
    public Slider healthSlider;
    
    [Tooltip("Kéo Image chứa hiệu ứng máu viền màn hình vào đây")]
    public Image bloodScreen;
    
    [Tooltip("Độ mờ tối đa khi nhá máu (1 là rõ nhất, 0.5 là hơi mờ)")]
    public float maxAlpha = 1f;
    public float flashSpeed = 5f;

    private Soldier player;
    private bool isFlashing = false;
    private float previousHealth;

    private void Start()
    {
        player = Object.FindAnyObjectByType<Soldier>();

        if (player != null && healthSlider != null)
        {
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.maxHealth; 
            previousHealth = player.maxHealth;

            if (bloodScreen != null)
            {
                Color c = bloodScreen.color;
                c.a = 0f;
                bloodScreen.color = c;
                bloodScreen.gameObject.SetActive(false);
            }


            player.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth < previousHealth && bloodScreen != null)
        {
            isFlashing = true;
            bloodScreen.gameObject.SetActive(true);
        }
        previousHealth = currentHealth;
    }

    private void Update()
    {
        if (bloodScreen != null && bloodScreen.gameObject.activeSelf)
        {
            if (isFlashing)
            {
                Color c = bloodScreen.color;
                c.a = maxAlpha;
                bloodScreen.color = c;
                isFlashing = false; 
            }
            else
            {
                Color c = bloodScreen.color;
                c.a = Mathf.Lerp(c.a, 0f, flashSpeed * Time.deltaTime);
                bloodScreen.color = c;

                if (bloodScreen.color.a <= 0.01f)
                {
                    c.a = 0f;
                    bloodScreen.color = c;
                    bloodScreen.gameObject.SetActive(false);
                }
            }
        }
    }
}
