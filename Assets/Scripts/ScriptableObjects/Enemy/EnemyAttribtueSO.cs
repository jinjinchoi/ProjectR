using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EEnemyId
{
    None,
    Pang,
    HoodedDemon
}

[System.Serializable]
public class EnemyInformation
{
    [SerializeField] private EEnemyId enemyId;

    [SerializeField] private AnimationCurve physicalAttackPowerCurve;
    [SerializeField] private AnimationCurve physicalDefensePowerCurve;
    [SerializeField] private AnimationCurve magicAttackPowerCurve;
    [SerializeField] private AnimationCurve magicDefensePowerCurve;
    [SerializeField] private AnimationCurve criticalChanceCurve;
    [SerializeField] private AnimationCurve maxHealthCurve;

    public EEnemyId EnemyId => enemyId;

    public float GetAttributeValue(EAttributeType attributeType, int level)
    {
        float value = attributeType switch
        {
            EAttributeType.PhysicalAttackPower
                => physicalAttackPowerCurve.Evaluate(level),

            EAttributeType.PhysicalDefensePower
                => physicalDefensePowerCurve.Evaluate(level),

            EAttributeType.MagicAttackPower
                => magicAttackPowerCurve.Evaluate(level),

            EAttributeType.MagicDefensePower
                => magicDefensePowerCurve.Evaluate(level),

            EAttributeType.CriticalChance
                => criticalChanceCurve.Evaluate(level),

            EAttributeType.MaxHealth
                => maxHealthCurve.Evaluate(level),

            _ => 0f
        };

        return Mathf.Round(value);
    }
}

[CreateAssetMenu(fileName = "Attribute_", menuName = "ASC/EnemyInformation")]
public class EnemyAttribtueSO : ScriptableObject
{
    public List<EnemyInformation> enemyInfoList;

    private Dictionary<EEnemyId, EnemyInformation> enemyInfoMap;

    public void Init()
    {
        enemyInfoMap = enemyInfoList.ToDictionary(e => e.EnemyId);
    }

    public EnemyInformation GetEnemyInfo(EEnemyId id)
    {
        if (enemyInfoMap == null || enemyInfoMap.Count == 0)
            Init();

        enemyInfoMap.TryGetValue(id, out var data);
        return data;
    }
}
