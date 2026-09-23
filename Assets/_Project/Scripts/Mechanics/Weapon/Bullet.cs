using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 30f;
    [SerializeField] private float maxDistance = 25f; 
    [SerializeField] private float maxLifeTime = 2f;  

    private Vector3 spawnPosition;
    private float currentLifeTime;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void OnEnable()
    {
        spawnPosition = transform.position;
        currentLifeTime = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        currentLifeTime += Time.deltaTime;

        float travelledDistance = Vector3.Distance(spawnPosition, transform.position);
        if (travelledDistance >= maxDistance || currentLifeTime >= maxLifeTime)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet")) return;

        if (other.CompareTag("Enemy"))
        {
            Zombie zombie = other.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.TakeDamage(25f); // Sát thương của đạn
            }
        }

        Deactivate();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}