using System;

public class UIController_RestArea : BaseCharacterUIController
{
    public event Action UpgradeValueChanged;

    public override void Init(IAbilitySystemContext asc)
    {
        base.Init(asc);
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
        return GameManager.Instance.RuntimeGameState.CurrentGrowthData.SuccessChance * 100f;
    }

    public float GetUpgradeCost()
    {
        return GameManager.Instance.RuntimeGameState.CurrentGrowthData.Cost;
    }

    public int GetUpgradeValue(EAttributeType attribute)
    {
        return attribute switch
        {
            EAttributeType.Strength => GameManager.Instance.RuntimeGameState.CurrentGrowthData.StrPoint,
            EAttributeType.Dexterity => GameManager.Instance.RuntimeGameState.CurrentGrowthData.DexPoint,
            EAttributeType.Intelligence => GameManager.Instance.RuntimeGameState.CurrentGrowthData.IntelliPoint,
            EAttributeType.Vitality => GameManager.Instance.RuntimeGameState.CurrentGrowthData.VitalPoint,
            EAttributeType.SkillPoint => GameManager.Instance.RuntimeGameState.CurrentGrowthData.SkillPoint,
            _ => 0,
        };
    }

    public float GetRelaxValue()
    {
        return GameManager.Instance.RuntimeGameState.CurrentGrowthData.RelexPoint;
    }

    public void UpgaradeAttribute(EAttributeType attribute)
    {
        if (GameManager.Instance.RuntimeGameState.GrowthCalculator.IsSuccessUpgrade(GetHealthPercent()))
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

        ProcessDayAfterUpgrade();
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

        ProcessDayAfterUpgrade();
    }

    private void ProcessDayAfterUpgrade()
    {
        float currentHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.CurrentHealth);
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);
        GameManager.Instance.RuntimeGameState.GenerateGrowthData(currentHealth, maxHealth);

        UpgradeValueChanged?.Invoke();
        GameManager.Instance.ProcessDay();
    }
}
