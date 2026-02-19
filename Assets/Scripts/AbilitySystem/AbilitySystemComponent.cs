using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// onwer의 정보를 가져오는 인터페이스
public interface IAbilityOwner
{
    Animator Anim { get; }
    Transform Transform { get; }
    AnimationTrigger AnimationTrigger { get; }
    Transform AttackPoint { get; }
    EFaction Faction { get; }
    int FacingDir { get; }
}

// 어빌리티가 직접 ASC에 접근하지 않고도 ASC의 기능을 사용하게 해주는 인터페이스
public interface IAbilitySystemContext
{
    IAbilityOwner Owner { get; }
    IAttributeSet AttributeSet { get; }

    void EndAbility(AbilitySpec spec);
    Coroutine StartCoroutine(IEnumerator routine);
    void StopCoroutine(Coroutine routine);
    void RegisterWaitingAbility(EAnimationEventType eventType, AbilitySpec spec, Action callback);
    void UnregisterWaitingAbility(AbilitySpec spec);
    FModifierHandle? ApplyModifier(FAttributeModifier modifier);
    EAbilityId GetRandomAbilityId();
    event Action<BuffUIData> OnBuffActivated;
    event Action<EAbilityId> OnBuffDeactivated;

    void RaiseOnBuffActivated(BuffUIData buffUIData);
    void RaiseOnBuffDeactivated(EAbilityId id);
}

// 어빌리티가 애니메이션을 기다릴 때 콜백을 저장하는 클래스
public class WaitingAbilityEntry
{
    public AbilitySpec spec;
    public Action callback;

    public WaitingAbilityEntry(AbilitySpec spec, Action callback)
    {
        this.spec = spec;
        this.callback = callback;
    }
}

public class AbilitySystemComponent : MonoBehaviour, IAbilitySystemContext
{
    public event Action<EAbilityId> OnAbilityEnded;
    public event Action<BuffUIData> OnBuffActivated;
    public event Action<EAbilityId> OnBuffDeactivated;

    public IAbilityOwner Owner => owner;
    public IAttributeSet AttributeSet
    {
        get
        {
            if (attributeSet == null)
            {
                attributeSet = new AttributeSet();
                attributeSet.InitAttributeCalcualtor();
            }
            return attributeSet;
        }
    }

    public void RaiseOnBuffActivated(BuffUIData buffUIData)
    {
        OnBuffActivated?.Invoke(buffUIData);
    }

    public void RaiseOnBuffDeactivated(EAbilityId id)
    {
        OnBuffDeactivated?.Invoke(id);
    }

    /*
     * abilities :현재 보유하고 있는 ability list,
     * activeAbilitySpecs :실행중인 ability map
     * waitingAbilitiesByAnimEvent: anim event를 기다리고 있는 ability map
     */
    private readonly List<AbilitySpec> abilities = new(); // 최적화 위해서는 Set으로 바꿔야함.
    private readonly Dictionary<EAbilityId, AbilitySpec> activatedAbilitySpecs = new();
    private readonly Dictionary<EAnimationEventType, List<WaitingAbilityEntry>> waitingAbilitiesByAnimEvent = new();

    private AttributeSet attributeSet;
    private IAbilityOwner owner;

    private void OnDisable()
    {
        if (owner != null && owner.AnimationTrigger != null)
            Owner.AnimationTrigger.OnAnimTriggered -= OnAnimationTriggered;

        attributeSet?.ClearAllModifiers();
    }


    public void Init(IAbilityOwner owner)
    {
        if (attributeSet == null)
        {
            attributeSet = new AttributeSet();
            attributeSet.InitAttributeCalcualtor();
        }
        this.owner = owner;
        Owner.AnimationTrigger.OnAnimTriggered += OnAnimationTriggered;
    }

    public void GiveAbility(BaseAbilityDataSO data)
    {
        abilities.Add(new AbilitySpec(data, this));
    }

    public void TryActivateAbilityById(EAbilityId abilityId)
    {
        if (Owner == null)
        {
            Debug.LogError($"Owner is not set on {gameObject.name}", this);
            OnAbilityEnded?.Invoke(abilityId);
            return;
        }

        if (activatedAbilitySpecs.ContainsKey(abilityId))
        {
            return;
        }

        AbilitySpec spec = abilities.Find(a => a.abilityData.abilityId == abilityId);
        if (spec != null && spec.ability.CanActivate(spec, this))
        {
            spec.ability.ActivateAbility(spec, this);
            activatedAbilitySpecs.Add(spec.abilityData.abilityId, spec);
        }
    }
    public void EndAbility(AbilitySpec spec)
    {
        EndAbilityBySpec(spec);
    }

    private void EndAbilityBySpec(AbilitySpec spec)
    {
        if (!activatedAbilitySpecs.ContainsKey(spec.abilityData.abilityId))
            return;

        UnregisterWaitingAbility(spec);
        spec.ability.OnEndAbility(spec, this);
        activatedAbilitySpecs.Remove(spec.abilityData.abilityId);

        OnAbilityEnded?.Invoke(spec.abilityData.abilityId);

    }

    public void RegisterWaitingAbility(EAnimationEventType eventType, AbilitySpec spec, Action callback)
    {
        // list가 존재하지 않으면 생성해서 map에 추가
        if (!waitingAbilitiesByAnimEvent.TryGetValue(eventType, out List<WaitingAbilityEntry> list))
        {
            list = new List<WaitingAbilityEntry>();
            waitingAbilitiesByAnimEvent.Add(eventType, list);
        }

        // list에 spec이 이미 있는지 확인. (중복 방지)
        if (list.Exists(e => e.spec == spec))
            return;

        list.Add(new WaitingAbilityEntry(spec, callback));
    }

    public void UnregisterWaitingAbility(AbilitySpec spec)
    {
        // value가 없는 EAnimationEventType를 모으는 배열.
        List<EAnimationEventType> emptyKeys = new();

        foreach (var pair in waitingAbilitiesByAnimEvent)
        {
            pair.Value.RemoveAll(e => e.spec == spec);

            if (pair.Value.Count == 0)
                emptyKeys.Add(pair.Key); // 더이상 value가 없는 EventType을 저장.
        }

        // Map에서 value가 없는 EventType 제거.
        foreach (EAnimationEventType key in emptyKeys)
            waitingAbilitiesByAnimEvent.Remove(key);
    }

    public FModifierHandle? ApplyModifier(FAttributeModifier modifier)
    {
        if (attributeSet == null)
        {
            DebugHelper.LogWarning("attribute set not exist");
            return null;
        }

        if (modifier.policy == EModifierPolicy.Instant)
        {
            attributeSet.ApplyPermanentModifier(modifier);
            return null;
        }
        else
        {
            return attributeSet.ApplyActiveModifier(modifier);
        }
    }

    public bool HasAbility(EAbilityId id)
    {
        return abilities.Exists(a => a.abilityData.abilityId == id);
    }

    /// <summary>
    /// 랜덤한 어빌리티의 아이디를 반환해주는 함수로 단순히 랜덤 인덱스를 뽑으면 쿨다운이나 실행중일 수 있기 때문에
    /// 셔플 후 순서대로 실행여부 확인하여 아이디 반환.
    /// 다만 랜덤 인덱스로 뽑고 유효할때까지 while문 반복하는게 더 좋을 수 있음.
    /// </summary>
    /// <returns></returns>
    public EAbilityId GetRandomAbilityId()
    {
        if (abilities.Count == 0)
            return EAbilityId.None;

        // 인덱스 리스트 생성
        List<int> indices = new();
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].abilityData.abilityId == EAbilityId.NormalAttack)
                continue;

            indices.Add(i);
        }

        // Fisher–Yates Shuffle Algorithm (무작위 순열 생성 알고리즘)
        // 배열의 마지막 요소로부터 시작  // 배열의 첫번째 요소에 도착할 때까지 반복
        for (int i = indices.Count - 1; i > 0; i--)
        {
            // 0부터 현재 인덱스(i) 사이에서 무작위 정수 j를 선택
            int j = UnityEngine.Random.Range(0, i + 1);
            // i와 j의 위치를 서로 변경
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        // 순서대로 검사
        foreach (int index in indices)
        {
            var spec = abilities[index];
            if (spec.ability.CanActivate(spec, this))
            {
                return spec.abilityData.abilityId;
            }
        }

        return EAbilityId.None;
    }

    public DamageAbilityDataSO GetDamageAbilityData(EAbilityId abilityId)
    {
        AbilitySpec spec = abilities.Find(a => a.abilityData.abilityId == abilityId);
        if (spec?.abilityData is DamageAbilityDataSO damageAbilityDataSO)
        {
            return damageAbilityDataSO;
        }

        return null;
    }

    private void OnAnimationTriggered(EAnimationEventType eventType)
    {
        if (!waitingAbilitiesByAnimEvent.TryGetValue(eventType, out List<WaitingAbilityEntry> list)) return;

        WaitingAbilityEntry[] snapshot = list.ToArray(); // 중간에 remove되는 상황을 방지하기 위해 스냅샷.
        foreach (WaitingAbilityEntry entry in snapshot)
        {
            entry.callback?.Invoke();
            list.Remove(entry);
        }

        if (list.Count == 0)
            waitingAbilitiesByAnimEvent.Remove(eventType);
    }


}
