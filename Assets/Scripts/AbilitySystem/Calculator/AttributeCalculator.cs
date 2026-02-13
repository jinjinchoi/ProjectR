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
      => EAttributeType.physicalAttackPower;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.strength,
        EAttributeType.dexterity
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.strength);
        float dex = attributeSet.GetAttributeValue(EAttributeType.dexterity);

        return 1f + str + (0.2f * dex);
    }
}

public class PhysicalDefensePowerCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.physicalDefensePower;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.strength,
        EAttributeType.vitality
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.strength);
        float vit = attributeSet.GetAttributeValue(EAttributeType.vitality);

        return 5f + (0.5f * vit) + (0.2f * str);
    }
}

public class MagicAttackPowerCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.magicAttackPower;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.intelligence,
        EAttributeType.dexterity
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float intell = attributeSet.GetAttributeValue(EAttributeType.intelligence);
        float dex = attributeSet.GetAttributeValue(EAttributeType.dexterity);

        return 2f + intell + (0.1f * dex);
    }
}

public class MagicDefensePowerCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.magicDefensePower;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.intelligence,
        EAttributeType.vitality
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float intell = attributeSet.GetAttributeValue(EAttributeType.intelligence);
        float vit = attributeSet.GetAttributeValue(EAttributeType.vitality);

        return 5f + 0.5f * vit + 0.2f * intell;
    }
}

public class CriticalChanceCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.criticalChance;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    { 
        EAttributeType.strength,
        EAttributeType.dexterity
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.strength);
        float dex = attributeSet.GetAttributeValue(EAttributeType.dexterity);

        float cirticalChance = 5f + (0.25f * dex) + (0.05f * str);

        return Mathf.Clamp(cirticalChance, 0f, 100f);
    }
}

public class MaxHealthChanceCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.maxHealth;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    {
        EAttributeType.strength,
        EAttributeType.vitality
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.strength);
        float vit = attributeSet.GetAttributeValue(EAttributeType.vitality);

        return 100f + (5f * vit) + (0.5f * str);
    }
}

public class MaxManaChanceCalculator : AttributeCalculatorBase
{
    public override EAttributeType TargetAttribute => EAttributeType.maxMana;

    public override IReadOnlyList<EAttributeType> Dependencies => new[]
    { 
        EAttributeType.vitality,
        EAttributeType.intelligence
    };

    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float vit = attributeSet.GetAttributeValue(EAttributeType.vitality);
        float intell = attributeSet.GetAttributeValue(EAttributeType.intelligence);

        return 50f + (3f * intell) + (1f * vit);
    }
}