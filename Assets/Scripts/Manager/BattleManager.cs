using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private string restAreaSceneName = "RestArea";

    private int spawnedEnemyCount;

    private void Awake()
    {
        EventHub.PlayerDied += OnPlayerDied;
    }

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            DebugHelper.LogWarning("Spawn points not configured");
            return;
        }

        BattleEventInfo sceneEnemyInfo = GameManager.Instance.GetCurrentEventEnemyInfo();
        if (sceneEnemyInfo != null )
            SpawnEnemy(sceneEnemyInfo.enemieInfos);
    }

    private void SpawnEnemy(List<EnemySpawnInfo> enemySpawnInfoList)
    {
        int spawnPointCount = spawnPoints.Length;

        foreach (EnemySpawnInfo spawnInfo in enemySpawnInfoList)
        {
            if (spawnInfo?.Prefab == null) continue;

            for (int i = 0; i < spawnInfo.Count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPointCount)];

                EnemyCharacter enemy = Instantiate(spawnInfo.Prefab, spawnPoint.transform.position, Quaternion.identity);
                enemy.Init(spawnInfo.Level);
                enemy.CharacterDied += OnEnemyDied;
                spawnedEnemyCount++;
            }
        }
    }

    private void OnEnemyDied()
    {
        spawnedEnemyCount--;
        if (spawnedEnemyCount == 0)
        {
            _ = GameManager.Instance.LoadSceneAsync(restAreaSceneName);
        }
    }

    private void OnPlayerDied()
    {
        // game over
    }
}
