using UnityEngine;
using UnityEngine.Pool;

public abstract class ProjectileObjectBase : MonoBehaviour, IPoolableObject
{
    static readonly int PlayHash = Animator.StringToHash("Play");

    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected bool hasDestroyAnim = false;

    protected IAbilityOwner owner;
    protected FAttackData attackData;

    private IObjectPool<GameObject> pool;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetObjectPool(IObjectPool<GameObject> pool)
    {
        this.pool = pool;
    }

    public void Init(Vector3 pos, IAbilityOwner owner, FAttackData attackData, int dir)
    {
        this.owner = owner;
        this.attackData = attackData;
        transform.position = pos;

        anim.Play(PlayHash, 0, 0);
        SetVelocity(dir);

        Invoke(nameof(OnLifeTimeExpired), lifeTime);
    }

    private void SetVelocity(int dir)
    {
        rb.linearVelocity = new Vector2(speed * dir, rb.linearVelocity.y);
        sr.flipX = dir < 0;
    }

    public void OnLifeTimeExpired()
    {
        PlayDestroyAnimation();
    }

    protected void PlayDestroyAnimation()
    {
        if (hasDestroyAnim)
            anim.SetTrigger("Destroy");
        else
            ReturnToPool();

        CancelInvoke(); // 싱글 Hit projectile이 이미 애니메이션 재생 중 함수 invoke 되는 것 방지
    }

    public void OnDestroyAnimationFinished()
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        pool.Release(gameObject);
    }
}
