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
        renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogError($"[ZombieEffects] Không chớp trắng vì {gameObject.name} KHÔNG CÓ Renderer!");
            return;
        }
        
        StopCoroutine(nameof(HitFlashRoutine));
        StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        Debug.Log($"[ZombieEffects] Đang chớp trắng cho {gameObject.name} trên {renderers.Length} meshes!");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensity = 1f - (elapsed / duration);
            
            SetProperty(GameConstants.Shaders.HitFlash, intensity);
            yield return null;
        }

        SetProperty(GameConstants.Shaders.HitFlash, 0f);
    }

    public void TriggerDissolve()
    {
        StopCoroutine(nameof(HitFlashRoutine)); 
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float amount = elapsed / duration; 
            
            SetProperty(GameConstants.Shaders.DissolveAmount, amount);
            yield return null;
        }

        SetProperty(GameConstants.Shaders.DissolveAmount, 1f);
        
        gameObject.SetActive(false);
    }

    public void ResetEffects()
    {
        SetProperty(GameConstants.Shaders.HitFlash, 0f);
        SetProperty(GameConstants.Shaders.DissolveAmount, 0f);
    }

    private void SetProperty(int propID, float value)
    {
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>(true);
        }

        if (renderers == null) return;

        foreach (var r in renderers)
        {
            r.GetPropertyBlock(propBlock);
            propBlock.SetFloat(propID, value);
            r.SetPropertyBlock(propBlock);
        }
    }
}
