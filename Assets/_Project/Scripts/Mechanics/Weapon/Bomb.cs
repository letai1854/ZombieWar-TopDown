using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 6f;
    [SerializeField] private float force = 700f;
    [Tooltip("Danh sách các VFX Particle sẽ được sinh ra cùng lúc để trộn hiệu ứng")]
    [SerializeField] private System.Collections.Generic.List<GameObject> explosionEffectPrefabs;

    private float countdown;
    private bool exploded = false;

    private void OnEnable()
    {
        exploded = false;
        countdown = delay;

        // Reset lực vật lý cũ khi lấy từ Pool ra
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (exploded) return;
        
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
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
                    
                    // Tự động thu hồi VFX rác về Pool sau 3s thay vì Destroy
                    ObjectPool.Instance.ReturnToPool(vfx, 3f);
                }
            }
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
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