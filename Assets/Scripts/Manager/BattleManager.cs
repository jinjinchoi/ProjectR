using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            DebugHelper.LogWarning("Spawn points not configured");
            return;
        }

        BattleEventInfo sceneEnemyInfo = GameManager.Instance.GetCurrentEventEnemyInfo();
        SpawnEnemy(sceneEnemyInfo.enemieInfos);
    }

    private void SpawnEnemy(List<EnemySpawnInfo> enemySpawnInfoList)
    {
        EnemyAttribtueSO enemyTable = GameManager.Instance.EnemyAttribtueSO;
        int spawnPointCount = spawnPoints.Length;

        foreach (var spawnInfo in enemySpawnInfoList)
        {
            EnemyInformation enemyInfo = enemyTable.GetEnemyInfo(spawnInfo.enemyId);
            if (enemyInfo?.Prefab == null) continue;

            FEnemySecondaryAttribute attributeInfo = CreateEnemyAttributeInfo(enemyInfo, spawnInfo.level);

            for (int i = 0; i < spawnInfo.count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPointCount)];

                EnemyCharacter enemy = Instantiate(enemyInfo.Prefab, spawnPoint.transform.position, Quaternion.identity);
                enemy.Init(attributeInfo);
            }
        }
    }

    private FEnemySecondaryAttribute CreateEnemyAttributeInfo(EnemyInformation enemyInfo, int enemyLevel)
    {
        return new FEnemySecondaryAttribute()
        {
            level = enemyLevel,
            physicalAttackPower = enemyInfo.GetPhysicalAttackPower(enemyLevel),
            physicalDefensePower = enemyInfo.GetPhysicalDefensePower(enemyLevel),
            magicAttackPower = enemyInfo.GetMagicAttackPower(enemyLevel),
            magicDefensePower = enemyInfo.GetMagicDefensePower(enemyLevel),
            criticalChance = enemyInfo.GetCriticalChance(enemyLevel),
            maxHealth = enemyInfo.GetMaxHealth(enemyLevel)
        };
    }
}
