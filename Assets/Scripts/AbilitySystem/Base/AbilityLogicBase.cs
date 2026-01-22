using System;
using System.Collections;
using System.Threading;
using UnityEngine;

/* 실제 어빌리티 구현을 위한 베이스 클래스 */
public abstract class AbilityLogicBase
{
    private bool isAnimPlaying = false;

    // TODO: Context 멤버변수로 저장하는 방안도 고려.

    protected AbilityLogicBase()
    {
    }

    public void Init(AbilitySystemComponent asc)
    {
        
    }

    public abstract bool CanActivate(AbilitySpec spec, IAbilitySystemContext context);
    public abstract void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context);
    public abstract void EndAbility(AbilitySpec spec, IAbilitySystemContext context);
    public abstract void CancelAbility(AbilitySpec spec, IAbilitySystemContext context);

    protected void WaitAnimationEvent(AbilitySpec spec, IAbilitySystemContext context, EAnimationEventType animationEvent, Action callback)
    {
        context.RegisterWaitingAbility(animationEvent, spec, callback);
    }

    protected void PlayAnimationAndWait(AbilitySpec spec, IAbilitySystemContext context, Action callback)
    {
        if (isAnimPlaying) return;

        isAnimPlaying = true;
        context.StartCoroutine(PlayAndWaitAnimation(spec, context, callback));
    }

    private IEnumerator PlayAndWaitAnimation(AbilitySpec spec, IAbilitySystemContext context, Action callback)
    {
        Animator animator = context.Owner.Anim;

        // 기존 재생중인 애니메이션이 있으면 끝날 때까지 대기
        yield return new WaitUntil(() =>
            !animator.GetCurrentAnimatorStateInfo(0).IsTag(spec.abilityData.animTag) &&
            !animator.IsInTransition(0)
        );

        animator.SetBool(spec.abilityData.animName, true);

        // 애니메이션 재생 끝날때 까지 대기
        yield return new WaitUntil(() =>
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsTag(spec.abilityData.animTag) &&
                 animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        });

        animator.SetBool(spec.abilityData.animName, false);

        isAnimPlaying = false;
        callback?.Invoke();
    }


}
