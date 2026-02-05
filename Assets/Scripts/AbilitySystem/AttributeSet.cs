using System;
using System.Collections.Generic;
using UnityEngine;

public enum EModifierOp
{
    None,
    Add,
    Multiply,
    Override
}

public struct FAttributeModifier
{
    public EAttributeType attributeType;
    public float value;
    public EModifierOp operation; // Add, Mul, Override
    public bool isPermanent; // change base value
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
    // Primary Attribute
    strength,
    dexterity,
    intelligence,
    vitality,

    // Secondary Attribute
    physicalAttackPower,
    magicAttackPower,
    physicalDefensePower,
    magicDefensePower,
    criticalChance,
    maxHealth,
    maxMana,

    // Vital Attribute
    currentHealth,
    currentMana,

    // Meta Attribute
    incommingDamage

}

public interface IAttributeSet
{
    float GetAttributeValue(EAttributeType type);
    event Action OnDaed;
    event Action<EAttributeType, float> OnAttributeChanged;
}

public class AttributeSet : IAttributeSet
{
    // 어트리뷰트에 따른 값을 저장
    private Dictionary<EAttributeType, AttributeValue> attributes = new();
    // 어트리뷰트 별로 적용되어있는 modifer 저장
    private Dictionary<EAttributeType, List<FAttributeModifier>> modifiers = new();
    // 2차 속성 계산을 위한 전략 저장
    private Dictionary<EAttributeType, IAttributeCalculator> calculators = new();

    public event Action OnDaed;
    public event Action<EAttributeType, float> OnAttributeChanged;

    public AttributeSet()
    {
        foreach (EAttributeType type in Enum.GetValues(typeof(EAttributeType)))
        {
            attributes[type] = new AttributeValue(0f);
            modifiers[type] = new List<FAttributeModifier>();
        }
    }
    public void SetBaseValue(EAttributeType type, float value)
    {
        attributes[type].baseValue = value;
        Recalculate(type);
    }

    public float GetBaseValue(EAttributeType type)
    {
        return attributes[type].baseValue;
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

    private void PostAttributeChange(FAttributeModifier modifier)
    {
        if (modifier.attributeType== EAttributeType.currentHealth)
        {
            attributes[modifier.attributeType].baseValue
                = Mathf.Clamp(attributes[modifier.attributeType].baseValue, 0f, GetAttributeValue(EAttributeType.maxHealth));

            Recalculate(EAttributeType.currentHealth);

            if (GetAttributeValue(modifier.attributeType) <= 0)
                OnDaed?.Invoke();
            
        }

        if (modifier.attributeType == EAttributeType.currentMana)
        {
            attributes[modifier.attributeType].baseValue
                = Mathf.Clamp(attributes[modifier.attributeType].baseValue, 0f, GetAttributeValue(EAttributeType.maxMana));

            Recalculate(EAttributeType.currentMana);
        }

        if (modifier.attributeType == EAttributeType.incommingDamage)
        {
            HandleIncomingDamage(modifier);
        }


        OnAttributeChanged?.Invoke(modifier.attributeType, GetAttributeValue(modifier.attributeType));
    }

    private void HandleIncomingDamage(FAttributeModifier modifier)
    {
        float localIncomingDamage = modifier.value;
        SetBaseValue(EAttributeType.incommingDamage, 0);

        FAttributeModifier healthMod;
        healthMod.attributeType = EAttributeType.currentHealth;
        healthMod.value = -localIncomingDamage;
        healthMod.operation = EModifierOp.Add;
        healthMod.isPermanent = true;

        ApplyModifier(healthMod);
    }

    public void ApplyModifier(FAttributeModifier modifier)
    {
        if (modifier.isPermanent)
        {
            ApplyModifierToBaseValue(modifier);
        }
        else
        {
            AddToModifierList(modifier);
        }

        Recalculate(modifier.attributeType);
        PostAttributeChange(modifier);
    }

    private void ApplyModifierToBaseValue(FAttributeModifier modifier)
    {
        switch (modifier.operation)
        {
            case EModifierOp.Add:
                attributes[modifier.attributeType].baseValue += modifier.value;
                break;

            case EModifierOp.Multiply:
                attributes[modifier.attributeType].baseValue *= modifier.value;
                break;

            case EModifierOp.Override:
                attributes[modifier.attributeType].baseValue = modifier.value;
                break;
        }
    }

    private void AddToModifierList(FAttributeModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.attributeType))
            modifiers[modifier.attributeType] = new List<FAttributeModifier>();

        modifiers[modifier.attributeType].Add(modifier);
    }

    public void RemoveModifier(FAttributeModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.attributeType))
            return;

        modifiers[modifier.attributeType].Remove(modifier);
        Recalculate(modifier.attributeType);
        PostAttributeChange(modifier);

    }

    public void ClearModifiers(EAttributeType type)
    {
        if (!modifiers.ContainsKey(type))
            return;

        modifiers[type].Clear();
        Recalculate(type);

        FAttributeModifier mod;
        mod.attributeType = type;
        mod.isPermanent = true;
        mod.value = 0;
        mod.operation = EModifierOp.None;
        PostAttributeChange(mod);
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
}
