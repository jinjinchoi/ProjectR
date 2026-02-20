using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static event Action BattleWon; // 게임 매니저에 알려서 승리 이후 로직 담당하게 하는게 좋을 수도 있음.

    [SerializeField] private Transform[] spawnPoints;

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
                Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPointCount)];

                GameObject obj = Instantiate(spawnInfo.Prefab, spawnPoint.transform.position, Quaternion.identity);
                var enemy = obj.GetComponent<EnemyCharacter>();
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
            BattleWon?.Invoke();
        }
    }

    private void OnPlayerDied()
    {
        // game over
    }
}
