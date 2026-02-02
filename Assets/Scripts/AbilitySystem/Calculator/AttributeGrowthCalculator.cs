using UnityEngine;

public class AttributeGrowthCalculator
{
    public int strPoint { get; private set; }
    public int intelliPoint { get; private set; }
    public int dexPoint { get; private set; }
    public int vitalPoint { get; private set; }

    public int skilPoint { get; private set; }

    private const float GuaranteedHp = 0.6f;
    private const float CostCorrection = 0.05f;

    public float CalculateCost(float maxHealth)
    {
        return Mathf.Round(maxHealth * CostCorrection);
    }

    public float CalculateSuccessChance(float hpPercent)
    {
        return Mathf.Clamp01(hpPercent / GuaranteedHp);
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

    public void UpdateUpgradePoint()
    {
        strPoint = CalculateUpgradeValue();
        intelliPoint = CalculateUpgradeValue();
        dexPoint = CalculateUpgradeValue();
        vitalPoint = CalculateUpgradeValue();

        skilPoint = CalculateUpgradeValue();
    }

    private int CalculateUpgradeValue()
    {
        return Random.Range(1, 5);
    }
}
