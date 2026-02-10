using System.Collections;
using UnityEngine;

public class NormalAttackAbility : AbilityLogicBase
{
    private Coroutine comboTimerCo;
    private int comboCount = 1;
    

    public NormalAttackAbility()
    {
    }

    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.ActivateAbility(spec, context);

        StopComboResetTimer(context);

        if (spec.abilityData is not Common_NormalAttackDataSO attackAbilityData)
        {
            Debug.LogError("spec.abilityData is Not Player_NormalAttackDataSO Type at NormalAttackAbility");
            context.EndAbility(spec);
            return;
        }

        Animator animator = context.Owner.Anim;
        animator.SetInteger(attackAbilityData.comboCountName, comboCount);

        PlayAbilityAnimationAndWait(spec, context, () =>
        {
            context.EndAbility(spec);
        });

        WaitAnimationEvent(spec, context, EAnimationEventType.Attack, () =>
        {
            if (spec.abilityData is Common_NormalAttackDataSO attackData)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(context.Owner.AttackPoint.position, attackData.attackDamageRadius, attackData.hostileTargetLayer);

                FDamageInfo damageInfo = DamageCalculator.CalculateOutgoingDamage(context, attackData, context.Owner.Transform);

                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<IDamageable>(out var damageable))
                    {
                        damageable.TakeDamage(damageInfo);
                    }
                }
            }
        });
    }


    public override void OnEndAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.OnEndAbility(spec, context);

        if (spec.abilityData is not Common_NormalAttackDataSO attackData)
            return;

        comboCount++;

        if (attackData.maxComboCount < comboCount)
            comboCount = 1;
        else
            comboTimerCo = context.StartCoroutine(ComboReset());

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
}
