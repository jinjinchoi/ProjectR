using System;
using System.Collections.Generic;
using UnityEngine;



public class AbilitySystemComponent : MonoBehaviour
{
    public event Action OnAbilityEnded;
    public BaseCharacter owner { get; private set; }

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
        if (currentSpec != null)
        {
            EndCurrentActivatedAbility();
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
    // Currently ends ability by comparing abilityId from AbilityData.
    // This assumes only one active AbilitySpec per abilityId.
    //
    // TODO (Refactor):
    // - Give AbilitySpec a unique runtime handle (e.g. int Handle)
    // - Store active specs in a Dictionary<Handle, AbilitySpec>
    // - EndAbilityBySpec should then use the spec handle instead of abilityId
    public void EndAbilityBySpec(AbilitySpec specToEnd)
    {
        foreach (var abilitySpec in abilities)
        {
            if (abilitySpec.abilityData.abilityId == specToEnd.abilityData.abilityId)
            {
                abilitySpec.ability.EndAbility(abilitySpec);
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
