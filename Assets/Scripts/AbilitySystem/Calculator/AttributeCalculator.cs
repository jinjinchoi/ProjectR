using System.Collections.Generic;
using UnityEngine;

public interface IAttributeCalculator
{
    float GetAttributeValue(AttributeSet attributeSet, EAttributeType type);
    public abstract EAttributeType TargetAttribute { get; }
    public abstract IReadOnlyList<EAttributeType> Dependencies { get; }
}

// 2차 속성 계산을 위한 클래스. 클래스 생성 후 Attribute Set 클래스에서 적용해야함.
public abstract class AttributeCalculatorBase : IAttributeCalculator
{
    // 현재 계산기가 담당하고 있는 attribute
    public abstract EAttributeType TargetAttribute { get; }

    // 계산기가 담당하는 attribute를 올리기 위해 필요한 attribute를 저장하는 list
    public abstract IReadOnlyList<EAttributeType> Dependencies { get; }

    protected abstract float CalculateAttribute(AttributeSet attributeSet);

    public float GetAttributeValue(AttributeSet attributeSet, EAttributeType type)
    {
        return CalculateAttribute(attributeSet);
    }
}

public class PhysicalAttackPowerCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute
      => EAttributeType.PhysicalAttackPower;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.Strength,
        EAttributeType.Dexterity
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.Strength);
        float dex = attributeSet.GetAttributeValue(EAttributeType.Dexterity);

        return 1f + str + (0.2f * dex);
    }
}

public class PhysicalDefensePowerCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.PhysicalDefensePower;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.Strength,
        EAttributeType.Vitality
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.Strength);
        float vit = attributeSet.GetAttributeValue(EAttributeType.Vitality);

        return 5f + (0.5f * vit) + (0.2f * str);
    }
}

public class MagicAttackPowerCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.MagicAttackPower;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.Intelligence,
        EAttributeType.Dexterity
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float intell = attributeSet.GetAttributeValue(EAttributeType.Intelligence);
        float dex = attributeSet.GetAttributeValue(EAttributeType.Dexterity);

        return 2f + intell + (0.1f * dex);
    }
}

public class MagicDefensePowerCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.MagicDefensePower;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.Intelligence,
        EAttributeType.Vitality
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float intell = attributeSet.GetAttributeValue(EAttributeType.Intelligence);
        float vit = attributeSet.GetAttributeValue(EAttributeType.Vitality);

        return 5f + 0.5f * vit + 0.2f * intell;
    }
}

public class CriticalChanceCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.CriticalChance;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    { 
        EAttributeType.Strength,
        EAttributeType.Dexterity
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.Strength);
        float dex = attributeSet.GetAttributeValue(EAttributeType.Dexterity);

        float cirticalChance = 5f + (0.25f * dex) + (0.05f * str);

        return Mathf.Clamp(cirticalChance, 0f, 100f);
    }
}

public class MaxHealthChanceCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.MaxHealth;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.Strength,
        EAttributeType.Vitality
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.Strength);
        float vit = attributeSet.GetAttributeValue(EAttributeType.Vitality);

        return 100f + (5f * vit) + (0.5f * str);
    }
}

public class MaxManaChanceCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.MaxMana;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    { 
        EAttributeType.Vitality,
        EAttributeType.Intelligence
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float vit = attributeSet.GetAttributeValue(EAttributeType.Vitality);
        float intell = attributeSet.GetAttributeValue(EAttributeType.Intelligence);

        return 50f + (3f * intell) + (1f * vit);
    }
}