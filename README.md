## 01. 목차

---

## 02. 개요

- **프로젝트 소개**  
유니티 2D게임으로 우마무스메와 유사한 방식으로 캐릭터를 육성하고 적을 물리치는 게임.
- **개발 인원**  
  1인 개발
- **사용 엔진**  
유니티 엔진 6
- **작업 기간**  
  1월 19일 ~ 2월 23일 (약 1달)

---

## 03. 프로젝트 요약
  
  - **게임 특징**  
  날짜별로 캐릭터를 육성하고 자동 전투를 통해 진행을 하는 게임으로 언리얼 엔진에 존재하는 프레임 워크인 Gameplay Ability System을 모방하여 Attribute(능력치)와 Ability(스킬)를 구현.

  - **주요기능 요약**
    - **Gameplay Ability System**: 캐릭터의 능력치를 저장하는 Attribute Set 클래스와 캐릭터의 스킬을 저장하는 Ability Component를 구현.
    - **캐릭터 강화**: Attribute를 올려 강화가 가능하며 이때 영구적 상승과 일시적 상승을 구분하여 버프나 아이템 장착 등에 대응이 가능하도록 구현.
    - **Event System**: 날짜에 다른 정규 이벤트와 날짜와 상관없는 랜덤 이벤트를 구현하여 전투에 들어가거나 캐릭터 능력치에 영향을 끼치도록 구현.
    - **Dialgoue System**: 노드 기반의 다이얼로그 시스템 구현.
    - **Save System**: 날짜 및 능력치와 습득한 스킬을 저장 및 로드 가능하게 구현.
    - **FSM**: Finite State Machine을 통해 캐릭터의 이동과 전투 등을 구현.
    - **Object Pooling**: 자주 사용하는 이펙트나 발사체는 풀링을 사용하여 최적화.

---

## 04. 핵심 기능 및 구현 내용

### 01. Gameplay Ability System
**GAS**는 언리얼 엔진에 존재하는 프레임 워크를 모방하여 제작하였습니다. GAS에는 어빌리티를 부여하여 스킬들을 사용할 수 있고 Attribute Set 클래스가 존재하여 이곳에서 능력치를 설정합니다.

캐릭터 클래스는 이 GAS 컴포넌트를 가지고 있어 캐릭터 마다 고유의 스킬과 능력치를 가지게 됩니다.

#### 1) Attribute(능력치)
attribute는 아래와 같이 설정됩니다.

```c#
// Attribute의 값을 저장하는 클래스
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

// Attribute Set 클래스 안에 있는 Attribute 별 값을 저장하는 Map
private Dictionary<EAttributeType, AttributeValue> attributes = new();
```
enum을 통해 attribute를 구분합니다.

attribute는 **baseValue**와 **currentValue**로 구분 되는데 base value는 기본적인 값이고 current value는 기본 값에 modifier에 따른 값을 더한 최종적인 실제 값을 나타냅니다.

둘을 구분한 이유는 영구적인 능력치 변화와 일시적인 능력치 변화를 구분하기 위해 분리하였습니다.


#### 2) Modifier
modifier는 능력치를 설정하기 위해 사용하는 구조체입니다. 구조체에 값을 설정하여 원하는대로 능력치를 강화할 수 있게 됩니다.

```c#
// Attribute를 강화할 때 사용하는 구조체
public struct FAttributeModifier
{
    // 강화할 attribute의 타입 (str, dex, vit....)
    public EAttributeType attributeType;

    // 강화 할 값
    public float value;

    // 강화할 방식 Add(기존 값에서 더하기), Multiplier(기존 값에서 곱하기), Override(덮어쓰기)
    public EModifierOp operation;

    // instant( 불변), duration(일정 기간 동안), infinite(영구적이나 제거 가능)
    public EModifierPolicy policy;
}

// 활성화 되어 있는 modifier를 id로 구분하고 저장하여 추후 제거가 가능하게 구현
public class ActiveModifier
{
    public FModifierHandle Handle;
    public FAttributeModifier Modifier;
}

 // 어트리뷰트 별로 적용되어있는 modifer 저장
 private Dictionary<EAttributeType, List<ActiveModifier>> modifiers = new();
```
각각의 modifier에는 policy가 존재하는데 이 중 **Instant** 정책은 base value를 바꾸고 나머지 정책은 **ActiveModifier**에 저장되어 current value를 계산할 때 사용합니다.

modifier의 정책을 구분함으로써 강화하려는 능력치를 영구적으로 설정할지 일시적으로 설정할지 구분이 가능해집니다.

#### 03) modifier의 적용 및 제거
능력치는 Set 함수가 아닌 오직 modifier를 통해서만 바꾸도록 구현하였습니다.

`FAttributeModifier` 구조체를 만들어 ASC에 전해줍니다. ASC에서는 policy를 구분하여 각각에 맞는 함수를 호출합니다.

```c#
// ASC 클래스에 존재하는 함수로 modifier를 적용
public FModifierHandle? ApplyModifier(FAttributeModifier modifier)
{
    if (modifier.policy == EModifierPolicy.Instant)
    {
        attributeSet.ApplyPermanentModifier(modifier);
        return null;
    }
    else
    {
        return attributeSet.ApplyOngoingModifier(modifier);
    }
}

///////// 이하 attribute set

// base value를 변경
public void ApplyPermanentModifier(FAttributeModifier modifier)
{
    ApplyModifierToBaseValue(modifier);
    Recalculate(modifier.attributeType);
    PostAttributeChange(modifier);
}

// modifier를 attribute에 맞는 map에 추가
public FModifierHandle ApplyOngoingModifier(FAttributeModifier modifier)
{
   // modifier를 map에 추가하여 추후 사용
    var handle = AddToModifierList(modifier);

    Recalculate(modifier.attributeType);
    PostAttributeChange(modifier);

    return handle;
}
```
여기서 **FModifierHandle**에는 id가 저장되어 있어 modifier의 적용을 요청한 클래스에서 이 handle을 보관했다가 제거할 때 사용할 수 있습니다.

modifier를 구분하고 제거하는 방식은 다른 클래스에서 아이디를 직접 설정하고 이후 제거할 때 사용하는 방식도 가능하겠지만 사용하는 클래스마다 서로 다른 아이디를 설정하는 것은 번거롭고 반복적인 작업이기 때문에 비효율적이라 생각하여 attribute set 클래스에서 아이디를 설정하고 반환하는 방식으로 구현하게 되었습니다.

적용된 modifier는 attribute를 계산할때 사용됩니다.

```c#
private void Recalculate(EAttributeType type)
{

  float baseValue = attributes[type].baseValue;

  float add = 0f;
  float mul = 1f;
  bool hasOverride = false;
  float overrideValue = 0f;

  // attribute type별로 저장된 modifier를 가져와 특성에 맞게 값 설정
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

  // base value에 modifier 값을 더하여 최종값 설정
  float finalValue = hasOverride ? overrideValue : (baseValue + add) * mul;

  attributes[type].currentValue = Mathf.Round(finalValue);
}
```
modifier를 적용할 때 마다 이 `Recalculate`함수를 사용하여 current value를 설정합니다.

#### 04) 2차 attribute 계산
위와 같은 방식으로 1차 attribute를 설정하면 이어서 2차 attribute가 설정됩니다. 물론 modifier를 통해 2차 attribute 또한 설정이 가능합니다.

2차 attribute는 오직 플레이어에게만 유효한 attribute로 에너미의 경우 1차 attribute를 통해 2차 attribute를 설정하는 것은 굉장히 비효율적이고 밸런싱 작업도 힘들기 때문에 직접 2차 attribute를 설정하는 방식으로 구현하였습니다.


**전략 패턴을 통한 attribute 설정**  
2차 attribute는 `AttributeCalculator` 클래스에서 계산합니다.

```c#
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
```

각각의 attribute를 담당하는 계산기 클래스는 위의 베이스 클래스를 상속받아 만들어집니다.

```c#
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
```

계산기 클래스의 예시 중 하나로 attribute set에서 1차 속성을 가져와 2차 속성을 설정합니다. 계산 클래스를 사용함으로써 다양한 2차 속성을 원하는대로 자유롭게 설정할 수 있게 됩니다.

```c#
private Dictionary<EAttributeType, IAttributeCalculator> calculators = new();

// 계산기 생성 함수
public void InitAttributeCalcualtor()
{
    // 전략 저장
    calculators.Clear();
    calculators = new Dictionary<EAttributeType, IAttributeCalculator>()
    {
        { EAttributeType.PhysicalAttackPower, new PhysicalAttackPowerCalculator() },
        { EAttributeType.PhysicalDefensePower, new PhysicalDefensePowerCalculator() },
        { EAttributeType.MagicAttackPower, new MagicAttackPowerCalculator() },
        { EAttributeType.MagicDefensePower, new MagicDefensePowerCalculator() },
        { EAttributeType.CriticalChance, new CriticalChanceCalculator() },
        { EAttributeType.MaxHealth, new MaxHealthChanceCalculator() },
        { EAttributeType.MaxMana, new MaxManaChanceCalculator() },
    };

  // ... (dependency 설정 부분 생략)
}

  private void CalculateDependentAttribute(EAttributeType type)
  {
      if (calculators.TryGetValue(type, out IAttributeCalculator calculator))
      {
          attributes[type].baseValue = calculator.GetAttributeValue(this);
      }
  }
```

이후 각각 계산기 클래스들은 attribute 별로 저장되어 1차 속성이 변할 때 그에 따라 변화할 2차 속성의 값을 계산기에서 가져와 값을 설정합니다.

필요할 때마다 클래스를 바꾸어 사용하는 전략 패턴을 통해 switch 문이나 if문 같은 복잡하고 가독성이 떨어지는 함수가 아니라 보기 아주 간단한 로직을 통해 2차 attribute의 값을 가져올 수 있게 되었습니다.

#### 05) defendancy 설정
2차 attribute가 어떤 1차 attribute에 의존하는지 알아야 1차 attribute가 변할때 그에 맞는 2차 attribute를 설정할 수 있을 것입니다. 그렇기에 계산기 클래스에는 해당 계산기가 의존하고 있는 attribute를 설정하여 이를 알 수 있게 하였습니다.

```c#
// 이 계산기가 관여하는 attribute
public override EAttributeType TargetAttribute
  => EAttributeType.PhysicalAttackPower;

// 이 계산기에서 사용(의존)할 attribute
public override IReadOnlyList<EAttributeType> Dependencies => new[]
{
    EAttributeType.Strength,
    EAttributeType.Dexterity
};
```

이렇게 계산기에 설정이 완료되면 위에 계산기 생성 함수에서 map을 통해 attribute별로 의존하고 있는 attribute를 설정합니다.

```c#
// InitAttributeCalcualtor 함수에서 의존성 설정하는 부분

foreach (var calculator in calculators)
{
  // calculator.Key   = TargetAttribute
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

////// attribute set 클래스에 존재하는 의존성 저장하는 map
private readonly Dictionary<EAttributeType /*primary attribute*/, HashSet<EAttributeType> /* dependent attributes */> dependencyMap = new();
```
dependencyMap은 attribute별로 의존하고 있는 다른 attribute를 저장하는 map입니다. 

```c#
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
```

modifier를 통해 모든 attribute의 계산이 끝나면 의존성 map을 순회하여 변경할 attribute가 있는지 확인하고 값을 설정하게 됩니다.

**DFS 통한 순환 참조 예방**  
이렇게 Dependency를 이용할때 가장 주의해야할 점은 순환을 방지하는 것일겁니다. 만약 A attribute가 B attribute에 의존하고 있는데 그 반대도 마찬가지이면 ProcessDirty 함수가 영원히 실행되는 문제가 생길 것입니다.

비록 이 프로젝트에서는 모든 2차 attribute가 오직 1차 attribute에만 의존하고 있기 때문에 순환이 생길 일은 없지만 포괄적이고 재사용이 가능한 시스템 구현을 위하여 순환 방지 대책을 구현하였습니다.

```c#
// 계산기 생성 함수에서 실행
#if UNITY_EDITOR
        ValidateNoCircularDependency(calculators.Values);
#endif

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
private bool HasCycleDFS(EAttributeType attribute, Dictionary<EAttributeType, List<EAttributeType>> graph, Dictionary<EAttributeType, int> visitState)
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

    // 모든 자식 노드 확인 후 검사 완료 표시. 공통 자식을 가질 수 있기 때문에 별도의 표시 통해 재검사 방지.
    visitState[attribute] = 2;
    return false;
}
```
노드를 검사하여 모든 재귀함수 호출이 끝나지 않았는데 같은 현재 검사 중인 노드를 만나게 되면 순환이 있는 것이고 이는 잘못된 것이기 때문에 경고하여 다시 작성할 수 있게 구현하였습니다.

이때 노드의 순환 여부는 실제 빌드된 게임에서는 알아도 별 의미가 없기 때문에 유니티 엔진에서만 실행되도록 하였습니다.

