using UnityEngine;
using System.Collections;

public class ZombieEffects : MonoBehaviour
{
    private SkinnedMeshRenderer[] renderers;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    public void TriggerHitFlash()
    {
        StopCoroutine(nameof(HitFlashRoutine));
        StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensity = 1f - (elapsed / duration); // Chạy từ 1 về 0
            
            SetProperty("_HitFlash", intensity);
            yield return null;
        }

        SetProperty("_HitFlash", 0f);
    }

    public void TriggerDissolve()
    {
        StopCoroutine(nameof(HitFlashRoutine)); // Ngừng chớp sáng nếu đang chết
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float amount = elapsed / duration; // Chạy từ 0 đến 1
            
            SetProperty("_DissolveAmount", amount);
            yield return null;
        }

        SetProperty("_DissolveAmount", 1f);
        
        // Sau khi tan biến xong, ẩn Zombie đi (cho ObjectPool)
        gameObject.SetActive(false);
    }

    public void ResetEffects()
    {
        SetProperty("_HitFlash", 0f);
        SetProperty("_DissolveAmount", 0f);
    }

    private void SetProperty(string propName, float value)
    {
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(propBlock);
            propBlock.SetFloat(propName, value);
            r.SetPropertyBlock(propBlock);
        }
    }
}
