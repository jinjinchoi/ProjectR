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

    public override void ActivateAbility(AbilitySpec spec)
    {
        isActivated = true;
        StopComboResetTimer();

        if (spec.abilityData is not Player_NormalAttackDataSO attackAbilityData)
        {
            Debug.LogError("spec.abilityData is Not Player_NormalAttackDataSO Type at NormalAttackAbility");
            asc.EndAbilityBySpec(spec);
            return;
        }

        Animator animator = asc.owner.anim;
        animator.SetInteger(attackAbilityData.comboCountName, comboCount);

        PlayAnimationAndWait(spec, () =>
        {
            asc.EndAbilityBySpec(spec);
        });
    }


    public override void EndAbility(AbilitySpec spec)
    {
        if (spec.abilityData is not Player_NormalAttackDataSO attackData)
            return;

        comboCount++;

        if (attackData.maxComboCount < comboCount)
            comboCount = 1;
        else
            comboTimerCo = asc.StartCoroutine(ComboReset());
        
        isActivated = false;
    }

    private IEnumerator ComboReset()
    {
        yield return new WaitForSeconds(0.75f);
        comboCount = 1;
    }

    private void StopComboResetTimer()
    {
        if (comboTimerCo != null)
        {
            asc.StopCoroutine(comboTimerCo);
            comboTimerCo = null;
        }
    }


    public override bool CanActivate(AbilitySpec spec)
    {
        return !isActivated;
    }

    public override void CancelAbility(AbilitySpec spec)
    {
        asc.EndAbilityBySpec(spec);
    }

    public override void ReceiveAnimationEvent(AbilitySpec spec, AnimationEventType eventType)
    {
    }
}
