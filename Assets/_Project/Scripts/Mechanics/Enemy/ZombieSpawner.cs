using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public string zombiePoolTag = "Zombie";
    public float initialSpawnInterval = 2f;
    public float spawnDistance = 20f; // Khoảng cách spawn tính từ player (đảm bảo ngoài màn hình)

    [Header("Rhythm / Difficulty")]
    public float minSpawnInterval = 0.5f; // Tốc độ spawn nhanh nhất (dồn dập)
    public float timeToMaxDifficulty = 180f; // Thời gian đạt tốc độ nhanh nhất (VD: 3 phút)
    
    private float currentSpawnInterval;
    private float gameTimer;

    private float spawnTimer;
    private Transform playerTransform;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        gameTimer = 0f;
        
        // Tìm player để spawn xung quanh
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

        // Tăng dần độ khó (nhịp điệu dồn dập hơn theo thời gian)
        gameTimer += Time.deltaTime;
        float difficultyPercent = Mathf.Clamp01(gameTimer / timeToMaxDifficulty);
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, difficultyPercent);

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnZombie();
        }
    }

    private void SpawnZombie()
    {
        // Balance: Tăng số lượng quái sinh ra mỗi nhịp theo thời gian (từ 1 đến 3 con)
        float difficultyPercent = Mathf.Clamp01(gameTimer / timeToMaxDifficulty);
        int spawnCount = Mathf.FloorToInt(Mathf.Lerp(1, 3, difficultyPercent));
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
            // Ép toạ độ Y ngang với Player ban đầu để ưu tiên quét mặt đất
            randomPos.y = playerTransform.position.y; 

            // Quét tìm điểm NavMesh gần nhất trong vòng 10m
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                // Kỹ thuật chốt chặn: Thử vẽ đường đi từ điểm đó tới Player
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(hit.position, playerTransform.position, NavMesh.AllAreas, path))
                {
                    // Nếu đường đi thông suốt (PathComplete), nghĩa là không bị kẹt trên nóc nhà
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
}
