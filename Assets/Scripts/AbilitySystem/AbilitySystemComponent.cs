using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IAbilityOwner
{
    Animator Anim { get; }
    Transform ActorTransform { get; }
    AnimationTrigger AnimationTrigger { get; }
}

public interface IAbilitySystemContext
{
    IAbilityOwner Owner { get; }

    void EndAbility(AbilitySpec spec);
    Coroutine StartCoroutine(IEnumerator routine);
    void StopCoroutine(Coroutine routine);
}

public class AbilitySystemComponent : MonoBehaviour, IAbilitySystemContext
{
    public event Action OnAbilityEnded;
    public IAbilityOwner Owner { get; private set; }

    /*
     * abilities :현재 보유하고 있는 ability 목록,
     * currentAbilitySpec :실행중인 ability
     */
    private List<AbilitySpec> abilities = new();
    private AbilitySpec currentSpec;
    //private Dictionary<AnimationEventType, List<AbilitySpec>> 

    public void Initialize(IAbilityOwner owner)
    {
        this.Owner = owner;
        Owner.AnimationTrigger.OnAnimTriggered -= OnAnimationTriggered;
        Owner.AnimationTrigger.OnAnimTriggered += OnAnimationTriggered;
    }

    public void GiveAbility(BaseAbilityDataSO data)
    {
        abilities.Add(new AbilitySpec(data, this));
    }


    public void EndAbility(AbilitySpec spec)
    {
        EndAbilityBySpec(spec);
    }

    public void TryActivateAbilityById(AbilityId abilityId)
    {
        if (Owner == null)
        {
            Debug.LogError($"Owner is not set on {gameObject.name}", this);
            return;
        }

        // 현재 어빌리티가 하나만 실행이 가능
        // TODO: 여러 어빌리티 실행 가능하면 List나 Dictionary에서 실행중인 어빌리티와 동일한 어빌리티를 실행하려는지를 확인해야함.
        if (currentSpec != null && !currentSpec.ability.CanActivate(currentSpec, this))
        {
            return;
        }

        AbilitySpec spec = abilities.Find(a => a.abilityData.abilityId == abilityId);
        if (spec != null && spec.ability.CanActivate(spec, this))
        {
            spec.ability.ActivateAbility(spec, this);
            currentSpec = spec;
        }
    }

    // NOTE:
    // 현재 하나의 ability만 실행이 가능하여 currentSpec에 실행중인 어빌리티 저장.
    // 종료시 아이디를 통하여 실행중인 어빌리티를 종료
    //
    // TODO (Refactor):
    // - string id가 아닌 int형 아이디를 통해 Dictionary<int id, AbilitySpec> 형태로 저장
    // - 여러 어빌리티를 실행하고 이에 아이디에 맞는 어빌리티 종료.
    // - int형을 사용안하더라도 list를 통해 여러 어빌리티 실행하게 구현도 가능.
    private void EndAbilityBySpec(AbilitySpec specToEnd)
    {
        foreach (var abilitySpec in abilities)
        {
            if (abilitySpec.abilityData.abilityId == specToEnd.abilityData.abilityId)
            {
                abilitySpec.ability.EndAbility(abilitySpec, this);
                currentSpec = null;

                OnAbilityEnded?.Invoke();
            }

        }
    }

    private void OnAnimationTriggered(AnimationEventType eventType)
    {

    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        Owner.AnimationTrigger.OnAnimTriggered -= OnAnimationTriggered;
    }

}
