using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    private Vector3 originalPos;
    [SerializeField] private float recoilZ = -0.15f;
    [SerializeField] private float returnSpeed = 12f;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * returnSpeed);
    }

    public void TriggerRecoil()
    {
        transform.localPosition += new Vector3(0, 0, recoilZ);
    }
}