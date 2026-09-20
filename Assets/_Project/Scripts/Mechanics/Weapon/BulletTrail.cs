using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Bullet))]
public class BulletTrail : MonoBehaviour
{
    [Header("Gắn hiệu ứng của bạn vào đây (hoặc để trống tự tìm)")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private ParticleSystem particleTrail;

    private void Awake()
    {
        if (trailRenderer == null)
        {
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        if (particleTrail == null)
        {
            particleTrail = GetComponentInChildren<ParticleSystem>();
        }
    }

    private void OnEnable()
    {
        if (trailRenderer != null)
        {
            StartCoroutine(ResetTrailRoutine());
        }

        if (particleTrail != null)
        {
            particleTrail.Clear(); 
            particleTrail.Play(); 
        }
    }

    private IEnumerator ResetTrailRoutine()
    {
        trailRenderer.emitting = false;
        yield return null; 
        trailRenderer.Clear(); 
        trailRenderer.emitting = true;
    }

    private void OnDisable()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }

        if (particleTrail != null)
        {
            particleTrail.Stop(); 
        }
    }
}
