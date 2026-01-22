using Unity.Mathematics;

public interface IAttributeCalculator
{
    float GetAttributeValue(AttributeSet attributeSet, EAttributeType type);
}

// 2차 속성 계산을 위한 클래스. 클래스 생성 후 Attribute Set 클래스에서 적용해야함.
public abstract class AttributeCalculatorBase : IAttributeCalculator
{
    protected abstract float CalculateAttribute(AttributeSet attributeSet);

    public float GetAttributeValue(AttributeSet attributeSet, EAttributeType type)
    {
        float defalutValue = CalculateAttribute(attributeSet);

        float add = 0f;
        float mul = 1f;
        bool hasOverride = false;
        float overrideValue = 0f;

        foreach (var mod in attributeSet.GetModifiers(type))
        {
            switch (mod.operation)
            {
                case EModifierOp.Add:
                    add += mod.value;
                    break;

                case EModifierOp.Multiply:
                    mul *= mod.value;
                    break;

                case EModifierOp.Override:
                    hasOverride = true;
                    overrideValue = mod.value;
                    break;
            }
        }

        return math.round(hasOverride ? overrideValue : (defalutValue + add) * mul);
    }
}

public class PhysicalAttackPowerCalculator : AttributeCalculatorBase
{
    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.strength);
        float dex = attributeSet.GetAttributeValue(EAttributeType.dexterity);

        return 10f + (3f * str) + dex;
    }
}

public class PhysicalDefensePowerCalculator : AttributeCalculatorBase
{
    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.strength);
        float vit = attributeSet.GetAttributeValue(EAttributeType.vitality);

        return 5f + (0.5f * vit) + (0.2f * str);
    }
}

public class MagicAttackPowerCalculator : AttributeCalculatorBase
{
    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float intell = attributeSet.GetAttributeValue(EAttributeType.intelligence);
        float dex = attributeSet.GetAttributeValue(EAttributeType.dexterity);

        return 10f + (3.2f * intell) + (0.5f * dex);
    }
}

public class MagicDefensePowerCalculator : AttributeCalculatorBase
{
    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float intell = attributeSet.GetAttributeValue(EAttributeType.intelligence);
        float vit = attributeSet.GetAttributeValue(EAttributeType.vitality);

        return 5f + 0.5f * vit + 0.2f * intell;
    }
}

public class CriticalChanceCalculator : AttributeCalculatorBase
{
    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.strength);
        float dex = attributeSet.GetAttributeValue(EAttributeType.dexterity);

        float cirticalChance = 5f + (0.25f * dex) + (0.05f * str);

        return math.clamp(cirticalChance, 0f, 100f);
    }
}

public class MaxHealthChanceCalculator : AttributeCalculatorBase
{
    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float str = attributeSet.GetAttributeValue(EAttributeType.strength);
        float vit = attributeSet.GetAttributeValue(EAttributeType.vitality);

        return 100f + (30f * vit) + (5f * str);
    }
}

public class MaxManaChanceCalculator : AttributeCalculatorBase
{
    protected override float CalculateAttribute(AttributeSet attributeSet)
    {
        float vit = attributeSet.GetAttributeValue(EAttributeType.vitality);
        float intell = attributeSet.GetAttributeValue(EAttributeType.intelligence);

        return 50f + (20f * intell) + (5f * vit);
    }
}