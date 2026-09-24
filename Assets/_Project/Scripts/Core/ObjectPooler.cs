using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        Queue<GameObject> queue = poolDictionary[tag];
        GameObject objectToSpawn = queue.Dequeue();

        // Khắc phục lỗi lôi vật thể đang dùng ra chỗ khác: 
        // Nếu object lấy ra VẪN ĐANG ACTIVE (tức là nó đang được xài trên Scene)
        if (objectToSpawn.activeInHierarchy)
        {
            // 1. Trả nó lại vào cuối hàng chờ để nó không bị mất tích trên Scene
            queue.Enqueue(objectToSpawn);

            // 2. Tự động đẻ thêm 1 bản sao mới toanh vì kho đã cạn
            Pool poolDef = pools.Find(p => p.tag == tag);
            if (poolDef != null)
            {
                objectToSpawn = Instantiate(poolDef.prefab, transform);
            }
            else
            {
                return null; // Phòng hờ lỗi nếu không tìm thấy gốc
            }
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        // Đưa object này vào cuối hàng chờ để xoay vòng cho các lần sau
        queue.Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    // Đưa Object về Pool (Tắt đi) sau một khoảng thời gian
    public void ReturnToPool(GameObject obj, float delay)
    {
        if (obj != null && gameObject.activeInHierarchy)
        {
            StartCoroutine(ReturnRoutine(obj, delay));
        }
    }

    private System.Collections.IEnumerator ReturnRoutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) obj.SetActive(false);
    }
}