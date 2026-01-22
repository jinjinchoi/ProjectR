using System.Collections;
using UnityEngine;

public class NormalAttackAbility : AbilityLogicBase
{
    private Coroutine comboTimerCo;
    private int comboCount = 1;
    bool isActivated = false;

    public NormalAttackAbility()
    {
    }

    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        isActivated = true;
        StopComboResetTimer(context);

        if (spec.abilityData is not Player_NormalAttackDataSO attackAbilityData)
        {
            Debug.LogError("spec.abilityData is Not Player_NormalAttackDataSO Type at NormalAttackAbility");
            context.EndAbility(spec);
            return;
        }

        Animator animator = context.Owner.Anim;
        animator.SetInteger(attackAbilityData.ComboCountName, comboCount);

        PlayAnimationAndWait(spec, context, () =>
        {
            context.EndAbility(spec);
        });

        WaitAnimationEvent(spec, context, EAnimationEventType.Attack, () =>
        {
            Debug.Log($"Attack : {comboCount}");
        });
    }


    public override void OnEndAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        if (spec.abilityData is not Player_NormalAttackDataSO attackData)
            return;

        comboCount++;

        if (attackData.MaxComboCount < comboCount)
            comboCount = 1;
        else
            comboTimerCo = context.StartCoroutine(ComboReset());
        
        isActivated = false;
    }

    private IEnumerator ComboReset()
    {
        yield return new WaitForSeconds(0.75f);
        comboCount = 1;
    }

    private void StopComboResetTimer(IAbilitySystemContext context)
    {
        if (comboTimerCo != null)
        {
            context.StopCoroutine(comboTimerCo);
            comboTimerCo = null;
        }
    }


    public override bool CanActivate(AbilitySpec spec, IAbilitySystemContext context)
    {
        return !isActivated;
    }

    public override void CancelAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        context.EndAbility(spec);
    }
}
