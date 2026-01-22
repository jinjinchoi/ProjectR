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

       return hasOverride ? overrideValue : (defalutValue + add) * mul;
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