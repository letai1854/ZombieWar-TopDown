using UnityEngine;
using System.Collections;

public class ZombieEffects : MonoBehaviour
{
    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    public void TriggerHitFlash()
    {
        // Quét lại toàn bộ lưới mỗi lần bắn trúng để đảm bảo KHÔNG THỂ trượt bất kỳ lưới nào (dù nó được load sau)
        renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogError($"[ZombieEffects] Không thể chớp trắng vì {gameObject.name} KHÔNG CÓ Renderer!");
            return;
        }
        
        StopCoroutine(nameof(HitFlashRoutine));
        StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        // Báo log để chứng minh Code đã gọi chớp trắng
        Debug.Log($"[ZombieEffects] Đang chớp trắng cho {gameObject.name} trên {renderers.Length} meshes!");

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
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>(true);
        }

        if (renderers == null) return;

        foreach (var r in renderers)
        {
            r.GetPropertyBlock(propBlock);
            propBlock.SetFloat(propName, value);
            r.SetPropertyBlock(propBlock);
        }
    }
}
