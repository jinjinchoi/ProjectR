using System;
using UnityEngine;

[Serializable]
public class AttributeGrowthData
{
    public int StrPoint;
    public int IntelliPoint;
    public int DexPoint;
    public int VitalPoint;
    public int SkillPoint;

    public float RelexPoint;
    public float Cost;
    public float SuccessChance = 1;

    public int GetUpgradeValueByType(EAttributeType type)
    {
        return type switch
        {
            EAttributeType.Strength => StrPoint,
            EAttributeType.Intelligence => IntelliPoint,
            EAttributeType.Dexterity => DexPoint,
            EAttributeType.Vitality => VitalPoint,
            EAttributeType.SkillPoint => SkillPoint,
            _ => 0
        };
    }
}

public class AttributeGrowthCalculator
{
    private const float guaranteedHp = 0.8f;
    private const float costCorrection = 0.2f;
    private const int minUpgradeValue = 3;
    private const int maxUpgradeValue = 10;
    private const int minSpValue = 1;
    private const int maxSpValue = 5;

    public AttributeGrowthData Generate(float currentHealth, float maxHelath)
    {
        return new AttributeGrowthData
        {
            StrPoint = UnityEngine.Random.Range(minUpgradeValue, maxUpgradeValue),
            IntelliPoint = UnityEngine.Random.Range(minUpgradeValue, maxUpgradeValue),
            DexPoint = UnityEngine.Random.Range(minUpgradeValue, maxUpgradeValue),
            VitalPoint = UnityEngine.Random.Range(minUpgradeValue, maxUpgradeValue),
            SkillPoint = UnityEngine.Random.Range(minSpValue, maxSpValue),
            RelexPoint = CalculateRelaxPoint(maxHelath),
            Cost = CalculateCost(maxHelath),
            SuccessChance = CalculateSuccessChance(currentHealth / maxHelath)
        };
    }

    public float CalculateRelaxPoint(float maxHealth)
    {
        return Mathf.Round(maxHealth * 0.5f);
    }

    public float CalculateCost(float maxHealth)
    {
        return Mathf.Round(maxHealth * costCorrection);
    }

    public float CalculateSuccessChance(float hpPercent)
    {
        float normalizedHp = Mathf.InverseLerp(costCorrection, guaranteedHp, hpPercent);
        return Mathf.Clamp01(normalizedHp);
    }

    public bool IsSuccessUpgrade(float hpPercent)
    {
        float successChance = CalculateSuccessChance(hpPercent);
        return UnityEngine.Random.value <= successChance;
    }

}
