using UnityEngine;

public class AnimationBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var projectile = animator.GetComponentInParent<ProjectileObjectBase>();
        if (projectile) projectile.OnDestroyAnimationFinished();
        
    }
}
