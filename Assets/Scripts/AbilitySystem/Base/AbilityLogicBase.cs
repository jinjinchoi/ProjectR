using UnityEngine;

/* 실제 어빌리티 구현을 위한 베이스 클래스 */
public abstract class AbilityLogicBase
{
    protected AbilitySystemComponent asc;

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
}
