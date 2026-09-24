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

    private Dictionary<GameObject, List<GameObject>> genericPool = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            if (bulletPrefab != null)
            {
                GameObject obj = Instantiate(bulletPrefab, transform);
                obj.SetActive(false);
                bulletPool.Add(obj);
            }
        }

        swordPool = new List<GameObject>();
        for (int i = 0; i < swordPoolSize; i++)
        {
            if (swordPrefab != null)
            {
                GameObject obj = Instantiate(swordPrefab, transform);
                obj.SetActive(false);
                swordPool.Add(obj);
            }
        }

        muzzleFlashPool = new List<GameObject>();
        for (int i = 0; i < muzzleFlashPoolSize; i++)
        {
            if (muzzleFlashPrefab != null)
            {
                GameObject obj = Instantiate(muzzleFlashPrefab, transform);
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
            GameObject newObj = Instantiate(bulletPrefab, transform);
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
            GameObject newObj = Instantiate(swordPrefab, transform);
            newObj.SetActive(false);
            swordPool.Add(newObj);
            return newObj;
        }
        return null;
    }

    public GameObject GetMuzzleFlash()
    {
        GameObject selectedObj = null;

        foreach (var obj in muzzleFlashPool)
        {
            if (!obj.activeInHierarchy)
            {
                selectedObj = obj;
                break;
            }
        }

        if (selectedObj == null && muzzleFlashPrefab != null)
        {
            selectedObj = Instantiate(muzzleFlashPrefab, transform);
            selectedObj.SetActive(false);
            muzzleFlashPool.Add(selectedObj);
        }

        return selectedObj;
    }

    public GameObject GetFromPool(GameObject prefab)
    {
        if (prefab == null) return null;

        if (!genericPool.ContainsKey(prefab))
        {
            genericPool[prefab] = new List<GameObject>();
        }

        foreach (var obj in genericPool[prefab])
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab, transform);
        newObj.SetActive(false);
        genericPool[prefab].Add(newObj);
        return newObj;
    }

    public void ReturnToPool(GameObject obj, float delay)
    {
        if (obj != null && gameObject.activeInHierarchy)
        {
            StartCoroutine(ReturnRoutine(obj, delay));
        }
    }

    private IEnumerator ReturnRoutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) obj.SetActive(false);
    }
}