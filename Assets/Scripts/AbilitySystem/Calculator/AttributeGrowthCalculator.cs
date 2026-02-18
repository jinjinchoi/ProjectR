using UnityEngine;

public class AttributeGrowthCalculator
{
    public int StrPoint { get; private set; }
    public int IntelliPoint { get; private set; }
    public int DexPoint { get; private set; }
    public int VitalPoint { get; private set; }
    public int SkilPoint { get; private set; }

    private const float GuaranteedHp = 0.8f;
    private const float CostCorrection = 0.2f;
    private const int minUpgradeValue = 3;
    private const int maxUpgradeValue = 10;
    private const int minSpValue = 1;
    private const int maxSpValue = 3;

    public float CalculateCost(float maxHealth)
    {
        return Mathf.Round(maxHealth * CostCorrection);
    }

    public float CalculateSuccessChance(float hpPercent)
    {
        float normalizedHp = Mathf.InverseLerp(CostCorrection, GuaranteedHp, hpPercent);
        return Mathf.Clamp01(normalizedHp);
    }

    public bool IsSuccessUpgrade(float hpPercent)
    {
        float successChance = CalculateSuccessChance(hpPercent);
        return Random.value <= successChance;
    }

    public float CalculateRelaxPoint(float maxHealth)
    {
        return Mathf.Round(maxHealth * 0.5f);
    }

    public void RecalculateUpgradePoint()
    {
        StrPoint = CalculateUpgradeValue();
        IntelliPoint = CalculateUpgradeValue();
        DexPoint = CalculateUpgradeValue();
        VitalPoint = CalculateUpgradeValue();

        SkilPoint = CalculateSkillValue();
    }

    private int CalculateUpgradeValue()
    {
        return Random.Range(minUpgradeValue, maxUpgradeValue);
    }

    private int CalculateSkillValue()
    {
        return Random.Range(minSpValue, maxSpValue);

    }
}
