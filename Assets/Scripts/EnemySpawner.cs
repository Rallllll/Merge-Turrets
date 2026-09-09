using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Cài đặt Quái")]
    public GameObject[] enemyPrefabs; // Chứa các Prefab quái (Ground, Flying, Hybrid)
    public float spawnInterval = 2f;  // Cách nhau 2s đẻ 1 con
    private float timer;

    [Header("Tọa độ 5 Làn (Lanes)")]
    public Transform[] spawnPoints; // Kéo 5 cái điểm Spawn_Lane_0 -> 4 vào đây

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnRandomEnemy();
            timer = spawnInterval;
        }
    }

    void SpawnRandomEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        // 1. Chọn ngẫu nhiên 1 trong 5 làn
        int randomLaneIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPos = spawnPoints[randomLaneIndex];

        // 2. Chọn ngẫu nhiên 1 loại quái
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[randomEnemyIndex];

        // 3. Sinh quái
        GameObject newEnemyObj = EnemyPool.Instance.GetEnemy(selectedPrefab, spawnPos.position, Quaternion.identity);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            // Ép quái nhận ID của làn đường để súng dò được
            newEnemy.laneID = randomLaneIndex;
        }
    }
}