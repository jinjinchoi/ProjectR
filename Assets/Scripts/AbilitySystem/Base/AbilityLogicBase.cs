using System;
using System.Collections;
using UnityEngine;

/* 실제 어빌리티 구현을 위한 베이스 클래스 */
public abstract class AbilityLogicBase
{
    protected AbilitySystemComponent asc;
    private Coroutine animPlayCo;
    private bool isAnimPlaying = false;

    protected AbilityLogicBase()
    {
    }

    public void Init(AbilitySystemComponent asc)
    {
        this.asc = asc;
    }

    public abstract bool CanActivate(AbilitySpec spec);
    public abstract void ActivateAbility(AbilitySpec spec);
    public abstract void EndAbility(AbilitySpec spec);
    public abstract void CancelAbility(AbilitySpec spec);
    public abstract void ReceiveAnimationEvent(AbilitySpec spec, AnimationEventType eventType);

    protected void PlayAnimationAndWait(AbilitySpec spec, Action callBack)
    {
        if (isAnimPlaying) return;

        isAnimPlaying = true;
        animPlayCo = asc.StartCoroutine(PlayAndWaitAnimation(spec, callBack));
    }

    private IEnumerator PlayAndWaitAnimation(AbilitySpec spec, Action callBack)
    {
        Animator animator = asc.owner.anim;

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
        animPlayCo = null;
        callBack?.Invoke();
    }
}
