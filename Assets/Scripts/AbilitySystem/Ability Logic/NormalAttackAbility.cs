using System.Collections;
using UnityEngine;

public class NormalAttackAbility : AbilityLogicBase
{
    private Coroutine abilityCo;

    public NormalAttackAbility()
    {
    }

    public override void ActivateAbility(AbilitySpec spec)
    {
        if (abilityCo != null)
        {
            asc.StopCoroutine(abilityCo);
            abilityCo = null;
        }

        abilityCo = asc.StartCoroutine(PlayAndWaitAnimation(spec));
    }

    private IEnumerator PlayAndWaitAnimation(AbilitySpec spec)
    {
        Animator animator = asc.owner.anim;

        if (spec.abilityData is not DamageAbilityDataSO damageData)
            yield break;

        // 기존 재생중인 애니메이션이 있으면 끝날 때까지 대기
        yield return new WaitUntil(() =>
            !animator.GetCurrentAnimatorStateInfo(0).IsTag(damageData.animTag) &&
            !animator.IsInTransition(0)
        );

        animator.SetBool(damageData.animName, true);

        // 애니메이션 재생 끝날때 까지 대기
        yield return new WaitUntil(() =>
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsTag(damageData.animTag) &&
                 animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        });

        animator.SetBool(damageData.animName, false);

        asc.EndAbilityBySpec(spec);

    }

    public override void ReceiveAnimationEvent(AbilitySpec spec, AnimationEventType eventType)
    {

    }

    public override bool CanActivate(AbilitySpec spec)
    {
        return true;
    }

    public override void CancelAbility(AbilitySpec spec)
    {
        EndAbility(spec);
    }

    public override void EndAbility(AbilitySpec spec)
    {

    }

}
