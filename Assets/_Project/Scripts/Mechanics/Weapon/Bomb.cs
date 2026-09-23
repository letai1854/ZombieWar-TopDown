using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 6f;
    [SerializeField] private float force = 700f;
    [SerializeField] private float damage = 100f; // Sát thương của lựu đạn
    [Tooltip("Danh sách các VFX Particle sẽ được sinh ra cùng lúc để trộn hiệu ứng")]
    [SerializeField] private System.Collections.Generic.List<GameObject> explosionEffectPrefabs;

    private float countdown;
    private bool exploded = false;
    private float beepTimer;
    private float beepInterval;
    private AudioSource bombAudioSource;

    private void OnEnable()
    {
        exploded = false;
        countdown = delay;

        if (bombAudioSource == null)
        {
            bombAudioSource = gameObject.AddComponent<AudioSource>();
            bombAudioSource.spatialBlend = 0f; // Đổi về 2D (0f) để Camera góc nhìn trên cao luôn nghe rõ
            bombAudioSource.playOnAwake = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        beepInterval = 0.5f;
        beepTimer = 0f; // Sửa số này thành 0 để bom kêu tiếng đầu tiên NGAY LẬP TỨC khi vừa rời tay!
    }

    private void Update()
    {
        if (exploded) return;
        
        countdown -= Time.deltaTime;

        float urgency = 1f - Mathf.Clamp01(countdown / delay); 
        beepInterval = Mathf.Lerp(0.5f, 0.08f, urgency); 
        
        beepTimer -= Time.deltaTime;
        if (beepTimer <= 0f)
        {
            beepTimer = beepInterval;
            
            if (SoundManager.HasInstance && bombAudioSource != null)
            {
                AudioClip tickClip = SoundManager.Instance.bombTickSFX;
                
                if (tickClip != null)
                {
                    bombAudioSource.pitch = 1f; // Nếu có âm thanh chuẩn do bạn gắn, không cần bóp méo cao độ nữa
                    bombAudioSource.PlayOneShot(tickClip, 1f);
                }
                else
                {
                    // Dự phòng nếu bạn chưa gắn: Lấy tiếng súng bóp méo
                    AudioClip fallbackClip = SoundManager.Instance.buttonClickSFX != null 
                                         ? SoundManager.Instance.buttonClickSFX 
                                         : SoundManager.Instance.rifleShotSFX;
                    if (fallbackClip != null)
                    {
                        bombAudioSource.pitch = 2.5f; 
                        bombAudioSource.PlayOneShot(fallbackClip, 0.7f);
                    }
                }
            }
        }

        if (countdown <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (SoundManager.HasInstance) SoundManager.Instance.PlayBombExplosion();
        if (explosionEffectPrefabs != null && explosionEffectPrefabs.Count > 0)
        {
            foreach (GameObject prefab in explosionEffectPrefabs)
            {
                if (prefab != null && ObjectPool.Instance != null)
                {
                    GameObject vfx = ObjectPool.Instance.GetFromPool(prefab);
                    vfx.transform.position = transform.position;
                    vfx.transform.rotation = Quaternion.identity;
                    vfx.SetActive(true);
                    
                    ObjectPool.Instance.ReturnToPool(vfx, 3f);
                }
            }
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Zombie zombie = hit.GetComponent<Zombie>();
                if (zombie != null)
                {
                    zombie.TakeDamage(damage);
                }
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius, 1f, ForceMode.Impulse);
            }
        }

#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == gameObject)
        {
            UnityEditor.Selection.activeGameObject = null;
        }
#endif

        gameObject.SetActive(false);
    }
}