using UnityEngine;
using System.Collections.Generic;

// Dòng này để bạn có thể tạo file SpawnPhase từ menu Create trong Unity
[CreateAssetMenu(fileName = "New Spawn Phase", menuName = "Enemy Spawner/Spawn Phase")]
public class SpawnPhase : ScriptableObject
{
    [Tooltip("Thời điểm trong game (giây) để bắt đầu đợt này")]
    public float startTime;

    [Tooltip("Danh sách các loại quái và số lượng sẽ spawn trong đợt này")]
    public List<EnemyGroup> enemyGroups;

    // Biến này để Spawner theo dõi, không cần chỉnh trong Editor
    [HideInInspector]
    public bool isPhaseActive = false;
}

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    [Tooltip("Tổng số lượng quái loại này sẽ spawn")]
    public int count;
    [Tooltip("Thời gian chờ giữa mỗi lần spawn một con quái trong nhóm này")]
    public float spawnInterval;

    // Biến nội bộ để theo dõi
    [HideInInspector]
    public int spawnedCount = 0;
    [HideInInspector]
    public float spawnTimer = 0f;
}