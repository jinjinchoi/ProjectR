using System;
using System.Collections;
using UnityEngine;

/* 실제 어빌리티 구현을 위한 베이스 클래스 */
public abstract class AbilityLogicBase
{
    private bool isAnimPlaying = false;

    protected bool isActivated = false;

    protected AbilityLogicBase()
    {
    }

    public void Init(AbilitySystemComponent asc)
    {
        
    }

    public abstract bool CanActivate(AbilitySpec spec, IAbilitySystemContext context);

    public virtual void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        isActivated = true;
    }

    public virtual void OnEndAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        isActivated = false;
        spec.lastActivatedTime = Time.time;
    }

    public virtual void CancelAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        OnEndAbility(spec, context);
    }

    protected void WaitAnimationEvent(AbilitySpec spec, IAbilitySystemContext context, EAnimationEventType animationEvent, Action callback)
    {
        context.RegisterWaitingAbility(animationEvent, spec, callback);
    }

    protected void PlayAbilityAnimationAndWait(AbilitySpec spec, IAbilitySystemContext context, Action callback)
    {
        if (isAnimPlaying) return;

        isAnimPlaying = true;
        context.StartCoroutine(PlayAndWaitAbilityAnimation(spec, context, callback));
    }

    private IEnumerator PlayAndWaitAbilityAnimation(AbilitySpec spec, IAbilitySystemContext context, Action callback)
    {
        Animator animator = context.Owner.Anim;

        // 기존 재생중인 애니메이션이 있으면 끝날 때까지 대기
        yield return new WaitUntil(() =>
            !animator.GetCurrentAnimatorStateInfo(0).IsTag(spec.abilityData.animTag) &&
            !animator.IsInTransition(0)
        );

        animator.SetBool(spec.abilityData.animBoolName, true);

        // 애니메이션 재생 끝날때 까지 대기
        yield return new WaitUntil(() =>
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsTag(spec.abilityData.animTag) &&
                 animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        });

        animator.SetBool(spec.abilityData.animBoolName, false);

        isAnimPlaying = false;
        callback?.Invoke();
    }

    protected bool IsCooldownReady(AbilitySpec spec)
    {
        if(spec.lastActivatedTime <= 0) return true;

        float elapsed = Time.time - spec.lastActivatedTime;
        return elapsed >= spec.abilityData.cooldown;
    }
    
    protected bool IsCooldownReadyAndNotActivated(AbilitySpec spec)
    {
        return IsCooldownReady(spec) && !isActivated;
    }

}
