using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InitLoader : MonoBehaviour
{
    [Header("Loading UI")]
    public GameObject loadingScreen; // Kéo Image nền đỏ vào đây
    public Slider loadingSlider;
    public float loadTime = 2f; // Thời gian giả lập load

    private float timer = 0f;

    private void Start()
    {
        if (loadingSlider != null)
        {
            loadingSlider.minValue = 0f;
            loadingSlider.maxValue = 100f;
            loadingSlider.value = 0f;
        }
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
    }

    private void Update()
    {
        if (!GameManager.HasInstance) return;

        timer += Time.deltaTime;
        
        if (loadingSlider != null)
        {
            float progress = Mathf.Clamp01(timer / loadTime) * 100f;
            loadingSlider.value = progress;
        }

        if (timer >= loadTime)
        {
            // Load xong, chuyển sang Home trước
            GameManager.Instance.LoadHome();
            
            // Đợi 1 frame để scene mới (Home) load xong rồi mới tắt màn hình đỏ
            // Tránh lỗi bị chớp nháy thấy camera trống của scene cũ
            StartCoroutine(HideLoadingRoutine());

            // Tắt script này để tránh gọi hàm LoadHome nhiều lần
            this.enabled = false;
        }
    }

    private IEnumerator HideLoadingRoutine()
    {
        yield return null; // Đợi 1 frame
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}
