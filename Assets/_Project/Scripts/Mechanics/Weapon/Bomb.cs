using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 6f;
    [SerializeField] private float force = 700f;
    [SerializeField] private GameObject explosionEffectPrefab;

    private float countdown;
    private bool exploded = false;

    private void Start() => countdown = delay;

    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !exploded)
        {
            Explode();
            exploded = true;
        }
    }

    private void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
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

        Destroy(gameObject);
    }
}