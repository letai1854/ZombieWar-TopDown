using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 6f;
    [SerializeField] private float force = 700f;
    [SerializeField] private float damage = 100f; 
    [Tooltip("Danh sách các VFX Particle sẽ được sinh ra cùng lúc để trộn hiệu ứng")]
    [SerializeField] private System.Collections.Generic.List<GameObject> explosionEffectPrefabs;

    private float countdown;
    private bool exploded = false;
    private float beepTimer;
    private float beepInterval;
    private AudioSource bombAudioSource;
    private GameObject aoeIndicator;
    private Material aoeMaterial;
    private LineRenderer aoeOutline;

    private void OnEnable()
    {
        exploded = false;
        countdown = delay;

        if (bombAudioSource == null)
        {
            bombAudioSource = gameObject.AddComponent<AudioSource>();
            bombAudioSource.spatialBlend = 0f; 
            bombAudioSource.playOnAwake = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        beepInterval = 0.5f;
        beepTimer = 0f; 

        if (aoeOutline == null)
        {
            aoeOutline = gameObject.AddComponent<LineRenderer>();
            aoeOutline.startWidth = 0.1f;
            aoeOutline.endWidth = 0.1f;
            aoeOutline.loop = true;
            aoeOutline.useWorldSpace = true;
            aoeOutline.positionCount = 40;
            
            Shader spriteShader = Shader.Find(GameConstants.Shaders.SpritesDefault);
            if (spriteShader != null)
            {
                aoeOutline.material = new Material(spriteShader);
                aoeOutline.startColor = new Color(1f, 0f, 0f, 0.8f);
                aoeOutline.endColor = new Color(1f, 0f, 0f, 0.8f);
            }
        }
        aoeOutline.enabled = true;

        if (aoeIndicator == null)
        {
            aoeIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(aoeIndicator.GetComponent<Collider>()); 
            
            aoeIndicator.transform.SetParent(transform);
            
            Shader spriteShader = Shader.Find(GameConstants.Shaders.SpritesDefault);
            if (spriteShader != null)
            {
                aoeMaterial = new Material(spriteShader);
                aoeIndicator.GetComponent<Renderer>().material = aoeMaterial;
            }
        }
        aoeIndicator.SetActive(true);
    }

    private void OnDisable()
    {
        if (aoeOutline != null) aoeOutline.enabled = false;
        if (aoeIndicator != null) aoeIndicator.SetActive(false);
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
                    bombAudioSource.pitch = 1f; 
                    bombAudioSource.PlayOneShot(tickClip, 1f);
                }
                else
                {
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

        DrawAoEIndicator(urgency);

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
            if (hit.CompareTag(GameConstants.Tags.Enemy))
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

    private void DrawAoEIndicator(float urgency)
    {
        Vector3 center = transform.position;
        center.y = 0.15f; 

        if (aoeOutline != null)
        {
            int segments = aoeOutline.positionCount;
            for (int i = 0; i < segments; i++)
            {
                float angle = ((float)i / segments) * Mathf.PI * 2f;
                float x = Mathf.Sin(angle) * radius; 
                float z = Mathf.Cos(angle) * radius;
                aoeOutline.SetPosition(i, center + new Vector3(x, 0, z));
            }
        }

        if (aoeIndicator == null || aoeMaterial == null) return;

        
        float flashSpeed = 1.5f + (urgency * 2f); 
        float sineValue = Mathf.Sin(Time.time * flashSpeed); 
        float smoothPulse = (sineValue + 1f) * 0.5f; 

        float alpha = 0.05f + (smoothPulse * 0.15f); 
        aoeMaterial.color = new Color(1f, 0f, 0f, alpha); 
        
        float scaleMultiplier = smoothPulse;
        float currentDiameter = (radius * 2f) * scaleMultiplier;
        
        aoeIndicator.transform.localScale = new Vector3(currentDiameter, 0.01f, currentDiameter);
        aoeIndicator.transform.position = center;
        aoeIndicator.transform.rotation = Quaternion.identity; 
    }
}