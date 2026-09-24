using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public string zombiePoolTag = "Zombie";
    public string bossPoolTag = "GiantZombie"; 
    public float initialSpawnInterval = 1.5f; 
    public float spawnDistance = 20f; 

    [Header("Rhythm / Difficulty")]
    public float minSpawnInterval = 0.3f; 
    public float timeToMaxDifficulty = 180f; 
    
    [Header("Boss Settings")]
    public bool enableBoss = false; 
    public float timeToSpawnBoss = 15f; 
    
    private float currentSpawnInterval;
    private float gameTimer;

    private float spawnTimer;
    private Transform playerTransform;
    
    private bool hasSpawnedBoss = false;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        gameTimer = 0f;
        
        Soldier player = Object.FindAnyObjectByType<Soldier>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;
        if (GameManager.HasInstance && GameManager.Instance.CurrentState != GameState.Playing) return;

        gameTimer += Time.deltaTime;
        float difficultyPercent = Mathf.Clamp01(gameTimer / timeToMaxDifficulty);
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, difficultyPercent);

        if (enableBoss && !hasSpawnedBoss && gameTimer >= timeToSpawnBoss)
        {
            hasSpawnedBoss = true;
            SpawnBoss();
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnZombie();
        }
    }

    private void SpawnZombie()
    {
        float difficultyPercent = Mathf.Clamp01(gameTimer / timeToMaxDifficulty);
        int spawnCount = Mathf.FloorToInt(Mathf.Lerp(1, 4, difficultyPercent));
        int spawned = 0;

        for (int i = 0; i < 20; i++)
        {
            if (spawned >= spawnCount) break;

            int dir = Random.Range(0, 4);
            Vector3 spawnOffset = Vector3.zero;

            switch (dir)
            {
                case 0: spawnOffset = new Vector3(Random.Range(-spawnDistance, spawnDistance), 0, spawnDistance); break;
                case 1: spawnOffset = new Vector3(Random.Range(-spawnDistance, spawnDistance), 0, -spawnDistance); break;
                case 2: spawnOffset = new Vector3(-spawnDistance, 0, Random.Range(-spawnDistance, spawnDistance)); break;
                case 3: spawnOffset = new Vector3(spawnDistance, 0, Random.Range(-spawnDistance, spawnDistance)); break;
            }

            Vector3 randomPos = playerTransform.position + spawnOffset;
            randomPos.y = playerTransform.position.y; 

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(hit.position, playerTransform.position, NavMesh.AllAreas, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        GameObject zombieObj = ObjectPooler.Instance.SpawnFromPool(zombiePoolTag, hit.position, Quaternion.identity);
                        
                        if (zombieObj != null)
                        {
                            Zombie zombie = zombieObj.GetComponent<Zombie>();
                            if (zombie != null)
                            {
                                zombie.Revive(); 
                            }
                            spawned++;
                        }
                    }
                }
            }
        }
    }

    private void SpawnBoss()
    {
        Debug.Log("[ZombieSpawner] Bắt đầu gọi SpawnBoss! Đang thả Boss...");
        
        Vector3 spawnOffset = new Vector3(8f, 0, 8f); 
        Vector3 randomPos = playerTransform.position + spawnOffset;
        randomPos.y = playerTransform.position.y;

        GameObject bossObj = ObjectPooler.Instance.SpawnFromPool(bossPoolTag, randomPos, Quaternion.identity);
        if (bossObj != null)
        {
            Debug.Log("[ZombieSpawner] Đã thả Boss thành công ra mặt đất!");
            Zombie boss = bossObj.GetComponent<Zombie>();
            if (boss != null) boss.Revive();
        }
        else
        {
            string allTags = "";
            foreach (var key in ObjectPooler.Instance.poolDictionary.Keys) allTags += "'" + key + "' ";
            Debug.LogError("[ZombieSpawner] LỖI: Không tìm thấy Boss. Pool Tag đang tìm: '" + bossPoolTag + "'. Các Tag có sẵn: " + allTags);
        }
    }
}
