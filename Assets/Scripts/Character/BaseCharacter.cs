using System.Collections;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, IAbilityOwner, IDamageable
{

    #region IAbilityActor
    public Animator Anim => anim;
    public Transform OwnerTransform => transform;
    public AnimationTrigger AnimationTrigger => animationTrigger;
    public Transform AttackPoint => attackPoint;
    #endregion
    public Rigidbody2D Rb => rb;
    public AbilitySystemComponent ASC => abilitySystemComponent ? abilitySystemComponent : GetComponent<AbilitySystemComponent>();
    public float MoveSpeed => moveSpeed;
    public bool IsGrounded => isGrounded;
    public bool IsDead => isDead;

    [Header("Debug")]
    [SerializeField] protected bool showDebug = false;

    [Header("Hostile Target Detect")]
    [SerializeField] protected Vector2 hostileDetectSize = new Vector2(50, 10);
    [SerializeField] private LayerMask hostileLayerMask;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private string deadLayerName = "Dead";

    private Rigidbody2D rb;
    private AbilitySystemComponent abilitySystemComponent;
    private Animator anim;
    private AnimationTrigger animationTrigger;
    private VFXComponent vfxComponent;
    private LayerMask originalLayerMask;
    private bool isGrounded = false;
    private bool isFacingRight = true;
    private bool isKnockback = false;
    private Coroutine knockbackCo;
    private bool isDead = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        abilitySystemComponent = GetComponent<AbilitySystemComponent>();
        vfxComponent = GetComponent<VFXComponent>();

        animationTrigger = GetComponentInChildren<AnimationTrigger>();
        anim = GetComponentInChildren<Animator>();

        originalLayerMask = gameObject.layer;
    }

    protected virtual void Start()
    {
        abilitySystemComponent.AttributeSet.OnDaed += OnDead;
    }

    protected virtual void Update()
    {
        CheckGrounded();
    }

    public void TakeDamage(FDamageInfo damageInfo)
    {
        if (!abilitySystemComponent || isDead) return;

        FAttributeModifier damageModifier = new FAttributeModifier();
        damageModifier.attributeType = EAttributeType.incommingDamage;
        damageModifier.value = DamageCalculator.CalculateIncomingDamage(abilitySystemComponent.AttributeSet, damageInfo);
        damageModifier.isPermanent = true;
        damageModifier.operation = EModifierOp.Add;

        if (vfxComponent) vfxComponent.PlayOnDamageVfx();
        abilitySystemComponent.ApplyModifier(damageModifier);

        if (damageInfo.KnockbackPower != Vector2.zero)
        {
            if (knockbackCo != null)
                StopCoroutine(knockbackCo);

            int dir = transform.position.x > damageInfo.Instigator.OwnerTransform.position.x ? 1 : -1;
            Vector2 kncokback = damageInfo.KnockbackPower;
            kncokback.x *= dir;
            knockbackCo = StartCoroutine(ExcuteKnockback(kncokback, 0.15f));
        }
    }

    private IEnumerator ExcuteKnockback(Vector2 knockback, float duration)
    {
        isKnockback = true;
        rb.linearVelocity = knockback;
        
        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnockback = false;

    }

    private void OnDead()
    {
        isDead = true;
        int deadLayer = LayerMask.NameToLayer(deadLayerName);
        gameObject.layer = deadLayer;
    }

    private void Revive()
    {
        isDead = false;
        gameObject.layer = originalLayerMask;
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnockback || isDead) return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        anim.SetFloat("xVelocity", Mathf.Abs(xVelocity));
        HandleFlip(xVelocity);
    }


    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (xVelocity < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        if (isDead) return;

        transform.Rotate(0f, 180f, 0f);
        isFacingRight = !isFacingRight;
    }

    private void CheckGrounded()
    {
        if (!groundCheck) return;

        Collider2D result = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        if (result != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public Transform FindClosestTargetWithinBox()
    {
        Collider2D[] detectedEnemy = Physics2D.OverlapBoxAll(transform.position, hostileDetectSize, 0f, hostileLayerMask);

        if (detectedEnemy.Length == 0)
        {
            Debug.Log("detected Enemy is 0, FindClosestTarget");
            return null;
        }

        Transform closestTarget = null;
        float minSqrDistance = float.MaxValue;

        Vector2 myPos = transform.position;

        foreach (Collider2D enemy in detectedEnemy)
        {

            Vector2 enemyPos = enemy.transform.position;
            float sqrDist = (enemyPos - myPos).sqrMagnitude;

            if (sqrDist < minSqrDistance)
            {
                minSqrDistance = sqrDist;
                closestTarget = enemy.transform;
            }
        }

        return closestTarget;
    }

    protected virtual void OnDrawGizmos()
    {
        if (!showDebug) return;

        if (!groundCheck) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        abilitySystemComponent.AttributeSet.OnDaed -= OnDead;
    }
}
