using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// onwer의 정보를 가져오는 인터페이스
public interface IAbilityOwner
{
    Animator Anim { get; }
    Transform ActorTransform { get; }
    AnimationTrigger AnimationTrigger { get; }
}

// 어빌리티가 직접 ASC에 접근하지 않고도 ASC의 기능을 사용하게 해주는 인터페이스
public interface IAbilitySystemContext
{
    IAbilityOwner Owner { get; }

    void EndAbility(AbilitySpec spec);
    Coroutine StartCoroutine(IEnumerator routine);
    void StopCoroutine(Coroutine routine);
    void RegisterWaitingAbility(EAnimationEventType eventType, AbilitySpec spec, Action callback);
    void UnregisterWaitingAbility(AbilitySpec spec);
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
    [SerializeField] private AttributeSO attributeInfoSO;

    public event Action OnAbilityEnded;
    public IAbilityOwner Owner { get; private set; }

    /*
     * abilities :현재 보유하고 있는 ability list,
     * activeAbilitySpecs :실행중인 ability map
     * waitingAbilitiesByAnimEvent: anim event를 기다리고 있는 ability map
     */
    private List<AbilitySpec> abilities = new();
    private Dictionary<EAbilityId, AbilitySpec> activeAbilitySpecs = new();
    private Dictionary<EAnimationEventType, List<WaitingAbilityEntry>> waitingAbilitiesByAnimEvent = new();

    private AttributeSet attributeSet;

    private void Awake()
    {
        attributeSet = new AttributeSet();

        foreach (AttributeInitInfo info in attributeInfoSO.attributes)
        {
            attributeSet.InitAttribute(info.attributeType, info.baseValue);
        }

        Debug.Log(attributeSet.GetAttributeValue(EAttributeType.physicalAttackPower));
    }

    public void Initialize(IAbilityOwner owner)
    {
        this.Owner = owner;
        Owner.AnimationTrigger.OnAnimTriggered -= OnAnimationTriggered;
        Owner.AnimationTrigger.OnAnimTriggered += OnAnimationTriggered;

        if (attributeInfoSO == null)
        {
            Debug.LogError($"attributeInfo is not Set to {gameObject.name}");
            return;
        }
    }

    public void GiveAbility(BaseAbilityDataSO data)
    {
        abilities.Add(new AbilitySpec(data, this));
    }

    public void EndAbility(AbilitySpec spec)
    {
        EndAbilityBySpec(spec);
    }

    public void TryActivateAbilityById(EAbilityId abilityId)
    {
        if (Owner == null)
        {
            Debug.LogError($"Owner is not set on {gameObject.name}", this);
            OnAbilityEnded?.Invoke();
            return;
        }

        if (activeAbilitySpecs.ContainsKey(abilityId))
        {
            return;
        }

        AbilitySpec spec = abilities.Find(a => a.abilityData.abilityId == abilityId);
        if (spec != null && spec.ability.CanActivate(spec, this))
        {
            spec.ability.ActivateAbility(spec, this);
            activeAbilitySpecs.Add(spec.abilityData.abilityId, spec);
        }
    }

    private void EndAbilityBySpec(AbilitySpec spec)
    {
        if (!activeAbilitySpecs.ContainsKey(spec.abilityData.abilityId))
            return;

        UnregisterWaitingAbility(spec);
        spec.ability.OnEndAbility(spec, this);
        activeAbilitySpecs.Remove(spec.abilityData.abilityId);

        OnAbilityEnded?.Invoke();

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

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        Owner.AnimationTrigger.OnAnimTriggered -= OnAnimationTriggered;
    }


}
