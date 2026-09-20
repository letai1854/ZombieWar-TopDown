using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [Header("Bullet Pool")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletPoolSize = 30;
    private List<GameObject> bulletPool;

    [Header("Sword Pool")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private int swordPoolSize = 10;
    private List<GameObject> swordPool;

    [Header("Muzzle Flash Pool (Tia lửa súng)")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private int muzzleFlashPoolSize = 10;
    private List<GameObject> muzzleFlashPool;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Khởi tạo Bullet Pool
        bulletPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            if (bulletPrefab != null)
            {
                GameObject obj = Instantiate(bulletPrefab);
                obj.SetActive(false);
                bulletPool.Add(obj);
            }
        }

        // Khởi tạo Sword Pool
        swordPool = new List<GameObject>();
        for (int i = 0; i < swordPoolSize; i++)
        {
            if (swordPrefab != null)
            {
                GameObject obj = Instantiate(swordPrefab);
                obj.SetActive(false);
                swordPool.Add(obj);
            }
        }

        // Khởi tạo Muzzle Flash Pool
        muzzleFlashPool = new List<GameObject>();
        for (int i = 0; i < muzzleFlashPoolSize; i++)
        {
            if (muzzleFlashPrefab != null)
            {
                GameObject obj = Instantiate(muzzleFlashPrefab);
                obj.SetActive(false);
                muzzleFlashPool.Add(obj);
            }
        }
    }

    public GameObject GetBullet()
    {
        foreach (var obj in bulletPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj; 
            }
        }

        if (bulletPrefab != null)
        {
            GameObject newObj = Instantiate(bulletPrefab);
            newObj.SetActive(false);
            bulletPool.Add(newObj);
            return newObj;
        }
        return null;
    }

    public GameObject GetSword()
    {
        foreach (var obj in swordPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        if (swordPrefab != null)
        {
            GameObject newObj = Instantiate(swordPrefab);
            newObj.SetActive(false);
            swordPool.Add(newObj);
            return newObj;
        }
        return null;
    }

    public GameObject GetMuzzleFlash()
    {
        GameObject selectedObj = null;

        // Tìm tia lửa đang rảnh
        foreach (var obj in muzzleFlashPool)
        {
            if (!obj.activeInHierarchy)
            {
                selectedObj = obj;
                break;
            }
        }

        // Nếu thiếu thì đẻ thêm
        if (selectedObj == null && muzzleFlashPrefab != null)
        {
            selectedObj = Instantiate(muzzleFlashPrefab);
            selectedObj.SetActive(false);
            muzzleFlashPool.Add(selectedObj);
        }

        return selectedObj;
    }
}