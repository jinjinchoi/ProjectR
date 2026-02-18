public class UIController_RestArea : BaseCharacterUIController
{
    private AttributeGrowthCalculator growthCalculator;

    public override void Init(IAbilitySystemContext asc)
    {
        base.Init(asc);

        growthCalculator = new AttributeGrowthCalculator();
        growthCalculator.RecalculateUpgradePoint();
    }

    public float GetAttributeValue(EAttributeType attributeType)
    {
        if (abilitySystem == null) return 0f;

        return abilitySystem.AttributeSet.GetAttributeValue(attributeType);
    }

    public float GetHealthPercent()
    {
        if (abilitySystem == null) return 0f;

        float currentHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.CurrentHealth);
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);

        return currentHealth / maxHealth;
    }

    public float GetSuccessChance()
    {
        return growthCalculator.CalculateSuccessChance(GetHealthPercent()) * 100f;
    }

    public float GetUpgradeCost()
    {
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);
        return growthCalculator.CalculateCost(maxHealth);
    }

    public int GetUpgradeValue(EAttributeType attribute)
    {
        return attribute switch
        {
            EAttributeType.Strength => growthCalculator.StrPoint,
            EAttributeType.Dexterity => growthCalculator.DexPoint,
            EAttributeType.Intelligence => growthCalculator.IntelliPoint,
            EAttributeType.Vitality => growthCalculator.VitalPoint,
            EAttributeType.SkillPoint => growthCalculator.SkilPoint,
            _ => 0,
        };
    }

    public float GetRelaxValue()
    {
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);
        return growthCalculator.CalculateRelaxPoint(maxHealth);
    }

    public void UpgaradeAttribute(EAttributeType attribute)
    {
        if (growthCalculator.IsSuccessUpgrade(GetHealthPercent()))
        {
            FAttributeModifier upgradeModifier = new()
            {
                attributeType = attribute,
                policy = EModifierPolicy.Instant,
                operation = EModifierOp.Add,
                value = GetUpgradeValue(attribute)
            };
            abilitySystem.ApplyModifier(upgradeModifier);


            FAttributeModifier costModifier = new()
            {
                attributeType = EAttributeType.IncommingDamage,
                policy = EModifierPolicy.Instant,
                operation = EModifierOp.Add,
                value = GetUpgradeCost()
            };
            abilitySystem.ApplyModifier(costModifier);


            FAttributeModifier skillPointModifier = new()
            {
                attributeType = EAttributeType.SkillPoint,
                policy = EModifierPolicy.Instant,
                operation = EModifierOp.Add,
                value = GetUpgradeValue(EAttributeType.SkillPoint)
            };
            abilitySystem.ApplyModifier(skillPointModifier);

            DebugHelper.Log(
                 $"{attribute} +{upgradeModifier.value}, " +
                 $"SP: +{skillPointModifier.value}, " +
                 $"Cost: {costModifier.value}"
             );

            PoolingManager.Instance.ActivateEffect(EEffectType.Healing, abilitySystem.Owner.Transform.position);
        }
        else
        {
            abilitySystem.Owner.Anim.SetTrigger("Hit");
            DebugHelper.Log($"Failed to upgrade {attribute}.");
        }

        growthCalculator.RecalculateUpgradePoint();
        GameManager.Instance.ProcessDay();
    }

    public void Relax()
    {
        FAttributeModifier modifier = new()
        {
            attributeType = EAttributeType.CurrentHealth,
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Add,
            value = GetRelaxValue()
        };

        abilitySystem.ApplyModifier(modifier);

        DebugHelper.Log($"Health +{modifier.value}");

        PoolingManager.Instance.ActivateEffect(EEffectType.Healing, abilitySystem.Owner.Transform.position);
        growthCalculator.RecalculateUpgradePoint();
        GameManager.Instance.ProcessDay();
    }

}
