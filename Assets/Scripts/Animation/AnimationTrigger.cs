using System;
using UnityEngine;

public enum AnimationEventType
{
    None,
    AnimationEnd,
    Attack
}

public class AnimationTrigger : MonoBehaviour
{
    public event Action<AnimationEventType> OnAnimTriggered;

    public void AnimationEnd()
    {
        OnAnimTriggered?.Invoke(AnimationEventType.AnimationEnd);
    }

    public void Attack()
    {
        OnAnimTriggered?.Invoke(AnimationEventType.Attack);
    }
}