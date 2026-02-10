using UnityEngine;
using UnityEngine.Pool;

public class AnimatorPooledEffect : MonoBehaviour
{
    static readonly int PlayHash = Animator.StringToHash("Play");

    IObjectPool<GameObject> pool;

    Animator animator;
    AnimatorOverrideController overrideController;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
    }

    public void Init(IObjectPool<GameObject> pool)
    {
        this.pool = pool;
    }

    public void Play(AnimationClip clip)
    {
        overrideController["BaseEffect_Active"] = clip;
        animator.Play(PlayHash, 0, 0);
        Invoke(nameof(ReturnToPool), clip.length);
    }

    void ReturnToPool()
    {
        pool.Release(gameObject);
    }
}
