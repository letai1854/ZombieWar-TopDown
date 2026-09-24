using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Bone Settings")]
    [Tooltip("TÍCH VÀO nếu script gắn trên Xương bị Animator điều khiển (như Bip001 R Hand của Rifle).\nBỎ TÍCH nếu gắn trên Súng Rời (như AttachedPistol).")]
    [SerializeField] private bool isAnimatedBone = false;

    [Header("Recoil Position Settings")]
    [Tooltip("Khoảng cách súng bị giật lùi về sau (Trục Z local)")]
    [SerializeField] private float recoilZ = -0.15f;
    [SerializeField] private float recoilDuration = 0.05f;
    [SerializeField] private float returnDuration = 0.2f;

    [Header("Recoil Rotation Settings")]
    [Tooltip("Góc hếch nòng súng lên trên khi bắn (Trục X local)")]
    [SerializeField] private float recoilPitch = -5f; 
    
    [Header("Camera Shake")]
    [Tooltip("Gắn CinemachineImpulseSource vào object này để rung màn hình")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private Vector3 originalPos;
    private Quaternion originalRotQuat;

    private Tween recoilPosTween;
    private Tween recoilRotTween;
    
    private float currentRecoilZ = 0f;
    private float currentRecoilPitch = 0f;

    private void Awake()
    {
        originalPos = transform.localPosition;
        originalRotQuat = transform.localRotation;
        
        if (impulseSource == null) 
            impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void TriggerRecoil()
    {
        recoilPosTween?.Kill();
        recoilRotTween?.Kill();

        if (isAnimatedBone)
        {
        
            currentRecoilZ = 0f;
            currentRecoilPitch = 0f;

            recoilPosTween = DOTween.To(() => currentRecoilZ, x => currentRecoilZ = x, recoilZ, recoilDuration)
                .SetEase(Ease.OutExpo)
                .OnComplete(() => 
                {
                    recoilPosTween = DOTween.To(() => currentRecoilZ, x => currentRecoilZ = x, 0f, returnDuration).SetEase(Ease.InOutSine);
                });

            recoilRotTween = DOTween.To(() => currentRecoilPitch, x => currentRecoilPitch = x, recoilPitch, recoilDuration)
                .SetEase(Ease.OutExpo)
                .OnComplete(() => 
                {
                    recoilRotTween = DOTween.To(() => currentRecoilPitch, x => currentRecoilPitch = x, 0f, returnDuration).SetEase(Ease.InOutSine);
                });
        }
        else
        {
           
            transform.localPosition = originalPos;
            transform.localRotation = originalRotQuat;

            recoilPosTween = transform.DOLocalMoveZ(originalPos.z + recoilZ, recoilDuration)
                .SetEase(Ease.OutExpo)
                .OnComplete(() => 
                {
                    recoilPosTween = transform.DOLocalMoveZ(originalPos.z, returnDuration).SetEase(Ease.InOutSine);
                });

            Quaternion recoilTargetRot = originalRotQuat * Quaternion.Euler(recoilPitch, 0, 0);

            recoilRotTween = transform.DOLocalRotateQuaternion(recoilTargetRot, recoilDuration)
                .SetEase(Ease.OutExpo)
                .OnComplete(() => 
                {
                    recoilRotTween = transform.DOLocalRotateQuaternion(originalRotQuat, returnDuration).SetEase(Ease.InOutSine);
                });
        }

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }

    private void LateUpdate()
    {
        if (isAnimatedBone)
        {
            if (Mathf.Abs(currentRecoilZ) > 0.001f || Mathf.Abs(currentRecoilPitch) > 0.001f)
            {
                transform.localPosition += new Vector3(0, 0, currentRecoilZ);
                transform.localRotation *= Quaternion.Euler(currentRecoilPitch, 0, 0);
            }
        }
    }

    private void OnDisable()
    {
        recoilPosTween?.Kill();
        recoilRotTween?.Kill();
        currentRecoilZ = 0f;
        currentRecoilPitch = 0f;
        
        if (!isAnimatedBone)
        {
            transform.localPosition = originalPos;
            transform.localRotation = originalRotQuat;
        }
    }
}