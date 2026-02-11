using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public enum EModifierOp
{
    None,
    Add,
    Multiply,
    Override
}

public enum EModifierPolicy
{
    Instant,   // BaseValue 변경
    Duration,
    Infinite 
}

public struct FAttributeModifier
{
    public EAttributeType attributeType;
    public float value;
    public EModifierOp operation; // Add, Mul, Override
    public EModifierPolicy policy;
}

public readonly struct FModifierHandle
{
    public readonly int Id;

    public FModifierHandle(int id)
    {
        Id = id;
    }
}

public class ActiveModifier
{
    public FModifierHandle Handle;
    public FAttributeModifier Modifier;
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
    private Dictionary<EAttributeType, List<ActiveModifier>> modifiers = new();
    Dictionary<FModifierHandle, ActiveModifier> modifierHandleMap = new();
    private int handleCounter = 0;
    // 2차 속성 계산을 위한 전략 저장
    private Dictionary<EAttributeType, IAttributeCalculator> calculators = new();

    public event Action OnDaed;
    public event Action<EAttributeType, float> OnAttributeChanged;

    public AttributeSet()
    {
        foreach (EAttributeType type in Enum.GetValues(typeof(EAttributeType)))
        {
            attributes[type] = new AttributeValue(0f);
            modifiers[type] = new List<ActiveModifier>();
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

    public void InitAttributeCalcualtor()
    {
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
        healthMod.policy = EModifierPolicy.Instant;

        ApplyPermanentModifier(healthMod);
    }

    public void ApplyPermanentModifier(FAttributeModifier modifier)
    {
        ApplyModifierToBaseValue(modifier);
        Recalculate(modifier.attributeType);
        PostAttributeChange(modifier);
    }

    public FModifierHandle ApplyActiveModifier(FAttributeModifier modifier)
    {
        var handle = AddToModifierList(modifier);

        Recalculate(modifier.attributeType);
        PostAttributeChange(modifier);

        return handle;
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

    private FModifierHandle AddToModifierList(FAttributeModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.attributeType))
            modifiers[modifier.attributeType] = new();

        var handle = new FModifierHandle(++handleCounter);
        var active = new ActiveModifier
        {
            Handle = handle,
            Modifier = modifier
        };

        modifiers[modifier.attributeType].Add(active);
        modifierHandleMap.Add(handle, active);

        return handle;
    }

    public void RemoveModifier(FModifierHandle handle)
    {
        if (!modifierHandleMap.TryGetValue(handle, out var active))
            return;

        EAttributeType attribute = active.Modifier.attributeType;
        modifiers[attribute].Remove(active);
        modifierHandleMap.Remove(handle);

        Recalculate(attribute);
        PostAttributeChange(active.Modifier);

    }

    public void ClearModifiers(EAttributeType type)
    {
        if (!modifiers.ContainsKey(type))
            return;

        modifiers[type].Clear();
        Recalculate(type);

        FAttributeModifier mod;
        mod.attributeType = type;
        mod.policy = EModifierPolicy.Instant;
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

    public List<ActiveModifier> GetModifiers(EAttributeType type)
    {
        if (!modifiers.ContainsKey(type))
            return new List<ActiveModifier>();

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
            switch (mod.Modifier.operation)
            {
                case EModifierOp.Add:
                    add += mod.Modifier.value;
                    break;

                case EModifierOp.Multiply:
                    mul *= mod.Modifier.value;
                    break;

                case EModifierOp.Override:
                    hasOverride = true;
                    overrideValue = mod.Modifier.value;
                    break;
            }
        }

        float finalValue = hasOverride ? overrideValue : (baseValue + add) * mul;

        attributes[type].currentValue = finalValue;
    }
}
