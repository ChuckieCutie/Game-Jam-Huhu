using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Danh sách tất cả các đợt spawn, hãy kéo các file SpawnPhase vào đây")]
    [SerializeField] private List<SpawnPhase> spawnPhases;

    [Header("Spawn Vị Trí")]
    [SerializeField] private Transform minPos;
    [SerializeField] private Transform maxPos;

    [Tooltip("Danh sách tất cả các đợt spawn, hãy kéo các file SpawnPhase vào đây")]

    private void Start()
    {
        // Sắp xếp các đợt theo thời gian bắt đầu để đảm bảo logic chạy đúng
        spawnPhases = spawnPhases.OrderBy(p => p.startTime).ToList();
        
        // Reset lại trạng thái của các phase mỗi khi bắt đầu game
        foreach (var phase in spawnPhases)
        {
            phase.isPhaseActive = false;
            foreach (var group in phase.enemyGroups)
            {
                group.spawnedCount = 0;
                group.spawnTimer = 0f;
            }
        }
    }

    void Update()
    {
        if (!PlayerController.Instance.gameObject.activeSelf) return;

        float currentGameTime = GameManager.Instance.gameTime;

        // Kích hoạt các đợt spawn phù hợp với thời gian hiện tại
        foreach (var phase in spawnPhases)
        {
            if (!phase.isPhaseActive && currentGameTime >= phase.startTime)
            {
                phase.isPhaseActive = true;
                Debug.Log($"Spawning phase started at {phase.startTime}s");
            }

            if (phase.isPhaseActive)
            {
                HandleSpawningForPhase(phase);
            }
        }
    }

    private void HandleSpawningForPhase(SpawnPhase phase)
    {
        foreach (var group in phase.enemyGroups)
        {
            // Nếu đã spawn đủ số lượng thì bỏ qua
            if (group.spawnedCount >= group.count) continue;

            group.spawnTimer += Time.deltaTime;
            if (group.spawnTimer >= group.spawnInterval)
            {
                group.spawnTimer = 0f;
                SpawnEnemy(group);
            }
        }
    }

    private void SpawnEnemy(EnemyGroup group)
    {
        // Gọi object từ pool thay vì Instantiate
        GameObject enemyObject = ObjectPooler.Instance.SpawnFromPool(group.enemyPrefab.name, RandomSpawnPoint(), Quaternion.identity);

        if (enemyObject != null)
        {
             // Có thể reset máu hoặc các chỉ số khác của enemy ở đây nếu cần
            Enemy enemyScript = enemyObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.ResetStats(); 
            }
            group.spawnedCount++;
        }
    }

    private Vector2 RandomSpawnPoint()
    {
        Vector2 spawnPoint;
        if (Random.Range(0f, 1f) > 0.5f)
        {
            spawnPoint.x = Random.Range(minPos.position.x, maxPos.position.x);
            spawnPoint.y = Random.Range(0f, 1f) > 0.5f ? maxPos.position.y : minPos.position.y;
        }
        else
        {
            spawnPoint.y = Random.Range(minPos.position.y, maxPos.position.y);
            spawnPoint.x = Random.Range(0f, 1f) > 0.5f ? maxPos.position.x : minPos.position.x;
        }
        return spawnPoint;
    }
}