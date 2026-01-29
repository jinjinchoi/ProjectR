using UnityEngine;


public class UIController_RestArea : BaseUIController
{
    private AttributeGrowthCalculator growthCalculator;

    protected override void Awake()
    {
        base.Awake();

        growthCalculator = new AttributeGrowthCalculator();
        growthCalculator.UpdateUpgradePoint();
    }

    public float GetAttributeValue(EAttributeType attributeType)
    {
        if (abilitySystem == null) return 0f;

        return abilitySystem.AttributeSet.GetAttributeValue(attributeType);
    }

    public float GetHealthPercent()
    {
        if (abilitySystem == null) return 0f;

        float currentHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.currentHealth);
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.maxHealth);

        return currentHealth / maxHealth;
    }

    public float GetSuccessChance()
    {
        return growthCalculator.CalculateSuccessChance(GetHealthPercent()) * 100f;
    }

    public float GetUpgradeCost()
    {
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.maxHealth);
        return growthCalculator.CalculateCost(maxHealth);
    }

    public int GetUpgradeValue(EAttributeType attribute)
    {
        return attribute switch
        {
            EAttributeType.strength => growthCalculator.strPoint,
            EAttributeType.dexterity => growthCalculator.dexPoint,
            EAttributeType.intelligence => growthCalculator.intelliPoint,
            EAttributeType.vitality => growthCalculator.vitalPoint,
            _ => 0,
        };
    }

    public float GetRelaxValue()
    {
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.maxHealth);
        return growthCalculator.CalculateRelaxPoint(maxHealth);
    }

    public void UpgaradeAttribute(EAttributeType attribute)
    {
        if (growthCalculator.IsSuccessUpgrade(GetHealthPercent()))
        {
            FAttributeModifier upgradeModifier = new()
            {
                attributeType = attribute,
                isPermanent = true,
                operation = EModifierOp.Add,
                value = GetUpgradeValue(attribute)
            };

            abilitySystem.ApplyModifier(upgradeModifier);

            FAttributeModifier costModifier = new()
            {
                attributeType = EAttributeType.incommingDamage,
                isPermanent = true,
                operation = EModifierOp.Add,
                value = GetUpgradeCost()
            };

            abilitySystem.ApplyModifier(costModifier);
        }
    }
}
