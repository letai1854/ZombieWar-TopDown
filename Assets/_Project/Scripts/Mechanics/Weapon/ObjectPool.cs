using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 30;

    private List<GameObject> bulletPool;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            bulletPool.Add(obj);
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

        GameObject newObj = Instantiate(bulletPrefab);
        newObj.SetActive(false);
        bulletPool.Add(newObj);
        return newObj;
    }
}