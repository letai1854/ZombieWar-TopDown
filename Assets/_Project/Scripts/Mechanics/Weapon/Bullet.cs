using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 35f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private GameObject hitEffectPrefab;

    private float timer;

    private void OnEnable() => timer = 0f;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        timer += Time.deltaTime;
        if (timer >= lifeTime) gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet")) return;

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false); 
    }
}