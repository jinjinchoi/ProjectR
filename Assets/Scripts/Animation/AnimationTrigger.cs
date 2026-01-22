using System;
using UnityEngine;

public enum EAnimationEventType
{
    None,
    AnimationEnd,
    Attack
}

public class AnimationTrigger : MonoBehaviour
{
    public event Action<EAnimationEventType> OnAnimTriggered;

    public void AnimationEnd()
    {
        OnAnimTriggered?.Invoke(EAnimationEventType.AnimationEnd);
    }

    public void Attack()
    {
        OnAnimTriggered?.Invoke(EAnimationEventType.Attack);
    }
}