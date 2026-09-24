using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InitLoader : MonoBehaviour
{
    [Header("Loading UI")]
    public GameObject loadingScreen; 
    public Slider loadingSlider;
    public float loadTime = 2f; 

    private float timer = 0f;

    private void Start()
    {
        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = 120;

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
            float progress = Mathf.Clamp01(timer / loadTime)* 100f;
            loadingSlider.value = progress;
        }

        if (timer >= loadTime)
        {
            GameManager.Instance.LoadHome();

            StartCoroutine(HideLoadingRoutine());

            this.enabled = false;
        }
    }

    private IEnumerator HideLoadingRoutine()
    {
        yield return null; 
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}
