using UnityEngine;

public class AttributeGrowthCalculator
{
    public int strPoint { get; private set; }
    public int intelliPoint { get; private set; }
    public int dexPoint { get; private set; }
    public int vitalPoint { get; private set; }

    public int skilPoint { get; private set; }

    private const float GuaranteedHp = 0.8f;
    private const float CostCorrection = 0.2f;
    private const int minUpgradeValue = 3;
    private const int maxUpgradeValue = 10;

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
        strPoint = CalculateUpgradeValue();
        intelliPoint = CalculateUpgradeValue();
        dexPoint = CalculateUpgradeValue();
        vitalPoint = CalculateUpgradeValue();

        skilPoint = CalculateUpgradeValue();
    }

    private int CalculateUpgradeValue()
    {
        return Random.Range(minUpgradeValue, maxUpgradeValue);
    }
}
