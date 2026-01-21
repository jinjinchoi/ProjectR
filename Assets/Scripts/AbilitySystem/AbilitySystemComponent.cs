using System;
using System.Collections.Generic;
using UnityEngine;



public class AbilitySystemComponent : MonoBehaviour
{
    public event Action OnAbilityEnded;
    public BaseCharacter owner { get; private set; }

    // TODO: 우마무스메처럼 스킬 부여가 가능하게 하려면 전역 skill manager 만들어서 SO 파일 관리해야함.
    [SerializeField] private List<BaseAbilityDataSO> defaultAbilities;

    /*
     * abilities :현재 보유하고 있는 ability 목록, 
     * currentAbilitySpec :실행중인 ability
     */
    private List<AbilitySpec> abilities = new();
    private AbilitySpec currentSpec;

    private void Awake()
    {
        owner = GetComponent<BaseCharacter>();

        foreach (BaseAbilityDataSO data in defaultAbilities)
        {
            GiveAbility(data);
        }
    }

    private void GiveAbility(BaseAbilityDataSO data)
    {
        abilities.Add(new AbilitySpec(data, this));
    }

    public void TryActivateAbilityById(AbilityId abilityId)
    {
        // 현재 어빌리티가 하나만 실행이 가능
        // TODO: 여러 어빌리티 실행 가능하면 List나 Dictionary에서 실행중인 어빌리티와 동일한 어빌리티를 실행하려는지를 확인해야함.
        if (currentSpec != null && !currentSpec.ability.CanActivate(currentSpec))
        {
            return;
        }

        AbilitySpec spec = abilities.Find(a => a.abilityData.abilityId == abilityId);
        if (spec != null && spec.ability.CanActivate(spec))
        {
            spec.ability.ActivateAbility(spec);
            currentSpec = spec;
        }
    }

    public void EndCurrentActivatedAbility()
    {
        if (currentSpec == null) return;

        currentSpec.ability.EndAbility(currentSpec);
        currentSpec = null;

        OnAbilityEnded?.Invoke();
    }

    // NOTE:
    // 현재 하나의 ability만 실행이 가능하여 currentSpec에 실행중인 어빌리티 저장.
    // 종료시 아이디를 통하여 실행중인 어빌리티를 종료
    //
    // TODO (Refactor):
    // - string id가 아닌 int형 아이디를 통해 Dictionary<int id, AbilitySpec> 형태로 저장
    // - 여러 어빌리티를 실행하고 이에 아이디에 맞는 어빌리티 종료.
    // - int형을 사용안하더라도 list를 통해 여러 어빌리티 실행하게 구현도 가능.
    public void EndAbilityBySpec(AbilitySpec specToEnd)
    {
        foreach (var abilitySpec in abilities)
        {
            if (abilitySpec.abilityData.abilityId == specToEnd.abilityData.abilityId)
            {
                abilitySpec.ability.EndAbility(abilitySpec);
                currentSpec = null;

                OnAbilityEnded?.Invoke();
            }

        }
    }

    private void OnEnable()
    {
        owner.animationTrigger.OnAnimTriggered += OnAnimationTriggered;
    }

    private void OnDisable()
    {
        owner.animationTrigger.OnAnimTriggered -= OnAnimationTriggered;
    }

    private void OnAnimationTriggered(AnimationEventType eventType)
    {
        if (currentSpec == null) return;

        //if (eventType == AnimationEventType.AnimationEnd)
        //{
        //    EndCurrentActivatedAbility();
        //}
        //else
        //{
        //    currentSpec.ability.ReceiveAnimationEvent(currentSpec, eventType);
        //}
    }
}
