using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyInformation
{
    [SerializeField] private string enemyId;
    [SerializeField] private EnemyCharacter enemyPrefab;

    [SerializeField] private AnimationCurve physicalAttackPowerCurve;
    [SerializeField] private AnimationCurve physicalDefensePowerCurve;
    [SerializeField] private AnimationCurve magicAttackPowerCurve;
    [SerializeField] private AnimationCurve magicDefensePowerCurve;
    [SerializeField] private AnimationCurve criticalChanceCurve;
    [SerializeField] private AnimationCurve maxHealthCurve;

    public string EnemyId => enemyId;
    public EnemyCharacter Prefab => enemyPrefab;

    #region Attribute Getter
    public int GetPhysicalAttackPower(int level)
        => Mathf.RoundToInt(physicalAttackPowerCurve.Evaluate(level));

    public int GetPhysicalDefensePower(int level)
        => Mathf.RoundToInt(physicalDefensePowerCurve.Evaluate(level));

    public int GetMagicAttackPower(int level)
        => Mathf.RoundToInt(magicAttackPowerCurve.Evaluate(level));

    public int GetMagicDefensePower(int level)
        => Mathf.RoundToInt(magicDefensePowerCurve.Evaluate(level));

    public float GetCriticalChance(int level)
        => Mathf.Clamp01(criticalChanceCurve.Evaluate(level));

    public int GetMaxHealth(int level)
        => Mathf.RoundToInt(maxHealthCurve.Evaluate(level));
    #endregion
}

[CreateAssetMenu(fileName = "EnemyInformation", menuName = "ASC/EnemyInformation")]
public class EnemyAttribtueSO : ScriptableObject
{
    public List<EnemyInformation> enemyInfoList;

    private Dictionary<string, EnemyInformation> enemyInfoMap;

    public void Init()
    {
        enemyInfoMap = enemyInfoList.ToDictionary(e => e.EnemyId);
    }

    public EnemyInformation GetEnemyInfo(string id)
    {
        enemyInfoMap.TryGetValue(id, out var data);
        return data;
    }
}
