using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifeTime = 3f;

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Deactivate), lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet")) return;
        Deactivate();
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}