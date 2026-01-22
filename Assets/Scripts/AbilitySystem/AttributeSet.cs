using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum EModifierOp
{
    Add,
    Multiply,
    Override
}

public struct FAttributeModifier
{
    public EAttributeType attribute;
    public float value;
    public EModifierOp operation; // Add, Mul, Override
}

public class AttributeValue
{
    public float baseValue;
    public float currentValue;

    public AttributeValue(float baseValue)
    {
        this.baseValue = baseValue;
        this.currentValue = baseValue;
    }
}

public enum EAttributeType
{
    strength,
    dexterity,
    intelligence,
    vitality,

    physicalAttackPower,
    magicAttackPower,
    physicalDefensePower,
    magicDefensePower,
    criticalChance,
    maxHealth,
    maxMana,

    currentHealth,
    currentMana

}

public class AttributeSet
{
    // 어트리뷰트에 따른 값을 저장
    private Dictionary<EAttributeType, AttributeValue> attributes = new();
    // 어트리뷰트 별로 적용되어있는 modifer 저장
    private Dictionary<EAttributeType, List<FAttributeModifier>> modifiers = new();
    // 2차 속성 계산을 위한 전략 저장
    private Dictionary<EAttributeType, IAttributeCalculator> calculators = new();

    public AttributeSet()
    {
        foreach (EAttributeType type in Enum.GetValues(typeof(EAttributeType)))
        {
            attributes[type] = new AttributeValue(0f);
            modifiers[type] = new List<FAttributeModifier>();
            Debug.Log(type);

        }
    }

    public void InitAttribute(EAttributeType type, float baseValue)
    {
        if (attributes.ContainsKey(type))
        {
            attributes[type].baseValue = baseValue;
            attributes[type].currentValue = baseValue;
        }

        // 계산기에 전략 저장
        calculators = new Dictionary<EAttributeType, IAttributeCalculator>()
        {
            { EAttributeType.physicalAttackPower, new PhysicalAttackPowerCalculator() },
            { EAttributeType.physicalDefensePower, new PhysicalDefensePowerCalculator() },
            { EAttributeType.magicAttackPower, new MagicAttackPowerCalculator() },
            { EAttributeType.magicDefensePower, new MagicDefensePowerCalculator() },
            { EAttributeType.criticalChance, new CriticalChanceCalculator() },
            { EAttributeType.maxHealth, new MaxHealthChanceCalculator() },
            { EAttributeType.maxMana, new MaxManaChanceCalculator() },
        };
    }

    public void SetBaseValue(EAttributeType type, float value)
    {
        attributes[type].baseValue = value;
    }

    public float GetBaseValue(EAttributeType type)
    {
        return attributes[type].baseValue;
    }

    public void AddModifier(FAttributeModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.attribute))
            modifiers[modifier.attribute] = new List<FAttributeModifier>();

        modifiers[modifier.attribute].Add(modifier);
        Recalculate(modifier.attribute);
        PostAttributeChange(modifier.attribute);
    }

    public void RemoveModifier(FAttributeModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.attribute))
            return;

        modifiers[modifier.attribute].Remove(modifier);
        Recalculate(modifier.attribute);
        PostAttributeChange(modifier.attribute);

    }

    public void ClearModifiers(EAttributeType type)
    {
        if (!modifiers.ContainsKey(type))
            return;

        modifiers[type].Clear();
        Recalculate(type);
        PostAttributeChange(type);
    }

    public float GetAttributeValue(EAttributeType type)
    {
        if (calculators.TryGetValue(type, out IAttributeCalculator calculator))
        {
            return calculator.GetAttributeValue(this, type);
        }

        return attributes[type].currentValue;
    }

    public List<FAttributeModifier> GetModifiers(EAttributeType type)
    {
        if (!modifiers.ContainsKey(type))
            return new List<FAttributeModifier>();

        return modifiers[type];
    }

    private void Recalculate(EAttributeType type)
    {
        float baseValue = attributes[type].baseValue;

        float add = 0f;
        float mul = 1f;
        bool hasOverride = false;
        float overrideValue = 0f;

        foreach (var mod in modifiers[type])
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

        float finalValue = hasOverride ? overrideValue : (baseValue + add) * mul;

        attributes[type].currentValue = finalValue;
    }

    private void PostAttributeChange(EAttributeType changedAttribute)
    {
        if (changedAttribute == EAttributeType.currentHealth)
        {
            attributes[changedAttribute].currentValue 
                = math.clamp(attributes[changedAttribute].currentValue, 0f, GetAttributeValue(EAttributeType.maxHealth));
        }

        if (changedAttribute == EAttributeType.currentMana)
        {
            attributes[changedAttribute].currentValue
                = math.clamp(attributes[changedAttribute].currentValue, 0f, GetAttributeValue(EAttributeType.maxMana));
        }

    }
}
