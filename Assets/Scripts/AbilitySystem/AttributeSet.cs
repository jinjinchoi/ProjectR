using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    Strength,
    Dexterity,
    Intelligence,
    Vitality,

    // Secondary Attribute
    PhysicalAttackPower,
    MagicAttackPower,
    PhysicalDefensePower,
    MagicDefensePower,
    CriticalChance,
    MaxHealth,
    MaxMana,

    // Vital Attribute
    CurrentHealth,
    CurrentMana,

    // Meta Attribute
    IncommingDamage,

    SkillPoint

}

public interface IAttributeSet
{
    float GetAttributeValue(EAttributeType type);
    event Action OnDaed;
    event Action<EAttributeType, float> OnAttributeChanged;
    void RemoveModifier(FModifierHandle handle);
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
    // 1차 속성에 따라 변하는 2차 속성 저장
    private readonly Dictionary<EAttributeType /*primary attribute*/, HashSet<EAttributeType> /* dependent attributes */> dependencyMap = new();

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
        calculators = new Dictionary<EAttributeType, IAttributeCalculator>();

        // 계산기 생성
        List<IAttributeCalculator> calculatorList = new()
        {
            new PhysicalAttackPowerCalculator(),
            new PhysicalDefensePowerCalculator(),
            new MagicAttackPowerCalculator(),
            new MagicDefensePowerCalculator(),
            new CriticalChanceCalculator(),
            new MaxHealthChanceCalculator(),
            new MaxManaChanceCalculator()
        };

        // 전략 저장
        foreach (var calculator in calculatorList)
        {
            calculators.Add(calculator.TargetAttribute, calculator);
        }

        // dependencyMap 구조:
        // key   = 어떤 속성이 변경되었는가 (Primary)
        // value = 그 속성에 의존하는 속성들 (Recalculate 대상)
        //
        // 예:
        // Strength → [MaxHealth, PhysicalAttackPower]
        // Vitality → [MaxHealth, PhysicalDefensePower]
        //
        dependencyMap.Clear();
        
        foreach (var calculator in calculators)
        {
            // calculator.Key   = 2차 attribute
            // calculator.Value = 해당 Attribute의 Calculator
            foreach (EAttributeType dependency in calculator.Value.Dependencies)
            {
                // dependency: 현재 Calculator가 의존하고 있는 attribute

                // dependencyMap에 이미 리스트가 존재하는지 확인
                if (!dependencyMap.TryGetValue(dependency, out HashSet<EAttributeType> hasSet))
                {
                    // 없으면 새로 생성
                    hasSet = new HashSet<EAttributeType>();
                    dependencyMap[dependency] = hasSet;
                }

                // dependency가 변경되었을 때 재계산해야 할 TargetAttribute 추가
                hasSet.Add(calculator.Value.TargetAttribute);
            }
        }

#if UNITY_EDITOR
        ValidateNoCircularDependency(calculators.Values);
#endif
    }

    // DFS 사용하여 계산 로직에 순환 있는지 확인
    private void ValidateNoCircularDependency(IEnumerable<IAttributeCalculator> calculators)
    {
        // dependency있는 attribute를 모은 그래프 생성
        var graph = new Dictionary<EAttributeType, List<EAttributeType>>();
        foreach (var calc in calculators)
        {
            graph[calc.TargetAttribute] = calc.Dependencies.ToList();
        }

        // 탐색할 attribute와 방문 상태를 저장하는 Map.
        // 검사할 attribute는 calculator를 통해 생성하는 attribute.
        Dictionary<EAttributeType /* 검사할 attribute*/, int> visitState = new();
        foreach (var node in graph.Keys)
            visitState[node] = 0;

        // 그래프에 존재하는 attribute 순환 있나 검사 시작
        foreach (EAttributeType attribute in graph.Keys)
        {
            if (visitState[attribute] == 0)
            {
                if (HasCycleDFS(attribute, graph, visitState))
                {
                    throw new Exception($"[AttributeSystem] Circular dependency detected at {attribute}");
                }
            }
        }
    }

    // DFS 검사를 통해 그래프에 존재하는 attribute의 자식 노드들을 전부 검사.
    private bool HasCycleDFS(EAttributeType attribute, 
        Dictionary<EAttributeType, List<EAttributeType>> graph, Dictionary<EAttributeType, int> visitState)
    {
        // 현재 검사중인 attribute 표시
        visitState[attribute] = 1;

        // attribute와 관련된 dependencies 확인
        if (graph.TryGetValue(attribute, out List<EAttributeType> dependencies))
        {
            foreach (var dependentAttribute in dependencies)
            {
                // 현재 검사중인 attribute에 존재하는 dependent attribute가 또 dependency를 가지고 있는지 확인.
                if (!graph.ContainsKey(dependentAttribute)) continue;

                // 해당 attribute를 이미 방문했는지 확인
                if (visitState[dependentAttribute] == 1)
                    return true; // 방문했으면 순환 중임

                // 아직 방문 안했으면 방문
                if (visitState[dependentAttribute] == 0)
                {
                    // 재귀함수로 검사 시작
                    if (HasCycleDFS(dependentAttribute, graph, visitState))
                        return true; // 자식 노드들 중 (순환)true가 반환되면 부모에도 전파
                }
            }
        }

        // 모든 자식 노드 확인 후 검사 완료 표시. 공통된 자식을 가질 수 있기 때문에 별도의 표시 통해 재검사 방지.
        visitState[attribute] = 2;
        return false;
    }

    private void PostAttributeChange(FAttributeModifier modifier)
    {

        if (modifier.attributeType == EAttributeType.CurrentHealth)
        {
            attributes[modifier.attributeType].baseValue
                = Mathf.Clamp(attributes[modifier.attributeType].baseValue, 0f, GetAttributeValue(EAttributeType.MaxHealth));

            Recalculate(EAttributeType.CurrentHealth);

            if (GetAttributeValue(modifier.attributeType) <= 0)
                OnDaed?.Invoke();
        }

        if (modifier.attributeType == EAttributeType.CurrentMana)
        {
            attributes[modifier.attributeType].baseValue
                = Mathf.Clamp(attributes[modifier.attributeType].baseValue, 0f, GetAttributeValue(EAttributeType.MaxMana));

            Recalculate(EAttributeType.CurrentMana);
        }

        if (modifier.attributeType == EAttributeType.IncommingDamage)
        {
            HandleIncomingDamage(modifier);
        }

        OnAttributeChanged?.Invoke(modifier.attributeType, GetAttributeValue(modifier.attributeType));
        ProcessDirty(modifier);
    }


    // attribute 변경 후 그와 연관된 attribute 있으면 재계산
    public void ProcessDirty(FAttributeModifier modifier)
    {
        if (dependencyMap.TryGetValue(modifier.attributeType, out var dependencies))
        {
            foreach (var dependency in dependencies)
            {
                CalculateDependentAttribute(dependency);
                Recalculate(dependency);
                OnAttributeChanged?.Invoke(dependency, GetAttributeValue(dependency));
            }
        }
    }

    private void CalculateDependentAttribute(EAttributeType type)
    {
        if (calculators.TryGetValue(type, out IAttributeCalculator calculator))
        {
            attributes[type].baseValue = calculator.GetAttributeValue(this);
        }
    }

    private void HandleIncomingDamage(FAttributeModifier modifier)
    {
        float localIncomingDamage = modifier.value;
        SetBaseValue(EAttributeType.IncommingDamage, 0);

        FAttributeModifier healthMod;
        healthMod.attributeType = EAttributeType.CurrentHealth;
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

    public FModifierHandle ApplyOngoingModifier(FAttributeModifier modifier)
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

    public void ClearAllModifiers()
    {
        modifiers.Clear();
    }

    public float GetAttributeValue(EAttributeType type)
    {
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

        attributes[type].currentValue = Mathf.Round(finalValue);
    }
}
